/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para Vendedor.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="Vendedor"/>.
/// Aplica reglas de unicidad y validación antes de persistir vendedores en la BD.
/// </summary>
public class VendedorLN
{
    /// <summary>
    /// Instancia de acceso a datos para operaciones SQL sobre la tabla Vendedor.
    /// </summary>
    private readonly VendedorAD _accesoDatos = new();

    /// <summary>
    /// Registra un nuevo vendedor en el sistema con validaciones de negocio completas.
    /// </summary>
    /// <param name="vendedor">Entidad Vendedor con datos personales y fecha de ingreso.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el IdVendedor o la Identificación ya existen, o si ocurre un error SQL.
    /// </exception>
    public void Registrar(Vendedor vendedor)
    {
        // Valida campos obligatorios, formatos de fecha y longitud de identificación
        ValidacionEntidad.Validar(vendedor);

        // Regla de negocio: IdVendedor único en el sistema
        if (_accesoDatos.ExisteId(vendedor.Id))
        {
            throw new InvalidOperationException("El IdVendedor ya existe. Debe ser único.");
        }

        // Regla de negocio: identificación única por vendedor
        if (_accesoDatos.ExisteIdentificacion(vendedor.Identificacion))
        {
            throw new InvalidOperationException("La Identificación del vendedor ya existe.");
        }

        try
        {
            _accesoDatos.Insertar(vendedor);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Error al registrar vendedor: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene la lista completa de vendedores registrados en el sistema.
    /// </summary>
    /// <returns>Colección de entidades Vendedor ordenadas por IdVendedor.</returns>
    public List<Vendedor> ConsultarTodos()
    {
        return _accesoDatos.ConsultarTodos();
    }
}
