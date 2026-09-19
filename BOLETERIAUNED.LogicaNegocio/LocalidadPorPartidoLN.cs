/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para LocalidadPorPartido.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="LocalidadPorPartido"/>.
/// Gestiona la asociación entre partidos y localidades con su inventario de entradas.
/// </summary>
public class LocalidadPorPartidoLN
{
    /// <summary>
    /// Acceso a datos de la tabla LocalidadPorPartido (asociación e inventario).
    /// </summary>
    private readonly LocalidadPorPartidoAD _accesoDatos = new();

    /// <summary>
    /// Acceso a datos de Partido para verificar existencia y estado activo.
    /// </summary>
    private readonly PartidoAD _partidoAD = new();

    /// <summary>
    /// Acceso a datos de Localidad para verificar que la sección esté registrada.
    /// </summary>
    private readonly LocalidadAD _localidadAD = new();

    /// <summary>
    /// Registra una nueva asociación partido-localidad con cantidad de entradas disponibles.
    /// </summary>
    /// <param name="registro">Entidad LocalidadPorPartido con partido, localidad y cupo.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si falla alguna regla de negocio (unicidad, existencia, partido activo) o error SQL.
    /// </exception>
    public void Registrar(LocalidadPorPartido registro)
    {
        // Valida cantidad > 0 y referencias no nulas en la entidad
        ValidacionEntidad.Validar(registro);

        // Regla de negocio: IdLocalidadPartido único como clave primaria de la asociación
        if (_accesoDatos.ExisteId(registro.IdLocalidadPartido))
        {
            throw new InvalidOperationException("El IdLocalidadPartido ya existe. Debe ser único.");
        }

        // Regla de negocio: el partido debe existir previamente en la BD (integridad referencial)
        var partido = _partidoAD.ConsultarPorId(registro.Partido.IdPartido)
            ?? throw new InvalidOperationException("El partido debe estar previamente registrado.");

        // Regla de negocio: no se asignan localidades a partidos desactivados
        if (!partido.Activo)
        {
            throw new InvalidOperationException("No se pueden registrar localidades para partidos inactivos.");
        }

        // Regla de negocio: la localidad (sección) debe existir en el catálogo
        if (_localidadAD.ConsultarPorId(registro.Localidad.IdLocalidad) == null)
        {
            throw new InvalidOperationException("La localidad debe estar previamente registrada.");
        }

        // Regla de negocio: una localidad solo puede asociarse una vez al mismo partido
        if (_accesoDatos.ExistePartidoLocalidad(registro.Partido.IdPartido, registro.Localidad.IdLocalidad))
        {
            throw new InvalidOperationException("No se puede registrar la misma localidad para el mismo partido más de una vez.");
        }

        // Enriquece el registro con el partido completo obtenido de BD (datos actualizados)
        registro.Partido = partido;

        try
        {
            _accesoDatos.Insertar(registro);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Error al registrar localidad por partido: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene el detalle completo de todas las asociaciones partido-localidad para reportes.
    /// </summary>
    /// <returns>Lista de ConsultaLocalidadPorPartido con datos descriptivos.</returns>
    public List<ConsultaLocalidadPorPartido> ConsultarTodosDetalle()
    {
        return _accesoDatos.ConsultarTodosDetalle();
    }

    /// <summary>
    /// Obtiene las localidades con entradas disponibles para un partido específico.
    /// </summary>
    /// <param name="idPartido">Identificador del partido a consultar.</param>
    /// <returns>Lista de asociaciones con stock mayor a cero.</returns>
    public List<LocalidadPorPartido> ConsultarPorPartido(int idPartido)
    {
        return _accesoDatos.ConsultarPorPartido(idPartido);
    }
}
