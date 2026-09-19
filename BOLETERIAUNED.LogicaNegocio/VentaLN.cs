/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para Venta con control de concurrencia.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="Venta"/>.
/// Implementa registro de ventas en boletería y en línea con transacciones SQL
/// y semáforos (<see cref="SemaphoreSlim"/>) para evitar sobreventa concurrente.
/// </summary>
public class VentaLN
{
    /// <summary>
    /// Acceso a datos para insertar ventas y consultar historial.
    /// </summary>
    private readonly VentaAD _ventaAD = new();

    /// <summary>
    /// Acceso a datos para consultar y actualizar inventario de entradas por partido/localidad.
    /// </summary>
    private readonly LocalidadPorPartidoAD _localidadPartidoAD = new();

    /// <summary>
    /// Acceso a datos para validar existencia y estado del cliente comprador.
    /// </summary>
    private readonly ClienteAD _clienteAD = new();

    /// <summary>
    /// Acceso a datos para validar existencia y vigencia del partido.
    /// </summary>
    private readonly PartidoAD _partidoAD = new();

    /// <summary>
    /// Acceso a datos para validar existencia de la localidad y obtener su precio.
    /// </summary>
    private readonly LocalidadAD _localidadAD = new();

    /// <summary>
    /// Acceso a datos para validar existencia del vendedor en ventas de boletería.
    /// </summary>
    private readonly VendedorAD _vendedorAD = new();

    /// <summary>
    /// Diccionario de semáforos por combinación partido_localidad.
    /// Cada clave "IdPartido_IdLocalidad" tiene un SemaphoreSlim(1,1) que serializa
    /// las ventas concurrentes sobre el mismo inventario de entradas.
    /// </summary>
    private static readonly Dictionary<string, SemaphoreSlim> SemáforosVentas = new();

    /// <summary>
    /// Objeto de bloqueo para acceso thread-safe al diccionario de semáforos.
    /// Protege la creación y lectura de entradas en SemáforosVentas.
    /// </summary>
    private static readonly object BloqueoSemáforos = new();

    /// <summary>
    /// Registra una venta realizada en boletería física, con vendedor obligatorio.
    /// </summary>
    /// <param name="venta">Entidad Venta con cliente, partido, localidad, cantidad y vendedor.</param>
    /// <returns>IdVenta generado tras completar la transacción exitosamente.</returns>
    public int RegistrarVentaBoleteria(Venta venta)
    {
        venta.TipoVenta = "Boletería"; // Tipo fijo para ventas presenciales con vendedor
        ValidacionEntidad.Validar(venta);
        ValidarVentaComun(venta, requiereVendedor: true);
        return EjecutarVentaTransaccional(venta);
    }

    /// <summary>
    /// Registra una venta en línea (autogestión del cliente), sin vendedor asociado.
    /// </summary>
    /// <param name="venta">Entidad Venta con cliente, partido, localidad y cantidad.</param>
    /// <returns>IdVenta generado tras completar la transacción exitosamente.</returns>
    public int RegistrarVentaEnLinea(Venta venta)
    {
        venta.TipoVenta = "En línea"; // Tipo fijo para compras web/autoservicio
        venta.Vendedor = null; // Ventas en línea no tienen vendedor; IdVendedor será NULL en BD
        ValidacionEntidad.Validar(venta);
        ValidarVentaComun(venta, requiereVendedor: false);
        return EjecutarVentaTransaccional(venta);
    }

    /// <summary>
    /// Obtiene el historial completo de ventas con datos descriptivos para reportes.
    /// </summary>
    /// <returns>Lista de ConsultaVenta con información de cliente, partido, localidad y vendedor.</returns>
    public List<ConsultaVenta> ConsultarTodosDetalle()
    {
        return _ventaAD.ConsultarTodosDetalle();
    }

    /// <summary>
    /// Obtiene el historial de compras de un cliente específico.
    /// </summary>
    /// <param name="idCliente">Identificador del cliente.</param>
    /// <returns>Lista de ventas del cliente ordenadas de más reciente a más antigua.</returns>
    public List<ConsultaVenta> ConsultarPorCliente(int idCliente)
    {
        return _ventaAD.ConsultarPorCliente(idCliente);
    }

