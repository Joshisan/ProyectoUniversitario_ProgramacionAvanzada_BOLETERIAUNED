/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para la entidad Cliente.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="Cliente"/>.
/// Encapsula todas las operaciones SQL contra la tabla Cliente de la base de datos.
/// </summary>
public class ClienteAD
{
    /// <summary>
    /// Inserta un nuevo registro de cliente en la tabla Cliente.
    /// </summary>
    /// <param name="cliente">Entidad Cliente con todos los datos a persistir.</param>
    public void Insertar(Cliente cliente)
    {
        // INSERT parametrizado: evita inyección SQL y mapea cada columna con su parámetro @
        const string sql = """
            INSERT INTO Cliente (IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo)
            VALUES (@IdCliente, @Identificacion, @Nombre, @Apellido, @FechaNacimiento, @FechaRegistro, @Activo)
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        // AddWithValue asigna el valor del objeto Cliente a cada parámetro del comando SQL
        comando.Parameters.AddWithValue("@IdCliente", cliente.Id);
        comando.Parameters.AddWithValue("@Identificacion", cliente.Identificacion);
        comando.Parameters.AddWithValue("@Nombre", cliente.Nombre);
        comando.Parameters.AddWithValue("@Apellido", cliente.Apellido);
        comando.Parameters.AddWithValue("@FechaNacimiento", cliente.FechaNacimiento.Date); // .Date elimina la parte horaria
        comando.Parameters.AddWithValue("@FechaRegistro", cliente.FechaRegistro.Date);
        comando.Parameters.AddWithValue("@Activo", cliente.Activo);
        conexion.Open(); // Abre la conexión antes de ejecutar el comando
        comando.ExecuteNonQuery(); // INSERT no retorna filas; ExecuteNonQuery devuelve filas afectadas
    }

    /// <summary>
    /// Verifica si ya existe un cliente con el identificador numérico indicado.
    /// </summary>
    /// <param name="idCliente">Valor del campo IdCliente a buscar.</param>
    /// <returns><c>true</c> si existe al menos un registro con ese IdCliente; <c>false</c> en caso contrario.</returns>
    public bool ExisteId(int idCliente)
    {
        // COUNT(1) retorna 0 si no hay coincidencias o 1 si existe; más eficiente que SELECT *
        const string sql = "SELECT COUNT(1) FROM Cliente WHERE IdCliente = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idCliente);
        conexion.Open();
        // ExecuteScalar retorna la primera columna de la primera fila (el conteo)
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Verifica si ya existe un cliente con la identificación (cédula u otro documento) indicada.
    /// </summary>
    /// <param name="identificacion">Número de identificación del cliente.</param>
    /// <returns><c>true</c> si la identificación ya está registrada; <c>false</c> si está disponible.</returns>
    public bool ExisteIdentificacion(string identificacion)
    {
        // Validación de unicidad a nivel de base de datos antes del INSERT
        const string sql = "SELECT COUNT(1) FROM Cliente WHERE Identificacion = @Identificacion";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Identificacion", identificacion);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Busca un cliente por su número de identificación.
    /// </summary>
    /// <param name="identificacion">Identificación exacta a consultar.</param>
    /// <returns>Instancia de <see cref="Cliente"/> si se encuentra; <c>null</c> si no existe.</returns>
    public Cliente? ConsultarPorIdentificacion(string identificacion)
    {
        // SELECT de todas las columnas necesarias para reconstruir la entidad Cliente
        const string sql = """
            SELECT IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo
            FROM Cliente WHERE Identificacion = @Identificacion
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Identificacion", identificacion);
        conexion.Open();
        using var lector = comando.ExecuteReader(); // ExecuteReader para consultas que retornan filas
        // lector.Read() avanza al primer registro; retorna false si no hay resultados
        return lector.Read() ? MapearCliente(lector) : null;
    }

    /// <summary>
    /// Busca un cliente por su identificador numérico interno.
    /// </summary>
    /// <param name="idCliente">Valor del campo IdCliente.</param>
    /// <returns>Instancia de <see cref="Cliente"/> si se encuentra; <c>null</c> si no existe.</returns>
    public Cliente? ConsultarPorId(int idCliente)
    {
        const string sql = """
            SELECT IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo
            FROM Cliente WHERE IdCliente = @Id
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idCliente);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        return lector.Read() ? MapearCliente(lector) : null;
    }

    /// <summary>
    /// Obtiene la lista completa de clientes ordenados por IdCliente ascendente.
    /// </summary>
    /// <returns>Colección con todos los clientes registrados en el sistema.</returns>
    public List<Cliente> ConsultarTodos()
    {
        const string sql = """
            SELECT IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo
            FROM Cliente ORDER BY IdCliente
            """;

        return EjecutarConsulta(sql);
    }

    /// <summary>
    /// Obtiene únicamente los clientes cuyo estado Activo es verdadero (Activo = 1).
    /// </summary>
    /// <returns>Colección de clientes activos, ordenados por IdCliente.</returns>
    public List<Cliente> ConsultarActivos()
    {
        // Filtro WHERE Activo = 1: en SQL Server, bit verdadero se almacena como 1
        const string sql = """
            SELECT IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo
            FROM Cliente WHERE Activo = 1 ORDER BY IdCliente
            """;

        return EjecutarConsulta(sql);
    }

    /// <summary>
    /// Ejecuta una consulta SELECT genérica y construye la lista de clientes resultante.
    /// </summary>
    /// <param name="sql">Sentencia SQL SELECT ya definida por el método llamador.</param>
    /// <returns>Lista de objetos Cliente mapeados desde el SqlDataReader.</returns>
    private static List<Cliente> EjecutarConsulta(string sql)
    {
        var lista = new List<Cliente>();
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        // Itera fila por fila hasta agotar el resultado del SELECT
        while (lector.Read())
        {
            lista.Add(MapearCliente(lector));
        }

        return lista;
    }

    /// <summary>
    /// Convierte una fila del SqlDataReader en un objeto <see cref="Cliente"/>.
    /// </summary>
    /// <param name="lector">Lector posicionado en la fila actual del resultado SQL.</param>
    /// <returns>Instancia de Cliente con los valores de las columnas 0 a 6.</returns>
    private static Cliente MapearCliente(SqlDataReader lector)
    {
        // Índices: 0=IdCliente, 1=Identificacion, 2=Nombre, 3=Apellido, 4=FechaNacimiento, 5=FechaRegistro, 6=Activo
        return new Cliente(
            lector.GetInt32(0),
            lector.GetString(1),
            lector.GetString(2),
            lector.GetString(3),
            lector.GetDateTime(4),
            lector.GetDateTime(5),
            lector.GetBoolean(6));
    }
}
