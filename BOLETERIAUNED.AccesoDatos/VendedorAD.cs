/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para la entidad Vendedor.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="Vendedor"/>.
/// Gestiona las operaciones CRUD de consulta e inserción sobre la tabla Vendedor.
/// </summary>
public class VendedorAD
{
    /// <summary>
    /// Inserta un nuevo vendedor en la base de datos.
    /// </summary>
    /// <param name="vendedor">Entidad Vendedor con los datos a registrar.</param>
    public void Insertar(Vendedor vendedor)
    {
        // INSERT en tabla Vendedor con parámetros para cada columna obligatoria
        const string sql = """
            INSERT INTO Vendedor (IdVendedor, Identificacion, Nombre, Apellido, FechaNacimiento, FechaIngreso)
            VALUES (@IdVendedor, @Identificacion, @Nombre, @Apellido, @FechaNacimiento, @FechaIngreso)
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdVendedor", vendedor.Id);
        comando.Parameters.AddWithValue("@Identificacion", vendedor.Identificacion);
        comando.Parameters.AddWithValue("@Nombre", vendedor.Nombre);
        comando.Parameters.AddWithValue("@Apellido", vendedor.Apellido);
        comando.Parameters.AddWithValue("@FechaNacimiento", vendedor.FechaNacimiento.Date);
        comando.Parameters.AddWithValue("@FechaIngreso", vendedor.FechaIngreso.Date); // Fecha de ingreso laboral al sistema
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Verifica si ya existe un vendedor con el IdVendedor indicado.
    /// </summary>
    /// <param name="idVendedor">Identificador numérico del vendedor.</param>
    /// <returns><c>true</c> si el IdVendedor ya está registrado; <c>false</c> si está disponible.</returns>
    public bool ExisteId(int idVendedor)
    {
        const string sql = "SELECT COUNT(1) FROM Vendedor WHERE IdVendedor = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idVendedor);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Verifica si ya existe un vendedor con la identificación indicada.
    /// </summary>
    /// <param name="identificacion">Número de identificación del vendedor.</param>
    /// <returns><c>true</c> si la identificación ya está en uso; <c>false</c> en caso contrario.</returns>
    public bool ExisteIdentificacion(string identificacion)
    {
        // Garantiza unicidad de Identificacion antes de permitir el registro
        const string sql = "SELECT COUNT(1) FROM Vendedor WHERE Identificacion = @Identificacion";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Identificacion", identificacion);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Obtiene todos los vendedores registrados, ordenados por IdVendedor.
    /// </summary>
    /// <returns>Lista completa de entidades Vendedor.</returns>
    public List<Vendedor> ConsultarTodos()
    {
        const string sql = """
            SELECT IdVendedor, Identificacion, Nombre, Apellido, FechaNacimiento, FechaIngreso
            FROM Vendedor ORDER BY IdVendedor
            """;

        var lista = new List<Vendedor>();
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(MapearVendedor(lector));
        }

        return lista;
    }

    /// <summary>
    /// Busca un vendedor por su identificador numérico.
    /// </summary>
    /// <param name="idVendedor">Valor del campo IdVendedor.</param>
    /// <returns>Entidad Vendedor si existe; <c>null</c> si no se encuentra.</returns>
    public Vendedor? ConsultarPorId(int idVendedor)
    {
        const string sql = """
            SELECT IdVendedor, Identificacion, Nombre, Apellido, FechaNacimiento, FechaIngreso
            FROM Vendedor WHERE IdVendedor = @Id
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idVendedor);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        return lector.Read() ? MapearVendedor(lector) : null;
    }

    /// <summary>
    /// Transforma una fila del resultado SQL en un objeto <see cref="Vendedor"/>.
    /// </summary>
    /// <param name="lector">SqlDataReader posicionado en la fila a mapear.</param>
    /// <returns>Instancia de Vendedor con los seis campos de la tabla.</returns>
    private static Vendedor MapearVendedor(SqlDataReader lector)
    {
        // Columnas: 0=IdVendedor, 1=Identificacion, 2=Nombre, 3=Apellido, 4=FechaNacimiento, 5=FechaIngreso
        return new Vendedor(
            lector.GetInt32(0),
            lector.GetString(1),
            lector.GetString(2),
            lector.GetString(3),
            lector.GetDateTime(4),
            lector.GetDateTime(5));
    }
}
