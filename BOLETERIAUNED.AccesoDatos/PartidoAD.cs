/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para la entidad Partido.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="Partido"/>.
/// Administra las operaciones SQL sobre la tabla Partido del estadio UNED.
/// </summary>
public class PartidoAD
{
    /// <summary>
    /// Inserta un nuevo partido deportivo en la base de datos.
    /// </summary>
    /// <param name="partido">Entidad Partido con rival, fecha, hora y estado activo.</param>
    public void Insertar(Partido partido)
    {
        // INSERT de un evento deportivo con todos sus atributos descriptivos
        const string sql = """
            INSERT INTO Partido (IdPartido, Rival, Fecha, Hora, Activo)
            VALUES (@IdPartido, @Rival, @Fecha, @Hora, @Activo)
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdPartido", partido.IdPartido);
        comando.Parameters.AddWithValue("@Rival", partido.Rival);
        comando.Parameters.AddWithValue("@Fecha", partido.Fecha.Date); // Solo la fecha, sin componente horario
        comando.Parameters.AddWithValue("@Hora", partido.Hora); // Hora almacenada como cadena en formato HH:mm
        comando.Parameters.AddWithValue("@Activo", partido.Activo);
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Verifica si ya existe un partido con el IdPartido indicado.
    /// </summary>
    /// <param name="idPartido">Identificador numérico del partido.</param>
    /// <returns><c>true</c> si el IdPartido ya está registrado; <c>false</c> si está disponible.</returns>
    public bool ExisteId(int idPartido)
    {
        const string sql = "SELECT COUNT(1) FROM Partido WHERE IdPartido = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idPartido);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Obtiene todos los partidos registrados, sin filtrar por estado ni fecha.
    /// </summary>
    /// <returns>Lista de partidos ordenados por IdPartido ascendente.</returns>
    public List<Partido> ConsultarTodos()
    {
        const string sql = "SELECT IdPartido, Rival, Fecha, Hora, Activo FROM Partido ORDER BY IdPartido";
        var lista = new List<Partido>();

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(MapearPartido(lector));
        }

        return lista;
    }

    /// <summary>
    /// Obtiene los partidos activos cuya fecha es hoy o posterior al día actual del servidor SQL.
    /// </summary>
    /// <returns>Lista de partidos vigentes para venta, ordenados por fecha y hora.</returns>
    public List<Partido> ConsultarActivos()
    {
        // Activo = 1: partido habilitado para ventas
        // CAST(GETDATE() AS DATE): compara solo la fecha del servidor, ignorando la hora del sistema
        const string sql = """
            SELECT IdPartido, Rival, Fecha, Hora, Activo
            FROM Partido
            WHERE Activo = 1 AND Fecha >= CAST(GETDATE() AS DATE)
            ORDER BY Fecha, Hora
            """;

        var lista = new List<Partido>();
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(MapearPartido(lector));
        }

        return lista;
    }

    /// <summary>
    /// Busca un partido específico por su identificador numérico.
    /// </summary>
    /// <param name="idPartido">Valor del campo IdPartido.</param>
    /// <returns>Entidad Partido si existe; <c>null</c> si no se encuentra.</returns>
    public Partido? ConsultarPorId(int idPartido)
    {
        const string sql = "SELECT IdPartido, Rival, Fecha, Hora, Activo FROM Partido WHERE IdPartido = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idPartido);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        return lector.Read() ? MapearPartido(lector) : null;
    }

    /// <summary>
    /// Convierte una fila del SqlDataReader en un objeto <see cref="Partido"/>.
    /// </summary>
    /// <param name="lector">Lector posicionado en la fila actual.</param>
    /// <returns>Instancia de Partido con los cinco campos de la tabla.</returns>
    private static Partido MapearPartido(SqlDataReader lector)
    {
        // Columnas: 0=IdPartido, 1=Rival, 2=Fecha, 3=Hora, 4=Activo
        return new Partido(
            lector.GetInt32(0),
            lector.GetString(1),
            lector.GetDateTime(2),
            lector.GetString(3),
            lector.GetBoolean(4));
    }
}
