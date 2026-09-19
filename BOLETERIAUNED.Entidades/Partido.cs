/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Entidad Partido de fútbol.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Representa un partido de fútbol programado en el estadio, con rival, fecha, hora
/// y estado activo. Implementa <see cref="IValidable"/> para validar reglas de negocio
/// antes de persistir o usar el partido en ventas.
/// </summary>
public class Partido : IValidable
{
    /// <summary>
    /// Identificador numérico único del partido en la base de datos.
    /// </summary>
    public int IdPartido { get; set; }

    /// <summary>
    /// Nombre del equipo rival que enfrentará al equipo local en el partido.
    /// </summary>
    public string Rival { get; set; } = string.Empty;

    /// <summary>
    /// Fecha programada del partido; debe ser igual o posterior al día actual.
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Hora del partido en formato de 24 horas (HH:mm), por ejemplo "19:30".
    /// </summary>
    public string Hora { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el partido está activo y disponible para venta de entradas.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Constructor predeterminado sin parámetros, requerido para serialización y enlace de datos.
    /// </summary>
    public Partido()
    {
    }

    /// <summary>
    /// Constructor parametrizado que inicializa todas las propiedades del partido.
    /// </summary>
    /// <param name="idPartido">Identificador único del partido.</param>
    /// <param name="rival">Nombre del equipo rival.</param>
    /// <param name="fecha">Fecha programada del partido.</param>
    /// <param name="hora">Hora del partido en formato HH:mm.</param>
    /// <param name="activo">Estado activo/inactivo del partido.</param>
    public Partido(int idPartido, string rival, DateTime fecha, string hora, bool activo)
    {
        // Asignación directa de cada parámetro a su propiedad correspondiente
        IdPartido = idPartido;
        Rival = rival;
        Fecha = fecha;
        Hora = hora;
        Activo = activo;
    }

    /// <summary>
    /// Obtiene una descripción legible del partido con rival, fecha formateada y hora.
    /// </summary>
    public string DescripcionPartido => $"{Rival} - {Fecha:dd/MM/yyyy} {Hora}";

    /// <summary>
    /// Valida que el rival, la fecha y la hora cumplan las reglas de negocio del sistema.
    /// </summary>
    public void Validar()
    {
        // El nombre del rival es obligatorio
        if (string.IsNullOrWhiteSpace(Rival))
        {
            throw new ArgumentException("El campo Rival no debe quedar vacío.");
        }

        // No se permiten partidos con fecha anterior al día de hoy
        if (Fecha.Date < DateTime.Today)
        {
            throw new ArgumentException("La Fecha del partido no debe ser menor al día actual.");
        }

        // La hora debe existir y coincidir con el patrón HH:mm en formato 24 horas (00:00 a 23:59)
        if (string.IsNullOrWhiteSpace(Hora) || !System.Text.RegularExpressions.Regex.IsMatch(Hora.Trim(), @"^([01]\d|2[0-3]):[0-5]\d$"))
        {
            throw new ArgumentException("La Hora debe registrarse en formato válido HH:mm.");
        }
    }

    /// <summary>
    /// Devuelve la descripción formateada del partido como representación textual del objeto.
    /// </summary>
    /// <returns>Cadena con rival, fecha y hora del partido.</returns>
    public override string ToString() => DescripcionPartido;
}
