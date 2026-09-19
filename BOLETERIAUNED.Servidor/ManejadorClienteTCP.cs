/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Procesa solicitudes TCP con sesión de cliente autenticado.
 */

using System.Text.Json;
using BOLETERIAUNED.Entidades;
using BOLETERIAUNED.LogicaNegocio;

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Procesa solicitudes JSON recibidas por TCP desde la aplicación cliente.
/// Mantiene el estado de sesión del cliente autenticado y delega la persistencia
/// en las capas de lógica de negocio (<see cref="ClienteLN"/>, <see cref="VentaLN"/>, etc.).
/// </summary>
/// <remarks>
/// Cada instancia vive durante una conexión TCP; al desconectarse el cliente se pierde la sesión.
/// Los comandos sensibles exigen autenticación previa mediante <see cref="ComandosRed.ValidarCliente"/>
/// o <see cref="ComandosRed.ValidarClientePorId"/>.
/// </remarks>
public class ManejadorClienteTCP
{
    /// <summary>Callback para registrar eventos en la bitácora del servidor (desde hilos TCP).</summary>
    private readonly Action<string> _registrarBitacora;

    /// <summary>Callback para notificar a la UI que debe refrescar grillas de consulta.</summary>
    private readonly Action _notificarDatosActualizados;

    /// <summary>Capa de negocio para validación y consulta de clientes.</summary>
    private readonly ClienteLN _clienteLN = new();

    /// <summary>Capa de negocio para registro y consulta de ventas en línea.</summary>
    private readonly VentaLN _ventaLN = new();

    /// <summary>Capa de negocio para consulta de partidos activos.</summary>
    private readonly PartidoLN _partidoLN = new();

    /// <summary>Capa de negocio para localidades disponibles por partido.</summary>
    private readonly LocalidadPorPartidoLN _localidadPartidoLN = new();

    /// <summary>
    /// Identificador del cliente autenticado en esta sesión TCP.
    /// Null hasta que se ejecute un comando de validación exitoso.
    /// </summary>
    private int? _clienteSesionId;

    /// <summary>
    /// Crea un manejador vinculado a los callbacks de bitácora y actualización de datos de la UI.
    /// </summary>
    /// <param name="registrarBitacora">Acción que escribe en la bitácora del servidor.</param>
    /// <param name="notificarDatosActualizados">Acción que solicita refresco de consultas en el formulario.</param>
    public ManejadorClienteTCP(Action<string> registrarBitacora, Action notificarDatosActualizados)
    {
        _registrarBitacora = registrarBitacora;
        _notificarDatosActualizados = notificarDatosActualizados;
    }

    /// <summary>
    /// Deserializa una solicitud JSON, enruta al comando correspondiente y devuelve la respuesta serializada.
    /// </summary>
    /// <param name="jsonSolicitud">Cadena JSON con comando y parámetros según <see cref="MensajeSolicitud"/>.</param>
    /// <returns>Respuesta JSON serializada (<see cref="MensajeRespuesta"/>).</returns>
    public string ProcesarSolicitud(string jsonSolicitud)
    {
        try
        {
            var solicitud = MensajeSolicitud.Deserializar(jsonSolicitud)
                ?? throw new InvalidOperationException("Solicitud inválida.");

            // Registro en bitácora de cada comando recibido (auditoría de tráfico TCP).
            _registrarBitacora($"Consulta recibida: {solicitud.Comando}");

            // Enrutamiento por comando de red definido en la capa de entidades compartida.
            return solicitud.Comando switch
            {
                ComandosRed.ValidarCliente => ValidarCliente(solicitud),
                ComandosRed.ValidarClientePorId => ValidarClientePorId(solicitud),
                ComandosRed.ComprarEnLinea => ComprarEnLinea(solicitud),
                ComandosRed.ConsultarCompras => ConsultarCompras(solicitud),
                ComandosRed.ConsultarPartidosActivos => ConsultarPartidosActivos(),
                ComandosRed.ConsultarLocalidadesPartido => ConsultarLocalidadesPartido(solicitud),
                ComandosRed.Ping => MensajeRespuesta.Ok("PONG").Serializar(),
                _ => MensajeRespuesta.Error("Comando no reconocido.").Serializar()
            };
        }
        catch (Exception ex)
        {
            _registrarBitacora($"Error procesado: {ex.Message}");
            return MensajeRespuesta.Error(ex.Message).Serializar();
        }
    }

