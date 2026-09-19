/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Parseo de respuestas JSON de consulta de compras del cliente.
 */

using System.Text.Json;

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Clase estática utilitaria que interpreta el campo <c>Datos</c> de una
/// <see cref="MensajeRespuesta"/> del comando consultar compras y lo convierte
/// en una lista de <see cref="ConsultaVenta"/> para la interfaz de usuario.
/// </summary>
public static class ConsultaComprasParser
{
    /// <summary>
    /// Deserializa el JSON de datos de compras en una lista de proyecciones de venta.
    /// Devuelve lista vacía si no hay datos, son nulos o el JSON es la cadena literal "null".
    /// </summary>
    /// <param name="datos">Cadena JSON con arreglo de compras; puede ser null o vacía.</param>
    /// <returns>Lista de <see cref="ConsultaVenta"/> deserializada o lista vacía en caso de ausencia de datos.</returns>
    public static List<ConsultaVenta> DesdeJson(string? datos)
    {
        // Si no hay contenido o el servidor devolvió JSON null, no hay compras que mostrar
        if (string.IsNullOrWhiteSpace(datos) || datos == "null")
        {
            return new List<ConsultaVenta>();
        }

        // Intenta deserializar; si falla o retorna null, se devuelve lista vacía como valor seguro
        return JsonSerializer.Deserialize<List<ConsultaVenta>>(datos, JsonConfig.Opciones)
            ?? new List<ConsultaVenta>();
    }

    /// <summary>
    /// Genera un mensaje resumen legible según la cantidad de compras encontradas para un cliente.
    /// </summary>
    /// <param name="cantidad">Número total de registros de compra del cliente.</param>
    /// <returns>Mensaje informativo para mostrar en la interfaz de usuario.</returns>
    public static string ResumenCantidad(int cantidad)
    {
        // Operador ternario: mensaje distinto según haya o no registros
        return cantidad == 0
            ? "No hay compras registradas para este cliente"
            : $"Compras del cliente: {cantidad} registro(s)";
    }
}
