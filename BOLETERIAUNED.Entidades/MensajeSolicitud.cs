/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Mensaje de solicitud del protocolo TCP.
 */

using System.Text.Json;

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa un mensaje de solicitud enviado del cliente al servidor por TCP.
/// Contiene el comando a ejecutar y un diccionario de parámetros adicionales
/// serializados en formato JSON según <see cref="JsonConfig.Opciones"/>.
/// </summary>
public class MensajeSolicitud
{
    /// <summary>
    /// Nombre del comando a ejecutar en el servidor; corresponde a una constante de <see cref="ComandosRed"/>.
    /// </summary>
    public string Comando { get; set; } = string.Empty;

    /// <summary>
    /// Parámetros clave-valor que acompañan al comando (por ejemplo, identificación del cliente o id de partido).
    /// </summary>
    public Dictionary<string, string> Parametros { get; set; } = new();

    /// <summary>
    /// Deserializa una cadena JSON recibida por red en una instancia de <see cref="MensajeSolicitud"/>.
    /// </summary>
    /// <param name="json">Cadena JSON con la estructura del mensaje de solicitud.</param>
    /// <returns>Instancia deserializada o null si el JSON no es válido.</returns>
    public static MensajeSolicitud? Deserializar(string json)
    {
        // Usa las opciones compartidas para respetar codificación y convenciones web
        return JsonSerializer.Deserialize<MensajeSolicitud>(json, JsonConfig.Opciones);
    }

    /// <summary>
    /// Serializa la instancia actual a una cadena JSON lista para enviar por TCP.
    /// </summary>
    /// <returns>Cadena JSON compacta con comando y parámetros.</returns>
    public string Serializar()
    {
        // Convierte el objeto a JSON usando la configuración global del proyecto
        return JsonSerializer.Serialize(this, JsonConfig.Opciones);
    }
}