    /// <summary>
    /// Valida un cliente activo por número de identificación y establece la sesión TCP.
    /// </summary>
    /// <param name="solicitud">Solicitud con parámetro <c>Identificacion</c>.</param>
    /// <returns>Respuesta OK con datos del cliente en JSON.</returns>
    private string ValidarCliente(MensajeSolicitud solicitud)
    {
        var identificacion = ObtenerParametro(solicitud, "Identificacion");
        var cliente = _clienteLN.ValidarClienteActivo(identificacion);
        _clienteSesionId = cliente!.Id;
        var datos = JsonSerializer.Serialize(cliente, JsonConfig.Opciones);
        _registrarBitacora($"Cliente validado: {cliente.NombreCompleto}");
        return MensajeRespuesta.Ok("Cliente validado correctamente.", datos).Serializar();
    }

    /// <summary>
    /// Valida un cliente activo por Id numérico y establece la sesión TCP.
    /// </summary>
    /// <param name="solicitud">Solicitud con parámetro <c>IdCliente</c>.</param>
    /// <returns>Respuesta OK con datos del cliente en JSON.</returns>
    private string ValidarClientePorId(MensajeSolicitud solicitud)
    {
        var idCliente = ParsearEntero(ObtenerParametro(solicitud, "IdCliente"), "IdCliente");
        var cliente = _clienteLN.ValidarClienteActivoPorId(idCliente);
        _clienteSesionId = cliente!.Id;
        var datos = JsonSerializer.Serialize(cliente, JsonConfig.Opciones);
        _registrarBitacora($"Cliente validado por Id: {cliente.NombreCompleto}");
        return MensajeRespuesta.Ok("Cliente validado correctamente.", datos).Serializar();
    }

    /// <summary>
    /// Registra una compra en línea verificando que el IdCliente coincida con la sesión autenticada.
    /// </summary>
    /// <param name="solicitud">Solicitud con IdCliente, IdPartido, IdLocalidad y Cantidad.</param>
    /// <returns>Respuesta OK con IdVenta y monto total.</returns>
    private string ComprarEnLinea(MensajeSolicitud solicitud)
    {
        var idClienteSolicitud = ParsearEntero(ObtenerParametro(solicitud, "IdCliente"), "IdCliente");
        var idCliente = VerificarSesionCliente(idClienteSolicitud);

        var venta = new Venta
        {
            Cliente = new Cliente { Id = idCliente },
            Partido = new Partido { IdPartido = ParsearEntero(ObtenerParametro(solicitud, "IdPartido"), "IdPartido") },
            Localidad = new Localidad { IdLocalidad = ParsearEntero(ObtenerParametro(solicitud, "IdLocalidad"), "IdLocalidad") },
            Cantidad = ParsearEntero(ObtenerParametro(solicitud, "Cantidad"), "Cantidad")
        };

        var idVenta = _ventaLN.RegistrarVentaEnLinea(venta);
        _registrarBitacora($"Venta en línea procesada. IdVenta: {idVenta}");
        // Venta en línea modifica inventario: la UI del servidor debe refrescar consultas.
        _notificarDatosActualizados();
        return MensajeRespuesta.Ok($"Compra registrada correctamente. Número de venta: {idVenta}",
            JsonSerializer.Serialize(new { IdVenta = idVenta, venta.MontoTotal }, JsonConfig.Opciones)).Serializar();
    }

    /// <summary>
    /// Consulta el historial de compras del cliente autenticado.
    /// </summary>
    /// <param name="solicitud">Solicitud con parámetro <c>IdCliente</c>.</param>
    /// <returns>Respuesta OK con lista de ventas serializada en JSON.</returns>
    private string ConsultarCompras(MensajeSolicitud solicitud)
    {
        var idClienteSolicitud = ParsearEntero(ObtenerParametro(solicitud, "IdCliente"), "IdCliente");
        var idCliente = VerificarSesionCliente(idClienteSolicitud);
        var compras = _ventaLN.ConsultarPorCliente(idCliente);
        _registrarBitacora($"Consulta de compras del cliente Id {idCliente}");
        return MensajeRespuesta.Ok("Consulta exitosa.", JsonSerializer.Serialize(compras, JsonConfig.Opciones)).Serializar();
    }