    /// <summary>
    /// Aplica todas las validaciones de negocio comunes a ventas de boletería y en línea.
    /// Enriquece la entidad Venta con objetos completos de BD y calcula monto total.
    /// </summary>
    /// <param name="venta">Entidad Venta a validar (se modifica in-place con datos completos).</param>
    /// <param name="requiereVendedor">
    /// <c>true</c> para ventas de boletería (vendedor obligatorio);
    /// <c>false</c> para ventas en línea.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si alguna regla de negocio no se cumple (cliente inactivo, sin stock, etc.).
    /// </exception>
    private void ValidarVentaComun(Venta venta, bool requiereVendedor)
    {
        // Regla de negocio: cantidad mínima de 1 entrada
        ValidadorComun.ValidarCantidad(venta.Cantidad);

        // Regla de negocio: el cliente debe existir en la BD
        var cliente = _clienteAD.ConsultarPorId(venta.Cliente.Id)
            ?? throw new InvalidOperationException("El cliente debe estar previamente registrado.");

        // Regla de negocio: solo clientes activos pueden comprar entradas
        if (!cliente.Activo)
        {
            throw new InvalidOperationException("El cliente debe estar activo.");
        }

        venta.Cliente = cliente; // Reemplaza referencia parcial con entidad completa de BD

        // Regla de negocio: el partido debe existir
        var partido = _partidoAD.ConsultarPorId(venta.Partido.IdPartido)
            ?? throw new InvalidOperationException("El partido debe estar previamente registrado.");

        // Regla de negocio: no se venden entradas de partidos desactivados
        if (!partido.Activo)
        {
            throw new InvalidOperationException("No se pueden vender entradas para partidos inactivos.");
        }

        // Regla de negocio: no se venden entradas de partidos con fecha pasada
        if (partido.Fecha.Date < DateTime.Today)
        {
            throw new InvalidOperationException("No se podrán realizar ventas de boletos para partidos con fecha menor al día actual.");
        }

        venta.Partido = partido;

        // Regla de negocio: la localidad debe existir en el catálogo
        var localidad = _localidadAD.ConsultarPorId(venta.Localidad.IdLocalidad)
            ?? throw new InvalidOperationException("La localidad debe estar previamente registrada.");

        venta.Localidad = localidad;

        // Regla de negocio: la localidad debe estar asociada al partido seleccionado
        var asociacion = _localidadPartidoAD.ConsultarPorPartidoLocalidad(partido.IdPartido, localidad.IdLocalidad)
            ?? throw new InvalidOperationException("La localidad debe estar asociada al partido seleccionado.");

        // Regla de negocio: verificación preliminar de stock (se revalida dentro de la transacción)
        if (asociacion.CantidadDisponible < venta.Cantidad)
        {
            throw new InvalidOperationException("No se pueden vender más entradas de las disponibles.");
        }

        if (requiereVendedor)
        {
            // Regla de negocio: ventas de boletería requieren vendedor registrado
            if (venta.Vendedor == null)
            {
                throw new InvalidOperationException("El vendedor debe estar previamente registrado.");
            }

            var vendedor = _vendedorAD.ConsultarPorId(venta.Vendedor.Id)
                ?? throw new InvalidOperationException("El vendedor debe estar previamente registrado.");

            venta.Vendedor = vendedor;
        }

        // Cálculo de negocio: fecha/hora actual de la venta y monto = cantidad × precio unitario
        venta.FechaVenta = DateTime.Now;
        venta.MontoTotal = venta.Cantidad * localidad.Precio;
    }

    /// <summary>
    /// Ejecuta la venta de forma atómica: descuenta inventario e inserta la venta en una transacción SQL,
    /// protegida por un semáforo que serializa accesos concurrentes al mismo partido/localidad.
    /// </summary>
    /// <param name="venta">Entidad Venta previamente validada y enriquecida.</param>
    /// <returns>IdVenta generado por la base de datos.</returns>
    private int EjecutarVentaTransaccional(Venta venta)
    {
        // Clave única por combinación partido+localidad para el semáforo de concurrencia
        var clave = $"{venta.Partido.IdPartido}_{venta.Localidad.IdLocalidad}";
        var semaforo = ObtenerSemaforo(clave);

        // Wait(): bloquea el hilo hasta obtener acceso exclusivo al inventario de esta partido/localidad
        semaforo.Wait();
        try
        {
            using var conexion = ConexionBD.CrearConexion();
            conexion.Open();
            // BeginTransaction: inicia transacción que agrupa UPDATE de stock + INSERT de venta
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Revalidación de stock dentro de la transacción (double-check pattern)
                // Evita condición de carrera entre la validación previa y el UPDATE
                var asociacionActual = _localidadPartidoAD.ConsultarPorPartidoLocalidad(
                    venta.Partido.IdPartido, venta.Localidad.IdLocalidad);

                if (asociacionActual == null || asociacionActual.CantidadDisponible < venta.Cantidad)
                {
                    throw new InvalidOperationException("No hay entradas suficientes disponibles.");
                }

                // Paso 1 de la transacción: descuenta entradas del inventario (UPDATE condicional)
                _localidadPartidoAD.ActualizarDisponibilidad(
                    conexion, transaccion,
                    venta.Partido.IdPartido,
                    venta.Localidad.IdLocalidad,
                    venta.Cantidad);

                // Paso 2 de la transacción: registra la venta y obtiene el IdVenta generado
                var idVenta = _ventaAD.Insertar(venta, conexion, transaccion);

                // Commit: confirma ambas operaciones de forma atómica (todo o nada)
                transaccion.Commit();
                return idVenta;
            }
            catch
            {
                // Rollback: revierte UPDATE e INSERT si cualquier paso falla
                transaccion.Rollback();
                throw;
            }
        }
        finally
        {
            // Release(): libera el semáforo para que otro hilo pueda procesar ventas del mismo inventario
            semaforo.Release();
        }
    }

    /// <summary>
    /// Obtiene o crea un semáforo exclusivo para la clave partido_localidad indicada.
    /// SemaphoreSlim(1, 1) permite un solo hilo concurrente por combinación.
    /// </summary>
    /// <param name="clave">Identificador compuesto "IdPartido_IdLocalidad".</param>
    /// <returns>Instancia de SemaphoreSlim asociada a esa clave de inventario.</returns>
    private static SemaphoreSlim ObtenerSemaforo(string clave)
    {
        // lock garantiza thread-safety al acceder/modificar el diccionario estático de semáforos
        lock (BloqueoSemáforos)
        {
            if (!SemáforosVentas.TryGetValue(clave, out var semaforo))
            {
                // SemaphoreSlim(1, 1): máximo 1 hilo en sección crítica simultáneamente
                semaforo = new SemaphoreSlim(1, 1);
                SemáforosVentas[clave] = semaforo;
            }

            return semaforo;
        }
    }
}
