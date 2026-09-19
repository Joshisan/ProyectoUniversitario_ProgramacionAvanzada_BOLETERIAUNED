/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Comandos del protocolo TCP cliente/servidor.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Clase estática que define las constantes de comandos del protocolo de comunicación TCP
/// entre el cliente WinForms y el servidor. Cada constante identifica una operación
/// que el servidor debe interpretar al recibir un <see cref="MensajeSolicitud"/>.
/// </summary>
public static class ComandosRed
{
    /// <summary>
    /// Comando para validar la existencia y estado de un cliente por su número de identificación.
    /// </summary>
    public const string ValidarCliente = "VALIDAR_CLIENTE";

    /// <summary>
    /// Comando para validar la existencia y estado de un cliente por su identificador numérico interno.
    /// </summary>
    public const string ValidarClientePorId = "VALIDAR_CLIENTE_ID";

    /// <summary>
    /// Comando para registrar una compra de entradas realizada en línea por el cliente.
    /// </summary>
    public const string ComprarEnLinea = "COMPRAR_EN_LINEA";

    /// <summary>
    /// Comando para consultar el historial de compras de un cliente específico.
    /// </summary>
    public const string ConsultarCompras = "CONSULTAR_COMPRAS";

    /// <summary>
    /// Comando para obtener la lista de partidos activos disponibles para venta.
    /// </summary>
    public const string ConsultarPartidosActivos = "CONSULTAR_PARTIDOS_ACTIVOS";

    /// <summary>
    /// Comando para consultar las localidades y disponibilidad asociadas a un partido.
    /// </summary>
    public const string ConsultarLocalidadesPartido = "CONSULTAR_LOCALIDADES_PARTIDO";

    /// <summary>
    /// Comando de verificación de conectividad (latido) entre cliente y servidor.
    /// </summary>
    public const string Ping = "PING";
}
