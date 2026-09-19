/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para Partido.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="Partido"/>.
/// Gestiona el registro y consulta de eventos deportivos del estadio UNED.
/// </summary>
public class PartidoLN
{
    /// <summary>
    /// Instancia de acceso a datos para operaciones SQL sobre la tabla Partido.
    /// </summary>
    private readonly PartidoAD _accesoDatos = new();

    /// <summary>
    /// Registra un nuevo partido aplicando validaciones de entidad y unicidad de IdPartido.
    /// </summary>
    /// <param name="partido">Entidad Partido con rival, fecha, hora y estado activo.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el IdPartido ya existe o si ocurre un error SQL.
    /// </exception>
    public void Registrar(Partido partido)
    {
        // Valida rival no vacío, hora en formato HH:mm, fecha no futura, etc.
        ValidacionEntidad.Validar(partido);

        // Regla de negocio: cada partido debe tener un IdPartido único
        if (_accesoDatos.ExisteId(partido.IdPartido))
        {
            throw new InvalidOperationException("El IdPartido ya existe. Debe ser único.");
        }

        try
        {
            _accesoDatos.Insertar(partido);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Error al registrar partido: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los partidos registrados, sin filtrar por estado ni fecha.
    /// </summary>
    /// <returns>Lista completa de partidos del sistema.</returns>
    public List<Partido> ConsultarTodos()
    {
        return _accesoDatos.ConsultarTodos();
    }

    /// <summary>
    /// Obtiene los partidos activos cuya fecha es hoy o posterior (disponibles para venta).
    /// </summary>
    /// <returns>Lista de partidos vigentes ordenados por fecha y hora.</returns>
    public List<Partido> ConsultarActivos()
    {
        return _accesoDatos.ConsultarActivos();
    }
}
