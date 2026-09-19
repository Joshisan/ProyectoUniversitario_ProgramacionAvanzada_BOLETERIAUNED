/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Modelo de consulta para Ventas en DataGridView.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Modelo de proyección (DTO) que aplana los datos de una venta y sus entidades relacionadas
/// para mostrarlos en un <c>DataGridView</c> o en respuestas JSON de consulta de compras.
/// No contiene lógica de negocio; solo transporta datos ya resueltos desde la capa de acceso.
/// </summary>
public class ConsultaVenta
{
    /// <summary>
    /// Identificador único de la venta.
    /// </summary>
    public int IdVenta { get; set; }

    /// <summary>
    /// Identificador numérico del cliente que realizó la compra.
    /// </summary>
    public int IdCliente { get; set; }

    /// <summary>
    /// Nombre completo del cliente para visualización en grillas e informes.
    /// </summary>
    public string ClienteNombre { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del equipo rival del partido asociado a la venta.
    /// </summary>
    public string PartidoRival { get; set; } = string.Empty;

    /// <summary>
    /// Fecha programada del partido comprado.
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
    /// Identificador numérico de la localidad vendida.
    /// </summary>
    public int IdLocalidad { get; set; }

    /// <summary>
    /// Nombre de la localidad del estadio donde se ubicaron las entradas.
    /// </summary>
    public string LocalidadNombre { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del vendedor; vacío si la venta fue en línea.
    /// </summary>
    public string VendedorNombre { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de entradas vendidas en la transacción.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Fecha en que se registró la venta.
    /// </summary>
    public DateTime FechaVenta { get; set; }

    /// <summary>
    /// Monto total cobrado por la venta.
    /// </summary>
    public decimal MontoTotal { get; set; }

    /// <summary>
    /// Tipo de canal de venta (por ejemplo, "En línea" o "Presencial").
    /// </summary>
    public string TipoVenta { get; set; } = string.Empty;
}
