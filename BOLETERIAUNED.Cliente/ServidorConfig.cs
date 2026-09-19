/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Constantes de configuración de red TCP compartidas por el cliente.
 *              Centraliza IP y puerto para facilitar cambios sin modificar la lógica de conexión.
 */

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Contiene los parámetros de conexión TCP hacia el servidor de boletaría.
/// Valores por defecto usados por <see cref="ClienteRed.Conectar"/>.
/// </summary>
public static class ServidorConfig
{
    /// <summary>
    /// Dirección IPv4 del servidor. 127.0.0.1 (localhost) para pruebas en la misma máquina.
    /// </summary>
    public const string DireccionIp = "127.0.0.1";

    /// <summary>
    /// Puerto TCP donde el servidor escucha solicitudes del protocolo de boletaría (JSON por línea).
    /// </summary>
    public const int Puerto = 14500;
}
