/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Entidad Cliente que hereda de Persona.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa a un cliente del sistema de boletaría que puede realizar compras en línea
/// o ser atendido por un vendedor. Hereda los datos personales de <see cref="Persona"/>
/// e implementa <see cref="IValidable"/> para validación polimórfica.
/// </summary>
public class Cliente : Persona, IValidable
{
    /// <summary>
    /// Fecha en la que el cliente fue registrado en el sistema; no puede ser futura.
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>
    /// Indica si el cliente está activo y puede realizar compras en el sistema.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public Cliente()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa todas las propiedades del cliente.
    /// </summary>
    /// <param name="id">Identificador único del cliente en base de datos.</param>
    /// <param name="identificacion">Número de identificación personal del cliente.</param>
    /// <param name="nombre">Nombre del cliente.</param>
    /// <param name="apellido">Apellido del cliente.</param>
    /// <param name="fechaNacimiento">Fecha de nacimiento del cliente.</param>
    /// <param name="fechaRegistro">Fecha en que se registró el cliente en el sistema.</param>
    /// <param name="activo">Estado activo/inactivo del cliente.</param>
    public Cliente(int id, string identificacion, string nombre, string apellido,
        DateTime fechaNacimiento, DateTime fechaRegistro, bool activo)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        Id = id;
        Identificacion = identificacion;
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        FechaRegistro = fechaRegistro;
        Activo = activo;
    }

    /// <summary>
    /// Devuelve la etiqueta "Cliente" para identificar el tipo de persona en tiempo de ejecución.
    /// </summary>
    /// <returns>Cadena literal "Cliente".</returns>
    public override string ObtenerTipoPersona() => "Cliente";

    /// <summary>
    /// Valida las reglas de negocio del cliente: primero las comunes de persona
    /// y luego la fecha de registro.
    /// </summary>
    public void Validar()
    {
        // Reutiliza las validaciones compartidas definidas en la clase base Persona
        ValidarPersonaBase();

        // La fecha de registro no puede ser posterior al día actual
        if (FechaRegistro.Date > DateTime.Today)
        {
            throw new ArgumentException("La FechaRegistro no debe ser mayor al día actual.");
        }
    }
}
