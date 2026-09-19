/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Capa de comunicación TCP del cliente. Serializa solicitudes JSON y lee respuestas del servidor.
 */

using System.Net.Sockets;
using System.Text;
using BOLETERIAUNED.Entidades;

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Encapsula la conexión TCP, el flujo de red y el intercambio de mensajes con el servidor.
/// Protocolo: una línea JSON por solicitud y una línea JSON por respuesta (UTF-8).
/// </summary>
public class ClienteRed
{
    /// <summary>Cliente TCP subyacente; null cuando no hay sesión activa.</summary>
    private TcpClient? _cliente;

    /// <summary>Flujo de bytes bidireccional sobre el socket TCP.</summary>
    private NetworkStream? _stream;

    /// <summary>Lector de texto para recibir respuestas del servidor línea a línea.</summary>
    private StreamReader? _lector;

    /// <summary>Escritor de texto para enviar solicitudes serializadas al servidor.</summary>
    private StreamWriter? _escritor;

    /// <summary>
    /// Indica si existe una conexión TCP activa con el servidor.
    /// </summary>
    public bool EstaConectado => _cliente?.Connected == true;

    /// <summary>
    /// Establece una conexión TCP con el servidor de boletaría.
    /// </summary>
    /// <param name="ip">Dirección IP del servidor; por defecto <see cref="ServidorConfig.DireccionIp"/>.</param>
    /// <param name="puerto">Puerto TCP; por defecto <see cref="ServidorConfig.Puerto"/>.</param>
    /// <remarks>
    /// Cierra cualquier conexión previa antes de abrir una nueva. Configura lectura/escritura UTF-8 con AutoFlush.
    /// </remarks>
    public void Conectar(string ip = ServidorConfig.DireccionIp, int puerto = ServidorConfig.Puerto)
    {
        // Libera recursos de una conexión anterior si el usuario reconecta.
        Desconectar();

        // Crea el socket TCP y realiza el handshake de conexión (SYN) hacia ip:puerto.
        _cliente = new TcpClient();
        _cliente.Connect(ip, puerto);

        // Obtiene el flujo de red y prepara lectores/escritores para el protocolo basado en líneas.
        _stream = _cliente.GetStream();
        _lector = new StreamReader(_stream, Encoding.UTF8);
        _escritor = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
    }

    /// <summary>
    /// Cierra la conexión TCP y libera lectores, escritores y el cliente.
    /// </summary>
    public void Desconectar()
    {
        _lector?.Dispose();
        _escritor?.Dispose();
        _stream?.Dispose();
        _cliente?.Close();
        _lector = null;
        _escritor = null;
        _stream = null;
        _cliente = null;
    }

    /// <summary>
    /// Envía una solicitud al servidor y espera la respuesta correspondiente en la misma conexión.
    /// </summary>
    /// <param name="solicitud">Mensaje con comando y parámetros (autenticación, compra, consulta, etc.).</param>
    /// <returns>Respuesta deserializada del servidor con indicador de éxito, mensaje y datos JSON opcionales.</returns>
    /// <exception cref="InvalidOperationException">Si no hay conexión, el servidor no responde o la respuesta es inválida.</exception>
    public MensajeRespuesta EnviarSolicitud(MensajeSolicitud solicitud)
    {
        // Verifica que la sesión TCP esté inicializada antes de escribir en el socket.
        if (_escritor == null || _lector == null)
        {
            throw new InvalidOperationException("No hay conexión activa con el servidor.");
        }

        try
        {
            // Serializa la solicitud a JSON y la envía como una única línea terminada en salto de línea.
            _escritor.WriteLine(solicitud.Serializar());

            // Bloquea hasta recibir la línea de respuesta del servidor (modelo request-response síncrono).
            var respuestaJson = _lector.ReadLine();
            if (string.IsNullOrWhiteSpace(respuestaJson))
            {
                throw new InvalidOperationException("El servidor no respondió.");
            }

            // Convierte el JSON recibido en un objeto MensajeRespuesta tipado.
            return MensajeRespuesta.Deserializar(respuestaJson)
                ?? throw new InvalidOperationException("Respuesta inválida del servidor.");
        }
        catch (IOException ex)
        {
            // Envuelve errores de red (conexión cerrada, timeout implícito, etc.) en una excepción de operación.
            throw new InvalidOperationException($"Error de comunicación: {ex.Message}", ex);
        }
    }
}
