/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Entidad Venta con objetos asociados (POO).
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa una venta de entradas registrada en el sistema. Agrupa por composición
/// los objetos <see cref="Cliente"/>, <see cref="Partido"/>, <see cref="Localidad"/>
/// y opcionalmente <see cref="Vendedor"/>, reflejando el modelo orientado a objetos del dominio.
/// </summary>
public class Venta : IValidable
{
    /// <summary>
    /// Identificador numérico único de la venta en la base de datos.
    /// </summary>
    public int IdVenta { get; set; }

    /// <summary>
    /// Cliente que realizó la compra de entradas.
    /// </summary>
    public Cliente Cliente { get; set; } = new();

    /// <summary>
    /// Partido para el cual se compraron las entradas.
    /// </summary>
    public Partido Partido { get; set; } = new();

    /// <summary>
    /// Localidad del estadio en la que se ubicaron las entradas vendidas.
    /// </summary>
    public Localidad Localidad { get; set; } = new();

    /// <summary>
    /// Vendedor que registró la venta presencial; es nulo cuando la compra fue en línea.
    /// </summary>
    public Vendedor? Vendedor { get; set; }

    /// <summary>
    /// Cantidad de entradas vendidas en esta transacción; debe ser mayor a cero.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Fecha y hora en que se registró la venta en el sistema.
    /// </summary>
    public DateTime FechaVenta { get; set; }

    /// <summary>
    /// Monto total cobrado por la venta (precio unitario × cantidad u otro cálculo aplicado).
    /// </summary>
    public decimal MontoTotal { get; set; }

    /// <summary>
    /// Tipo de venta realizada, por ejemplo "En línea" o "Presencial".
    /// </summary>
    public string TipoVenta { get; set; } = string.Empty;

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public Venta()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa una venta completa con todos sus objetos asociados.
    /// </summary>
    /// <param name="idVenta">Identificador único de la venta.</param>
    /// <param name="cliente">Cliente comprador.</param>
    /// <param name="partido">Partido asociado a la compra.</param>
    /// <param name="localidad">Localidad del estadio comprada.</param>
    /// <param name="vendedor">Vendedor presencial; null si la venta fue en línea.</param>
    /// <param name="cantidad">Número de entradas vendidas.</param>
    /// <param name="fechaVenta">Fecha de registro de la venta.</param>
    /// <param name="montoTotal">Monto total cobrado.</param>
    /// <param name="tipoVenta">Clasificación del canal de venta.</param>
    public Venta(int idVenta, Cliente cliente, Partido partido, Localidad localidad,
        Vendedor? vendedor, int cantidad, DateTime fechaVenta, decimal montoTotal, string tipoVenta)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        IdVenta = idVenta;
        Cliente = cliente;
        Partido = partido;
        Localidad = localidad;
        Vendedor = vendedor;
        Cantidad = cantidad;
        FechaVenta = fechaVenta;
        MontoTotal = montoTotal;
        TipoVenta = tipoVenta;
    }

    /// <summary>
    /// Valida que la cantidad vendida sea positiva y que el tipo de venta esté definido.
    /// </summary>
    public void Validar()
    {
        // Debe venderse al menos una entrada
        if (Cantidad <= 0)
        {
            throw new ArgumentException("La Cantidad debe ser mayor a cero.");
        }

        // El tipo de venta es obligatorio para distinguir canal en línea vs presencial
        if (string.IsNullOrWhiteSpace(TipoVenta))
        {
            throw new ArgumentException("El TipoVenta es requerido.");
        }
    }
}
