/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Clase abstracta base para Cliente y Vendedor (herencia).
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Clase abstracta base que representa los atributos y comportamientos comunes
/// compartidos por <see cref="Cliente"/> y <see cref="Vendedor"/>.
/// Implementa herencia para reutilizar validaciones y propiedades de identificación personal.
/// </summary>
public abstract class Persona
{
    /// <summary>
    /// Identificador numérico único de la persona en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número de identificación personal (cédula u otro documento), máximo 10 caracteres.
    /// </summary>
    public string Identificacion { get; set; } = string.Empty;

    /// <summary>
    /// Primer nombre de la persona.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido(s) de la persona.
    /// </summary>
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento de la persona; debe ser anterior al día actual.
    /// </summary>
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Obtiene el nombre completo concatenando nombre y apellido, eliminando espacios sobrantes.
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    /// <summary>
    /// Método abstracto que obliga a las clases derivadas a indicar su tipo concreto
    /// (por ejemplo, "Cliente" o "Vendedor") para uso polimórfico en la aplicación.
    /// </summary>
    /// <returns>Cadena que identifica el tipo de persona.</returns>
    public abstract string ObtenerTipoPersona();

    /// <summary>
    /// Valida las reglas de negocio comunes a toda persona: identificación, nombre,
    /// apellido y fecha de nacimiento. Lanza <see cref="ArgumentException"/> si alguna falla.
    /// </summary>
    protected void ValidarPersonaBase()
    {
        // La identificación es obligatoria y no puede ser nula, vacía ni solo espacios
        if (string.IsNullOrWhiteSpace(Identificacion))
        {
            throw new ArgumentException("La Identificación no debe quedar vacía.");
        }

        // Se eliminan espacios laterales y se verifica el límite de 10 caracteres del campo
        if (Identificacion.Trim().Length > 10)
        {
            throw new ArgumentException("La Identificación no debe exceder 10 caracteres.");
        }

        // Tanto el nombre como el apellido deben contener texto válido
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
        {
            throw new ArgumentException("Los campos Nombre y Apellido no deben quedar vacíos.");
        }

        // La fecha de nacimiento debe ser estrictamente anterior al día de hoy (solo fecha, sin hora)
        if (FechaNacimiento.Date >= DateTime.Today)
        {
            throw new ArgumentException("La FechaNacimiento no debe ser mayor o igual al día actual.");
        }
    }
}
