/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Acceso a datos para LocalidadPorPartido.
 */

using BOLETERIAUNED.Entidades;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Capa de acceso a datos (AD) para la entidad <see cref="LocalidadPorPartido"/>.
/// Administra la tabla puente que asocia localidades a partidos con cantidad de entradas disponibles.
/// </summary>
public class LocalidadPorPartidoAD
{
    /// <summary>
    /// Inserta una nueva asociación entre un partido y una localidad con su cupo de entradas.
    /// </summary>
    /// <param name="registro">Entidad LocalidadPorPartido con partido, localidad y cantidad disponible.</param>
    public void Insertar(LocalidadPorPartido registro)
    {
        // INSERT en tabla de relación N:M entre Partido y Localidad, con inventario de entradas
        const string sql = """
            INSERT INTO LocalidadPorPartido (IdLocalidadPartido, IdPartido, IdLocalidad, CantidadDisponible)
            VALUES (@IdLocalidadPartido, @IdPartido, @IdLocalidad, @CantidadDisponible)
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdLocalidadPartido", registro.IdLocalidadPartido);
        comando.Parameters.AddWithValue("@IdPartido", registro.Partido.IdPartido); // FK hacia tabla Partido
        comando.Parameters.AddWithValue("@IdLocalidad", registro.Localidad.IdLocalidad); // FK hacia tabla Localidad
        comando.Parameters.AddWithValue("@CantidadDisponible", registro.CantidadDisponible);
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Verifica si ya existe un registro con el IdLocalidadPartido indicado.
    /// </summary>
    /// <param name="idLocalidadPartido">Identificador de la asociación partido-localidad.</param>
    /// <returns><c>true</c> si el IdLocalidadPartido ya existe; <c>false</c> en caso contrario.</returns>
    public bool ExisteId(int idLocalidadPartido)
    {
        const string sql = "SELECT COUNT(1) FROM LocalidadPorPartido WHERE IdLocalidadPartido = @Id";
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@Id", idLocalidadPartido);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Verifica si ya existe la combinación partido + localidad (unicidad de la asociación).
    /// </summary>
    /// <param name="idPartido">Identificador del partido.</param>
    /// <param name="idLocalidad">Identificador de la localidad.</param>
    /// <returns><c>true</c> si la pareja partido/localidad ya está registrada; <c>false</c> si no.</returns>
    public bool ExistePartidoLocalidad(int idPartido, int idLocalidad)
    {
        // Evita duplicar la misma localidad para un mismo partido
        const string sql = """
            SELECT COUNT(1) FROM LocalidadPorPartido
            WHERE IdPartido = @IdPartido AND IdLocalidad = @IdLocalidad
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdPartido", idPartido);
        comando.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
        conexion.Open();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Consulta la asociación completa (con datos de partido y localidad) por clave compuesta.
    /// </summary>
    /// <param name="idPartido">Identificador del partido.</param>
    /// <param name="idLocalidad">Identificador de la localidad.</param>
    /// <returns>Entidad LocalidadPorPartido enriquecida; <c>null</c> si no existe la asociación.</returns>
    public LocalidadPorPartido? ConsultarPorPartidoLocalidad(int idPartido, int idLocalidad)
    {
        // INNER JOIN: trae datos del partido y la localidad en una sola consulta
        const string sql = """
            SELECT lp.IdLocalidadPartido, lp.IdPartido, p.Rival, p.Fecha, p.Hora, p.Activo,
                   lp.IdLocalidad, l.NombreLocalidad, l.Precio, lp.CantidadDisponible
            FROM LocalidadPorPartido lp
            INNER JOIN Partido p ON lp.IdPartido = p.IdPartido
            INNER JOIN Localidad l ON lp.IdLocalidad = l.IdLocalidad
            WHERE lp.IdPartido = @IdPartido AND lp.IdLocalidad = @IdLocalidad
            """;

        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdPartido", idPartido);
        comando.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        return lector.Read() ? MapearCompleto(lector) : null;
    }

    /// <summary>
    /// Obtiene las localidades disponibles (con entradas > 0) para un partido específico.
    /// </summary>
    /// <param name="idPartido">Identificador del partido a consultar.</param>
    /// <returns>Lista de asociaciones con stock disponible, ordenadas por nombre de localidad.</returns>
    public List<LocalidadPorPartido> ConsultarPorPartido(int idPartido)
    {
        // CantidadDisponible > 0: solo localidades con entradas a la venta
        const string sql = """
            SELECT lp.IdLocalidadPartido, lp.IdPartido, p.Rival, p.Fecha, p.Hora, p.Activo,
                   lp.IdLocalidad, l.NombreLocalidad, l.Precio, lp.CantidadDisponible
            FROM LocalidadPorPartido lp
            INNER JOIN Partido p ON lp.IdPartido = p.IdPartido
            INNER JOIN Localidad l ON lp.IdLocalidad = l.IdLocalidad
            WHERE lp.IdPartido = @IdPartido AND lp.CantidadDisponible > 0
            ORDER BY l.NombreLocalidad
            """;

        var lista = new List<LocalidadPorPartido>();
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@IdPartido", idPartido);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(MapearCompleto(lector));
        }

        return lista;
    }

