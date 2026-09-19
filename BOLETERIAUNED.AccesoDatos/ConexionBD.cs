/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Gestión de la cadena de conexión desde App.config.
 */

using System.Configuration;
using Microsoft.Data.SqlClient;

namespace BOLETERIAUNED.AccesoDatos;

/// <summary>
/// Clase utilitaria estática que centraliza la obtención de la cadena de conexión
/// y la creación de instancias de <see cref="SqlConnection"/> para SQL Server.
/// Implementa un patrón de caché en memoria para evitar leer App.config repetidamente.
/// </summary>
public static class ConexionBD
{
    /// <summary>
    /// Almacena la cadena de conexión leída desde App.config la primera vez que se solicita.
    /// Permite reutilizar el valor sin volver a consultar ConfigurationManager en cada operación.
    /// </summary>
    private static string? _cadenaConexion;

    /// <summary>
    /// Obtiene la cadena de conexión configurada en App.config bajo el nombre "BoletariaUNED".
    /// </summary>
    /// <returns>Cadena de conexión completa hacia la base de datos BoletariaUNED.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando no existe la entrada "BoletariaUNED" en la sección connectionStrings de App.config.
    /// </exception>
    public static string ObtenerCadenaConexion()
    {
        // Solo se lee App.config la primera vez; las siguientes llamadas reutilizan el valor en caché
        if (string.IsNullOrWhiteSpace(_cadenaConexion))
        {
            // ConfigurationManager busca la clave "BoletariaUNED" en <connectionStrings> del archivo de configuración
            _cadenaConexion = ConfigurationManager.ConnectionStrings["BoletariaUNED"]?.ConnectionString
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'BoletariaUNED' en App.config.");
        }

        return _cadenaConexion;
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="SqlConnection"/> lista para abrirse.
    /// El llamador es responsable de abrir, usar y cerrar la conexión (patrón using).
    /// </summary>
    /// <returns>Nueva conexión SQL Server sin abrir, configurada con la cadena del proyecto.</returns>
    public static SqlConnection CrearConexion()
    {
        // SqlConnection no abre la conexión automáticamente; debe invocarse conexion.Open() antes de ejecutar comandos
        return new SqlConnection(ObtenerCadenaConexion());
    }
}
