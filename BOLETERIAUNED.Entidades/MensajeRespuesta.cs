/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Mensaje de respuesta del protocolo TCP.
 */

using System.Text.Json;

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa un mensaje de respuesta enviado del servidor al cliente por TCP.
/// Indica si la operación fue exitosa, un mensaje descriptivo y datos opcionales en JSON.
/// </summary>
public class MensajeRespuesta
{
    /// <summary>
    /// Indica si la operación solicitada se completó correctamente (true) o falló (false).
    /// </summary>
    public bool Exito { get; set; }

    /// <summary>
    /// Mensaje descriptivo para el usuario o para registro en bitácora (éxito o error).
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Datos adicionales en formato JSON serializado como cadena; null cuando no hay payload.
    /// </summary>
    public string? Datos { get; set; }

    /// <summary>
    /// Crea una respuesta exitosa con mensaje opcional y datos JSON adjuntos.
    /// </summary>
    /// <param name="mensaje">Mensaje de confirmación para el cliente.</param>
    /// <param name="datos">Cadena JSON con datos de retorno; puede ser null.</param>
    /// <returns>Instancia de respuesta con <see cref="Exito"/> en true.</returns>
    public static MensajeRespuesta Ok(string mensaje, string? datos = null)
    {
        // Construye una respuesta positiva con los campos indicados
        return new MensajeRespuesta { Exito = true, Mensaje = mensaje, Datos = datos };
    }

    /// <summary>
    /// Crea una respuesta de error con un mensaje explicativo para el cliente.
    /// </summary>
    /// <param name="mensaje">Descripción del error ocurrido.</param>
    /// <returns>Instancia de respuesta con <see cref="Exito"/> en false y sin datos.</returns>
    public static MensajeRespuesta Error(string mensaje)
    {
        // Construye una respuesta negativa sin payload de datos
        return new MensajeRespuesta { Exito = false, Mensaje = mensaje };
    }

    /// <summary>
    /// Deserializa una cadena JSON recibida por red en una instancia de <see cref="MensajeRespuesta"/>.
    /// </summary>
    /// <param name="json">Cadena JSON con la estructura del mensaje de respuesta.</param>
    /// <returns>Instancia deserializada o null si el JSON no es válido.</returns>
    public static MensajeRespuesta? Deserializar(string json)
    {
        // Usa las opciones compartidas para respetar codificación y convenciones web
        return JsonSerializer.Deserialize<MensajeRespuesta>(json, JsonConfig.Opciones);
    }

    /// <summary>
    /// Serializa la instancia actual a una cadena JSON lista para enviar por TCP.
    /// </summary>
    /// <returns>Cadena JSON compacta con éxito, mensaje y datos opcionales.</returns>
    public string Serializar()
    {
        // Convierte el objeto a JSON usando la configuración global del proyecto
        return JsonSerializer.Serialize(this, JsonConfig.Opciones);
    }
}
