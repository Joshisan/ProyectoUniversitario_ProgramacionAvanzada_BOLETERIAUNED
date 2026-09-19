/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Servidor TCP multihilo para atender clientes en 127.0.0.1:14500.
 */

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Implementa un servidor TCP multihilo que escucha conexiones de clientes remotos
/// (aplicación cliente BOLETERIAUNED) y procesa solicitudes JSON línea por línea.
/// </summary>
/// <remarks>
/// Arquitectura de hilos:
/// <list type="bullet">
/// <item><description>Hilo principal (UI): suscrito a <see cref="EventoBitacora"/> y <see cref="DatosActualizados"/>.</description></item>
/// <item><description>Hilo de escucha (<c>HiloEscuchaTCP</c>): acepta conexiones entrantes de forma bloqueante.</description></item>
/// <item><description>Hilos de cliente (uno por conexión): leen/escriben el stream TCP y delegan en <see cref="ManejadorClienteTCP"/>.</description></item>
/// </list>
/// </remarks>
public class ServidorTCP
{
    /// <summary>Dirección IPv4 de escucha del servidor (localhost para desarrollo y pruebas).</summary>
    public const string DireccionIp = "127.0.0.1";

    /// <summary>Puerto TCP reservado para el protocolo de la boletaría UNED.</summary>
    public const int Puerto = 14500;

    /// <summary>Instancia del listener TCP; queda nulo hasta llamar a <see cref="Iniciar"/>.</summary>
    private TcpListener? _listener;

    /// <summary>
    /// Bandera volátil que indica si el servidor está aceptando conexiones.
    /// <c>volatile</c> garantiza visibilidad entre hilos de escucha y de UI al detener el servicio.
    /// </summary>
    private volatile bool _activo;

    /// <summary>Referencia al hilo dedicado al bucle <see cref="EscucharClientes"/>.</summary>
    private Thread? _hiloEscucha;

    /// <summary>
    /// Evento disparado cada vez que se registra un mensaje en la bitácora del servidor.
    /// Los suscriptores (por ejemplo <see cref="Form1"/>) actualizan la UI con el texto recibido.
    /// </summary>
    public event Action<string>? EventoBitacora;

    /// <summary>
    /// Evento disparado cuando una operación TCP modifica datos persistidos
    /// (por ejemplo, una venta en línea) para refrescar las grillas de consulta en el formulario.
    /// </summary>
    public event Action? DatosActualizados;

    /// <summary>Indica si el servidor TCP está actualmente en ejecución.</summary>
    public bool EstaActivo => _activo;

    /// <summary>
    /// Arranca el listener TCP y el hilo de escucha en segundo plano.
    /// Si ya está activo, la llamada se ignora de forma idempotente.
    /// </summary>
    public void Iniciar()
    {
        if (_activo)
        {
            return;
        }

        _listener = new TcpListener(IPAddress.Parse(DireccionIp), Puerto);
        _listener.Start();
        _activo = true;

        // Hilo de escucha: IsBackground=true evita que el proceso quede colgado al cerrar la UI.
        _hiloEscucha = new Thread(EscucharClientes)
        {
            IsBackground = true,
            Name = "HiloEscuchaTCP"
        };
        _hiloEscucha.Start();

        // Notifica en bitácora que el servicio quedó disponible para clientes.
        RegistrarBitacora($"Servidor TCP iniciado en {DireccionIp}:{Puerto}");
    }

    /// <summary>
    /// Detiene el listener y marca el servidor como inactivo.
    /// Las conexiones en curso pueden finalizar por excepción al cerrar el socket de escucha.
    /// </summary>
    public void Detener()
    {
        if (!_activo)
        {
            return;
        }

        _activo = false;
        try
        {
            // Stop() desbloquea AcceptTcpClient() en el hilo de escucha para permitir su salida.
            _listener?.Stop();
        }
        catch
        {
            // Ignorar errores al cerrar el listener (socket ya cerrado o estado inconsistente).
        }

        RegistrarBitacora("Servidor TCP detenido.");
    }

    /// <summary>
    /// Bucle ejecutado en el hilo <c>HiloEscuchaTCP</c>.
    /// Acepta conexiones entrantes y crea un hilo independiente por cada cliente conectado.
    /// </summary>
    private void EscucharClientes()
    {
        while (_activo)
        {
            try
            {
                if (_listener == null)
                {
                    break;
                }

                // AcceptTcpClient bloquea hasta que un cliente se conecta o el listener se detiene.
                var clienteTcp = _listener.AcceptTcpClient();

                // Cada cliente se atiende en su propio hilo para no bloquear nuevas conexiones.
                var hiloCliente = new Thread(() => AtenderCliente(clienteTcp))
                {
                    IsBackground = true
                };
                hiloCliente.Start();
            }
            catch (SocketException) when (!_activo)
            {
                // Salida esperada al detener el listener mientras AcceptTcpClient estaba bloqueado.
                break;
            }
            catch (Exception ex)
            {
                RegistrarBitacora($"Error en escucha TCP: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Atiende una sesión TCP completa: lectura de líneas JSON, procesamiento y respuesta.
    /// Se ejecuta en un hilo de trabajo distinto al hilo de UI.
    /// </summary>
    /// <param name="clienteTcp">Cliente TCP ya aceptado por el listener.</param>
    private void AtenderCliente(TcpClient clienteTcp)
    {
        var endpoint = clienteTcp.Client.RemoteEndPoint?.ToString() ?? "Cliente";
        RegistrarBitacora($"Conexión entrante: {endpoint}");

        try
        {
            // using garantiza cierre de socket y liberación de streams al finalizar la sesión.
            using (clienteTcp)
            using (var stream = clienteTcp.GetStream())
            using (var lector = new StreamReader(stream, Encoding.UTF8))
            using (var escritor = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                // Cada conexión tiene su propio manejador con estado de sesión (cliente autenticado).
                var manejador = new ManejadorClienteTCP(RegistrarBitacora, NotificarDatosActualizados);
                string? linea;

                // Protocolo: una solicitud JSON por línea; la respuesta también termina en newline.
                while ((linea = lector.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea))
                    {
                        continue;
                    }

                    var respuesta = manejador.ProcesarSolicitud(linea);
                    escritor.WriteLine(respuesta);
                }
            }
        }
        catch (Exception ex)
        {
            RegistrarBitacora($"Error con {endpoint}: {ex.Message}");
        }
        finally
        {
            RegistrarBitacora($"Desconexión: {endpoint}");
        }
    }

    /// <summary>
    /// Propaga un mensaje de bitácora con marca de tiempo a los suscriptores del evento.
    /// Puede invocarse desde hilos TCP; la UI debe usar Invoke/BeginInvoke al actualizarse.
    /// </summary>
    /// <param name="mensaje">Texto descriptivo del evento ocurrido en el servidor.</param>
    private void RegistrarBitacora(string mensaje)
    {
        // Formato [HH:mm:ss] para orden cronológico legible en el ListBox del formulario.
        EventoBitacora?.Invoke($"[{DateTime.Now:HH:mm:ss}] {mensaje}");
    }

    /// <summary>
    /// Notifica a la UI que debe refrescar consultas tras cambios en la base de datos vía TCP.
    /// </summary>
    private void NotificarDatosActualizados()
    {
        DatosActualizados?.Invoke();
    }
}
