/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para la entidad Venta.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="Venta"/>.
/// Registra transacciones de compra de entradas y expone consultas detalladas con JOINs.
/// </summary>
public class VentaAD
{
    /// <summary>
    /// Inserta una venta dentro de una transacción SQL existente y retorna el IdVenta generado.
    /// Debe invocarse junto con ActualizarDisponibilidad para garantizar atomicidad.
    /// </summary>
    /// <param name="venta">Entidad Venta con cliente, partido, localidad, cantidad y montos.</param>
    /// <param name="conexion">Conexión SQL abierta compartida con la transacción.</param>
    /// <param name="transaccion">Transacción activa que agrupa el INSERT y la actualización de stock.</param>
    /// <returns>Identificador IdVenta autogenerado por SQL Server (OUTPUT INSERTED).</returns>
    public int Insertar(Venta venta, SqlConnection conexion, SqlTransaction transaccion)
    {
        // OUTPUT INSERTED.IdVenta: retorna el ID autoincremental generado en la misma sentencia
        const string sql = """
            INSERT INTO Venta (IdCliente, IdPartido, IdLocalidad, IdVendedor, Cantidad, FechaVenta, MontoTotal, TipoVenta)
            OUTPUT INSERTED.IdVenta
            VALUES (@IdCliente, @IdPartido, @IdLocalidad, @IdVendedor, @Cantidad, @FechaVenta, @MontoTotal, @TipoVenta)
            """;

        // Comando enlazado a la transacción: si falla, el INSERT se revierte con Rollback
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@IdCliente", venta.Cliente.Id);
        comando.Parameters.AddWithValue("@IdPartido", venta.Partido.IdPartido);
        comando.Parameters.AddWithValue("@IdLocalidad", venta.Localidad.IdLocalidad);
        // IdVendedor es nullable: ventas en línea no tienen vendedor (DBNull.Value)
        comando.Parameters.AddWithValue("@IdVendedor", (object?)venta.Vendedor?.Id ?? DBNull.Value);
        comando.Parameters.AddWithValue("@Cantidad", venta.Cantidad);
        comando.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
        comando.Parameters.AddWithValue("@MontoTotal", venta.MontoTotal);
        comando.Parameters.AddWithValue("@TipoVenta", venta.TipoVenta); // "Boletería" o "En línea"

        return Convert.ToInt32(comando.ExecuteScalar()); // Retorna el IdVenta recién insertado
    }

    /// <summary>
    /// Obtiene el historial completo de ventas con datos descriptivos de cliente, partido, localidad y vendedor.
    /// </summary>
    /// <returns>Lista de objetos ConsultaVenta enriquecidos para reportes.</returns>
    public List<ConsultaVenta> ConsultarTodosDetalle()
    {
        // JOIN con Cliente, Partido, Localidad; LEFT JOIN con Vendedor (puede ser NULL en ventas en línea)
        const string sql = """
            SELECT v.IdVenta, v.IdCliente, c.Nombre + ' ' + c.Apellido AS ClienteNombre,
                   p.Rival, p.Fecha, p.Hora, p.Activo,
                   v.IdLocalidad, l.NombreLocalidad,
                   CASE WHEN v.IdVendedor IS NULL THEN 'Sin vendedor'
                        ELSE ve.Nombre + ' ' + ve.Apellido END AS VendedorNombre,
                   v.Cantidad, v.FechaVenta, v.MontoTotal, v.TipoVenta
            FROM Venta v
            INNER JOIN Cliente c ON v.IdCliente = c.IdCliente
            INNER JOIN Partido p ON v.IdPartido = p.IdPartido
            INNER JOIN Localidad l ON v.IdLocalidad = l.IdLocalidad
            LEFT JOIN Vendedor ve ON v.IdVendedor = ve.IdVendedor
            ORDER BY v.IdVenta
            """;

        return EjecutarConsultaDetalle(sql);
    }

    /// <summary>
    /// Obtiene el historial de ventas de un cliente específico, del más reciente al más antiguo.
    /// </summary>
    /// <param name="idCliente">Identificador del cliente cuyas compras se consultan.</param>
    /// <returns>Lista de ventas del cliente con información descriptiva completa.</returns>
    public List<ConsultaVenta> ConsultarPorCliente(int idCliente)
    {
        // Misma estructura de JOIN que ConsultarTodosDetalle, filtrada por IdCliente
        const string sql = """
            SELECT v.IdVenta, v.IdCliente, c.Nombre + ' ' + c.Apellido AS ClienteNombre,
                   p.Rival, p.Fecha, p.Hora, p.Activo,
                   v.IdLocalidad, l.NombreLocalidad,
                   CASE WHEN v.IdVendedor IS NULL THEN 'Sin vendedor'
                        ELSE ve.Nombre + ' ' + ve.Apellido END AS VendedorNombre,
                   v.Cantidad, v.FechaVenta, v.MontoTotal, v.TipoVenta
            FROM Venta v
            INNER JOIN Cliente c ON v.IdCliente = c.IdCliente
            INNER JOIN Partido p ON v.IdPartido = p.IdPartido
            INNER JOIN Localidad l ON v.IdLocalidad = l.IdLocalidad
            LEFT JOIN Vendedor ve ON v.IdVendedor = ve.IdVendedor
            WHERE v.IdCliente = @IdCliente
            ORDER BY v.IdVenta DESC
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdCliente", idCliente);
        conexion.Open();
        return LeerConsultaDetalle(comando);
    }

    /// <summary>
    /// Ejecuta una consulta de detalle de ventas creando su propia conexión.
    /// </summary>
    /// <param name="sql">Sentencia SELECT con JOINs ya definida.</param>
    /// <returns>Lista de ConsultaVenta mapeada desde el resultado.</returns>
    private List<ConsultaVenta> EjecutarConsultaDetalle(string sql)
    {
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        return LeerConsultaDetalle(comando);
    }

    /// <summary>
    /// Lee todas las filas de un SqlCommand y las convierte en objetos ConsultaVenta.
    /// </summary>
    /// <param name="comando">Comando SQL ya configurado con parámetros (si aplica).</param>
    /// <returns>Lista de ventas con datos descriptivos para la capa de presentación.</returns>
    private static List<ConsultaVenta> LeerConsultaDetalle(SqlCommand comando)
    {
        var lista = new List<ConsultaVenta>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            // Mapeo de las 14 columnas del SELECT enriquecido con JOINs
            lista.Add(new ConsultaVenta
            {
                IdVenta = lector.GetInt32(0),
                IdCliente = lector.GetInt32(1),
                ClienteNombre = lector.GetString(2),
                PartidoRival = lector.GetString(3),
                PartidoFecha = lector.GetDateTime(4),
                PartidoHora = lector.GetString(5),
                PartidoActivo = lector.GetBoolean(6) ? "Sí" : "No",
                IdLocalidad = lector.GetInt32(7),
                LocalidadNombre = lector.GetString(8),
                VendedorNombre = lector.GetString(9),
                Cantidad = lector.GetInt32(10),
                FechaVenta = lector.GetDateTime(11),
                MontoTotal = lector.GetDecimal(12),
                TipoVenta = lector.GetString(13)
            });
        }

        return lista;
    }
}
