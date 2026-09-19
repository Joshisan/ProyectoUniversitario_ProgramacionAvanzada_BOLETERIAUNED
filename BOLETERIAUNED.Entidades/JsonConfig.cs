/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Opciones JSON con soporte UTF-8 para caracteres en español.
 */

using System.Text.Encodings.Web;
using System.Text.Json;

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Clase estática que expone las opciones compartidas de serialización JSON
/// para todo el sistema de boletaría. Garantiza compatibilidad web y correcta
/// representación de caracteres en español (tildes, eñes) en la comunicación TCP.
/// </summary>
public static class JsonConfig
{
    /// <summary>
    /// Opciones de serialización JSON preconfiguradas para uso en solicitudes y respuestas de red.
    /// Utiliza convenciones web, escape relajado para UTF-8 y salida compacta sin indentación.
    /// </summary>
    public static JsonSerializerOptions Opciones { get; } = new(JsonSerializerDefaults.Web)
    {
        // Permite emitir caracteres Unicode (español) sin escaparlos como \uXXXX
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // JSON en una sola línea para optimizar el tamaño del mensaje TCP
        WriteIndented = false
    };
}
