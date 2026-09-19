/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Asociación entre Partido y Localidad con disponibilidad.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Entidad de asociación que vincula un <see cref="Partido"/> con una <see cref="Localidad"/>
/// e indica cuántas entradas quedan disponibles para esa combinación.
/// Permite gestionar inventario de boletos por partido y sección del estadio.
/// </summary>
public class LocalidadPorPartido : IValidable
{
    /// <summary>
    /// Identificador numérico único del registro de localidad por partido en la base de datos.
    /// </summary>
    public int IdLocalidadPartido { get; set; }

    /// <summary>
    /// Objeto partido asociado a esta disponibilidad de localidad.
    /// </summary>
    public Partido Partido { get; set; } = new();

    /// <summary>
    /// Objeto localidad asociado a este partido, con su nombre y precio.
    /// </summary>
    public Localidad Localidad { get; set; } = new();

    /// <summary>
    /// Cantidad de entradas aún disponibles para vender en esta localidad para el partido indicado.
    /// </summary>
    public int CantidadDisponible { get; set; }

    /// <summary>
    /// Obtiene una descripción legible con nombre de localidad, precio y cantidad disponible.
    /// </summary>
    public string DescripcionLocalidad =>
        $"{Localidad.NombreLocalidad} - Col. {Localidad.Precio:N0} (Disp: {CantidadDisponible})";

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public LocalidadPorPartido()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa la asociación partido-localidad con su inventario.
    /// </summary>
    /// <param name="idLocalidadPartido">Identificador único del registro.</param>
    /// <param name="partido">Partido al que pertenece la disponibilidad.</param>
    /// <param name="localidad">Localidad del estadio asociada al partido.</param>
    /// <param name="cantidadDisponible">Número de entradas disponibles para venta.</param>
    public LocalidadPorPartido(int idLocalidadPartido, Partido partido, Localidad localidad, int cantidadDisponible)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        IdLocalidadPartido = idLocalidadPartido;
        Partido = partido;
        Localidad = localidad;
        CantidadDisponible = cantidadDisponible;
    }

    /// <summary>
    /// Valida que exista al menos una entrada disponible para la combinación partido-localidad.
    /// </summary>
    public void Validar()
    {
        // La cantidad disponible debe ser positiva para permitir ventas
        if (CantidadDisponible <= 0)
        {
            throw new ArgumentException("La CantidadDisponible debe ser mayor a cero.");
        }
    }
}
