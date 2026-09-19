/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Entidad Localidad del estadio.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa una localidad (sección o zona) del estadio con su nombre y precio unitario.
/// Implementa <see cref="IValidable"/> para garantizar datos consistentes antes de usarlas en ventas.
/// </summary>
public class Localidad : IValidable
{
    /// <summary>
    /// Identificador numérico único de la localidad en la base de datos.
    /// </summary>
    public int IdLocalidad { get; set; }

    /// <summary>
    /// Nombre descriptivo de la localidad del estadio (por ejemplo, "Palco", "Gradería Norte").
    /// </summary>
    public string NombreLocalidad { get; set; } = string.Empty;

    /// <summary>
    /// Precio unitario de una entrada en esta localidad, expresado en colones; debe ser mayor a cero.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Valida que el nombre de la localidad no esté vacío y que el precio sea positivo.
    /// </summary>
    public void Validar()
    {
        // El nombre es obligatorio para identificar la sección del estadio
        if (string.IsNullOrWhiteSpace(NombreLocalidad))
        {
            throw new ArgumentException("El campo NombreLocalidad no debe quedar vacío.");
        }

        // El precio debe ser estrictamente mayor que cero para evitar ventas sin costo
        if (Precio <= 0)
        {
            throw new ArgumentException("El Precio debe ser mayor a cero.");
        }
    }

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public Localidad()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa todas las propiedades de la localidad.
    /// </summary>
    /// <param name="idLocalidad">Identificador único de la localidad.</param>
    /// <param name="nombreLocalidad">Nombre descriptivo de la localidad.</param>
    /// <param name="precio">Precio unitario de la entrada en colones.</param>
    public Localidad(int idLocalidad, string nombreLocalidad, decimal precio)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        IdLocalidad = idLocalidad;
        NombreLocalidad = nombreLocalidad;
        Precio = precio;
    }

    /// <summary>
    /// Devuelve una representación textual con el nombre de la localidad y su precio formateado.
    /// </summary>
    /// <returns>Cadena con nombre y precio en formato de moneda.</returns>
    public override string ToString() => $"{NombreLocalidad} - Col. {Precio:N2}";
}
