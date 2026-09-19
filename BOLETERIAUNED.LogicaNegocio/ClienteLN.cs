/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Lógica de negocio para Cliente.
 */

using BOLETERIAUNED.AccesoDatos;
using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Capa de lógica de negocio (LN) para la entidad <see cref="Cliente"/>.
/// Orquesta validaciones de entidad, reglas de unicidad y operaciones de acceso a datos.
/// </summary>
public class ClienteLN
{
    /// <summary>
    /// Instancia de acceso a datos para delegar operaciones SQL sobre la tabla Cliente.
    /// </summary>
    private readonly ClienteAD _accesoDatos = new();

    /// <summary>
    /// Registra un nuevo cliente en el sistema aplicando todas las validaciones de negocio.
    /// </summary>
    /// <param name="cliente">Entidad Cliente con datos completos a persistir.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el IdCliente o la Identificación ya existen, o si ocurre un error SQL.
    /// </exception>
    public void Registrar(Cliente cliente)
    {
        // Validación de atributos de la entidad (campos obligatorios, formatos, rangos)
        ValidacionEntidad.Validar(cliente);

        // Regla de negocio: IdCliente debe ser único en todo el sistema
        if (_accesoDatos.ExisteId(cliente.Id))
        {
            throw new InvalidOperationException("El IdCliente ya existe. Debe ser único.");
        }

        // Regla de negocio: no pueden existir dos clientes con la misma identificación
        if (_accesoDatos.ExisteIdentificacion(cliente.Identificacion))
        {
            throw new InvalidOperationException("La Identificación del cliente ya existe.");
        }

        try
        {
            _accesoDatos.Insertar(cliente);
        }
        catch (SqlException ex)
        {
            // Captura errores de BD (restricciones FK, conexión, etc.) y los expone con contexto
            throw new InvalidOperationException($"Error al registrar cliente: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Valida que un cliente exista y esté activo, buscándolo por identificación.
    /// </summary>
    /// <param name="identificacion">Número de identificación del cliente.</param>
    /// <returns>Entidad Cliente activa si pasa todas las validaciones.</returns>
    /// <exception cref="ArgumentException">Se lanza si la identificación no cumple el formato.</exception>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el cliente no existe o no está activo.
    /// </exception>
    public Cliente? ValidarClienteActivo(string identificacion)
    {
        // Validación de formato de identificación antes de consultar la BD
        ValidadorComun.ValidarIdentificacion(identificacion);
        var cliente = _accesoDatos.ConsultarPorIdentificacion(identificacion.Trim());
        if (cliente == null)
        {
            throw new InvalidOperationException("El cliente no está registrado en el sistema.");
        }

        // Regla de negocio: solo clientes activos pueden realizar compras
        if (!cliente.Activo)
        {
            throw new InvalidOperationException("El cliente no se encuentra activo.");
        }

        return cliente;
    }

    /// <summary>
    /// Valida que un cliente exista y esté activo, buscándolo por IdCliente.
    /// </summary>
    /// <param name="idCliente">Identificador numérico interno del cliente.</param>
    /// <returns>Entidad Cliente activa si pasa todas las validaciones.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el cliente no existe o no está activo.
    /// </exception>
    public Cliente? ValidarClienteActivoPorId(int idCliente)
    {
        var cliente = _accesoDatos.ConsultarPorId(idCliente);
        if (cliente == null)
        {
            throw new InvalidOperationException("El cliente no está registrado en el sistema.");
        }

        if (!cliente.Activo)
        {
            throw new InvalidOperationException("El cliente no se encuentra activo.");
        }

        return cliente;
    }

    /// <summary>
    /// Obtiene la lista completa de clientes registrados.
    /// </summary>
    /// <returns>Colección de todos los clientes, activos e inactivos.</returns>
    public List<Cliente> ConsultarTodos()
    {
        return _accesoDatos.ConsultarTodos();
    }

    /// <summary>
    /// Obtiene únicamente los clientes con estado Activo = true.
    /// </summary>
    /// <returns>Colección de clientes habilitados para compras.</returns>
    public List<Cliente> ConsultarActivos()
    {
        return _accesoDatos.ConsultarActivos();
    }
}