    /// <summary>
    /// Devuelve los partidos activos disponibles para compra en línea.
    /// Requiere sesión autenticada.
    /// </summary>
    /// <returns>Respuesta OK con lista de partidos en JSON.</returns>
    private string ConsultarPartidosActivos()
    {
        VerificarSesionActiva();
        var partidos = _partidoLN.ConsultarActivos();
        return MensajeRespuesta.Ok("Partidos activos.", JsonSerializer.Serialize(partidos, JsonConfig.Opciones)).Serializar();
    }

    /// <summary>
    /// Devuelve las localidades (con disponibilidad) asociadas a un partido.
    /// Requiere sesión autenticada.
    /// </summary>
    /// <param name="solicitud">Solicitud con parámetro <c>IdPartido</c>.</param>
    /// <returns>Respuesta OK con localidades del partido en JSON.</returns>
    private string ConsultarLocalidadesPartido(MensajeSolicitud solicitud)
    {
        VerificarSesionActiva();
        var idPartido = ParsearEntero(ObtenerParametro(solicitud, "IdPartido"), "IdPartido");
        var localidades = _localidadPartidoLN.ConsultarPorPartido(idPartido);
        return MensajeRespuesta.Ok("Localidades del partido.", JsonSerializer.Serialize(localidades, JsonConfig.Opciones)).Serializar();
    }

    /// <summary>
    /// Verifica que exista sesión activa y que el Id solicitado coincida con el cliente autenticado.
    /// Evita que un cliente autenticado opere con datos de otro.
    /// </summary>
    /// <param name="idClienteSolicitud">Id de cliente enviado en la solicitud TCP.</param>
    /// <returns>Id del cliente autorizado para la operación.</returns>
    /// <exception cref="InvalidOperationException">Si no hay sesión o el Id no coincide.</exception>
    private int VerificarSesionCliente(int idClienteSolicitud)
    {
        VerificarSesionActiva();
        if (_clienteSesionId != idClienteSolicitud)
        {
            throw new InvalidOperationException("No está autorizado para operar con datos de otro cliente.");
        }

        return _clienteSesionId.Value;
    }

    /// <summary>
    /// Comprueba que el cliente haya sido autenticado previamente en esta conexión TCP.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si no hay sesión activa.</exception>
    private void VerificarSesionActiva()
    {
        if (_clienteSesionId == null)
        {
            throw new InvalidOperationException("Debe autenticarse antes de usar esta funcionalidad.");
        }
    }

    /// <summary>
    /// Obtiene y valida un parámetro obligatorio del diccionario de la solicitud.
    /// </summary>
    /// <param name="solicitud">Solicitud deserializada.</param>
    /// <param name="nombre">Nombre del parámetro requerido.</param>
    /// <returns>Valor del parámetro recortado de espacios.</returns>
    /// <exception cref="ArgumentException">Si el parámetro falta o está vacío.</exception>
    private static string ObtenerParametro(MensajeSolicitud solicitud, string nombre)
    {
        if (!solicitud.Parametros.TryGetValue(nombre, out var valor) || string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException($"Parámetro requerido: {nombre}");
        }

        return valor.Trim();
    }

    /// <summary>
    /// Convierte una cadena a entero con mensaje de error contextual.
    /// </summary>
    /// <param name="valor">Texto a parsear.</param>
    /// <param name="nombreCampo">Nombre del campo para el mensaje de error.</param>
    /// <returns>Entero parseado.</returns>
    /// <exception cref="ArgumentException">Si el valor no es un entero válido.</exception>
    private static int ParsearEntero(string valor, string nombreCampo)
    {
        if (!int.TryParse(valor, out var numero))
        {
            throw new ArgumentException($"El parámetro {nombreCampo} debe ser un número entero válido.");
        }

        return numero;
    }
}
