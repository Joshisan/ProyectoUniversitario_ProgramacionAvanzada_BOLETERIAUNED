/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Modelo de consulta para LocalidadPorPartido en DataGridView.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Modelo de proyección (DTO) que combina datos de partido, localidad y disponibilidad
/// en un único registro plano para enlazar con un <c>DataGridView</c> o respuestas de red.
/// Facilita la consulta de inventario por partido sin navegar objetos anidados en la UI.
/// </summary>
public class ConsultaLocalidadPorPartido
{
    /// <summary>
    /// Identificador único del registro de localidad por partido.
    /// </summary>
    public int IdLocalidadPartido { get; set; }

    /// <summary>
    /// Identificador numérico del partido asociado.
    /// </summary>
    public int IdPartido { get; set; }

    /// <summary>
    /// Nombre del equipo rival del partido.
    /// </summary>
    public string PartidoRival { get; set; } = string.Empty;

    /// <summary>
    /// Fecha programada del partido.
    /// </summary>
    public DateTime PartidoFecha { get; set; }

    /// <summary>
    /// Hora del partido en formato HH:mm.
    /// </summary>
    public string PartidoHora { get; set; } = string.Empty;

    /// <summary>
    /// Estado del partido como texto legible (por ejemplo, "Sí" o "No" para activo).
    /// </summary>
    public string PartidoActivo { get; set; } = string.Empty;

    /// <summary>
    /// Identificador numérico de la localidad del estadio.
    /// </summary>
    public int IdLocalidad { get; set; }

    /// <summary>
    /// Nombre descriptivo de la localidad.
    /// </summary>
    public string LocalidadNombre { get; set; } = string.Empty;

    /// <summary>
    /// Precio unitario de la entrada en esa localidad.
    /// </summary>
    public decimal LocalidadPrecio { get; set; }

    /// <summary>
    /// Cantidad de entradas aún disponibles para venta en esta combinación partido-localidad.
    /// </summary>
    public int CantidadDisponible { get; set; }
}