    /// <summary>
    /// Obtiene el detalle completo de todas las asociaciones partido-localidad para reportes.
    /// </summary>
    /// <returns>Lista de objetos <see cref="ConsultaLocalidadPorPartido"/> con información descriptiva.</returns>
    public List<ConsultaLocalidadPorPartido> ConsultarTodosDetalle()
    {
        // Consulta sin filtro: incluye todas las asociaciones con datos enriquecidos de JOIN
        const string sql = """
            SELECT lp.IdLocalidadPartido, lp.IdPartido, p.Rival, p.Fecha, p.Hora, p.Activo,
                   lp.IdLocalidad, l.NombreLocalidad, l.Precio, lp.CantidadDisponible
            FROM LocalidadPorPartido lp
            INNER JOIN Partido p ON lp.IdPartido = p.IdPartido
            INNER JOIN Localidad l ON lp.IdLocalidad = l.IdLocalidad
            ORDER BY lp.IdLocalidadPartido
            """;

        var lista = new List<ConsultaLocalidadPorPartido>();
        using var conexion = ConexionBD.CrearConexion();
        using var comando = new SqlCommand(sql, conexion);
        conexion.Open();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            lista.Add(MapearConsulta(lector));
        }

        return lista;
    }

    /// <summary>
    /// Descuenta entradas del inventario dentro de una transacción SQL existente.
    /// Este método participa en la venta atómica junto con VentaAD.Insertar.
    /// </summary>
    /// <param name="conexion">Conexión SQL abierta compartida con la transacción de venta.</param>
    /// <param name="transaccion">Transacción activa que agrupa la actualización y el INSERT de venta.</param>
    /// <param name="idPartido">Partido del cual se descuentan entradas.</param>
    /// <param name="idLocalidad">Localidad específica cuyo stock se reduce.</param>
    /// <param name="cantidadVendida">Número de entradas a restar del inventario.</param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando no hay stock suficiente (0 filas afectadas por el UPDATE condicional).
    /// </exception>
    public void ActualizarDisponibilidad(SqlConnection conexion, SqlTransaction transaccion,
        int idPartido, int idLocalidad, int cantidadVendida)
    {
        // UPDATE atómico: solo resta si CantidadDisponible >= cantidad solicitada
        // La condición en WHERE actúa como bloqueo optimista contra sobreventa concurrente
        const string sql = """
            UPDATE LocalidadPorPartido
            SET CantidadDisponible = CantidadDisponible - @Cantidad
            WHERE IdPartido = @IdPartido AND IdLocalidad = @IdLocalidad
              AND CantidadDisponible >= @Cantidad
            """;

        // El comando se vincula a la transacción existente; no crea conexión propia
        using var comando = new SqlCommand(sql, conexion, transaccion);
        comando.Parameters.AddWithValue("@Cantidad", cantidadVendida);
        comando.Parameters.AddWithValue("@IdPartido", idPartido);
        comando.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
        var filas = comando.ExecuteNonQuery();
        // Si filas == 0, otro proceso consumió el stock o no hay suficientes entradas
        if (filas == 0)
        {
            throw new InvalidOperationException("No hay entradas suficientes disponibles.");
        }
    }

    /// <summary>
    /// Mapea una fila del JOIN (LocalidadPorPartido + Partido + Localidad) a la entidad compuesta.
    /// </summary>
    /// <param name="lector">SqlDataReader con 10 columnas del SELECT enriquecido.</param>
    /// <returns>Instancia de LocalidadPorPartido con objetos Partido y Localidad anidados.</returns>
    private static LocalidadPorPartido MapearCompleto(SqlDataReader lector)
    {
        // Columnas 1-5: datos del Partido (IdPartido está en índice 1, no en 0)
        var partido = new Partido(
            lector.GetInt32(1),
            lector.GetString(2),
            lector.GetDateTime(3),
            lector.GetString(4),
            lector.GetBoolean(5));

        // Columnas 6-8: datos de la Localidad
        var localidad = new Localidad(
            lector.GetInt32(6),
            lector.GetString(7),
            lector.GetDecimal(8));

        // Columna 0: IdLocalidadPartido; columna 9: CantidadDisponible
        return new LocalidadPorPartido(
            lector.GetInt32(0),
            partido,
            localidad,
            lector.GetInt32(9));
    }

    /// <summary>
    /// Mapea una fila del JOIN a un DTO de consulta con campos descriptivos para la interfaz.
    /// </summary>
    /// <param name="lector">SqlDataReader posicionado en la fila a convertir.</param>
    /// <returns>Objeto ConsultaLocalidadPorPartido listo para mostrar en pantalla.</returns>
    private static ConsultaLocalidadPorPartido MapearConsulta(SqlDataReader lector)
    {
        return new ConsultaLocalidadPorPartido
        {
            IdLocalidadPartido = lector.GetInt32(0),
            IdPartido = lector.GetInt32(1),
            PartidoRival = lector.GetString(2),
            PartidoFecha = lector.GetDateTime(3),
            PartidoHora = lector.GetString(4),
            PartidoActivo = lector.GetBoolean(5) ? "Sí" : "No", // Conversión bool a texto legible
            IdLocalidad = lector.GetInt32(6),
            LocalidadNombre = lector.GetString(7),
            LocalidadPrecio = lector.GetDecimal(8),
            CantidadDisponible = lector.GetInt32(9)
        };
    }
}
