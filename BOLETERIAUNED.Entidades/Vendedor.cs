/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Entidad Vendedor que hereda de Persona.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa a un vendedor del sistema de boletaría que registra ventas presenciales.
/// Hereda los datos personales de <see cref="Persona"/> e implementa <see cref="IValidable"/>.
/// </summary>
public class Vendedor : Persona, IValidable
{
    /// <summary>
    /// Fecha en la que el vendedor ingresó a laborar; debe ser posterior a su nacimiento y no futura.
    /// </summary>
    public DateTime FechaIngreso { get; set; }

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public Vendedor()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa todas las propiedades del vendedor.
    /// </summary>
    /// <param name="id">Identificador único del vendedor en base de datos.</param>
    /// <param name="identificacion">Número de identificación personal del vendedor.</param>
    /// <param name="nombre">Nombre del vendedor.</param>
    /// <param name="apellido">Apellido del vendedor.</param>
    /// <param name="fechaNacimiento">Fecha de nacimiento del vendedor.</param>
    /// <param name="fechaIngreso">Fecha de ingreso laboral del vendedor.</param>
    public Vendedor(int id, string identificacion, string nombre, string apellido,
        DateTime fechaNacimiento, DateTime fechaIngreso)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        Id = id;
        Identificacion = identificacion;
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        FechaIngreso = fechaIngreso;
    }

    /// <summary>
    /// Devuelve la etiqueta "Vendedor" para identificar el tipo de persona en tiempo de ejecución.
    /// </summary>
    /// <returns>Cadena literal "Vendedor".</returns>
    public override string ObtenerTipoPersona() => "Vendedor";

    /// <summary>
    /// Valida las reglas de negocio del vendedor: datos personales base,
    /// fecha de ingreso no futura y coherencia con la fecha de nacimiento.
    /// </summary>
    public void Validar()
    {
        // Reutiliza las validaciones compartidas definidas en la clase base Persona
        ValidarPersonaBase();

        // La fecha de ingreso no puede ser posterior al día actual
        if (FechaIngreso.Date > DateTime.Today)
        {
            throw new ArgumentException("La FechaIngreso no debe ser mayor al día actual.");
        }

        // El vendedor debe haber nacido antes de ingresar a laborar
        if (FechaIngreso.Date <= FechaNacimiento.Date)
        {
            throw new ArgumentException("La FechaIngreso debe ser posterior a la FechaNacimiento.");
        }
    }
}
