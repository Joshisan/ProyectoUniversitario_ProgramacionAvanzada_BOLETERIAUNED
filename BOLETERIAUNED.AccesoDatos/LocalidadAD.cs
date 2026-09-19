/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para la entidad Localidad.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="Localidad"/>.
/// Gestiona las secciones del estadio (Palco, Preferencia, etc.) y su precio base.
/// </summary>
public class LocalidadAD
{
    /// <summary>
    /// Inserta una nueva localidad (sección del estadio) en la base de datos.
    /// </summary>
    /// <param name="localidad">Entidad Localidad con id, nombre y precio unitario.</param>
    public void Insertar(Localidad localidad)
    {
        // INSERT de una sección del estadio con su precio base por entrada
        const string sql = """
            INSERT INTO Localidad (IdLocalidad, NombreLocalidad, Precio)
            VALUES (@IdLocalidad, @NombreLocalidad, @Precio)
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdLocalidad", localidad.IdLocalidad);
        comando.Parameters.AddWithValue("@NombreLocalidad", localidad.NombreLocalidad);
        comando.Parameters.AddWithValue("@Precio", localidad.Precio); // decimal: precio unitario de la entrada
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Verifica si ya existe una localidad con el IdLocalidad indicado.
    /// </summary>
    /// <param name="idLocalidad">Identificador numérico de la localidad.</param>
    /// <returns><c>true</c> si el IdLocalidad ya está registrado; <c>false</c> si está disponible.</returns>
    public bool ExisteId(int idLocalidad)
    {
        const string sql = "SELECT COUNT(1) FROM Localidad WHERE IdLocalidad = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idLocalidad);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Obtiene todas las localidades registradas en el sistema.
    /// </summary>
    /// <returns>Lista de localidades ordenadas por IdLocalidad.</returns>
    public List<Localidad> ConsultarTodos()
    {
        const string sql = "SELECT IdLocalidad, NombreLocalidad, Precio FROM Localidad ORDER BY IdLocalidad";
        var lista = new List<Localidad>();

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            // Mapeo inline: construye Localidad directamente desde las tres columnas
            lista.Add(new Localidad(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetDecimal(2)));
        }

        return lista;
    }

    /// <summary>
    /// Busca una localidad por su identificador numérico.
    /// </summary>
    /// <param name="idLocalidad">Valor del campo IdLocalidad.</param>
    /// <returns>Entidad Localidad si existe; <c>null</c> si no se encuentra.</returns>
    public Localidad? ConsultarPorId(int idLocalidad)
    {
        const string sql = "SELECT IdLocalidad, NombreLocalidad, Precio FROM Localidad WHERE IdLocalidad = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idLocalidad);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        if (!lector.Read())
        {
            return null; // No hay fila que coincida con el IdLocalidad buscado
        }

        return new Localidad(lector.GetInt32(0), lector.GetString(1), lector.GetDecimal(2));
    }
}
