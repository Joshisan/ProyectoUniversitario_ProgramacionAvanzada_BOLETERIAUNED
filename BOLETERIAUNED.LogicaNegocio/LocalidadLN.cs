/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para Localidad.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="Localidad"/>.
/// Administra las secciones del estadio y su precio base por entrada.
/// </summary>
public class LocalidadLN
{
    /// <summary>
    /// Instancia de acceso a datos para operaciones SQL sobre la tabla Localidad.
    /// </summary>
    private readonly LocalidadAD _accesoDatos = new();

    /// <summary>
    /// Registra una nueva localidad (sección del estadio) con validaciones de negocio.
    /// </summary>
    /// <param name="localidad">Entidad Localidad con id, nombre y precio unitario.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el IdLocalidad ya existe o si ocurre un error SQL.
    /// </exception>
    public void Registrar(Localidad localidad)
    {
        // Valida nombre no vacío y precio mayor a cero mediante ValidacionEntidad
        ValidacionEntidad.Validar(localidad);

        // Regla de negocio: IdLocalidad único en el catálogo de secciones
        if (_accesoDatos.ExisteId(localidad.IdLocalidad))
        {
            throw new InvalidOperationException("El IdLocalidad ya existe. Debe ser único.");
        }

        try
        {
            _accesoDatos.Insertar(localidad);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Error al registrar localidad: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las localidades registradas en el catálogo del estadio.
    /// </summary>
    /// <returns>Lista de localidades con nombre y precio.</returns>
    public List<Localidad> ConsultarTodos()
    {
        return _accesoDatos.ConsultarTodos();
    }
}
