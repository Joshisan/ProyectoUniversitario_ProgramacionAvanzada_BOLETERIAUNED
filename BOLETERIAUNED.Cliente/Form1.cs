/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Formulario principal del cliente en línea. Gestiona conexión TCP, autenticación,
 *              compra de entradas y consulta del historial de compras del cliente autenticado.
 */

using System.Text.Json;
using BOLETERIAUNED.Entidades;
using ClienteEntidad = BOLETERIAUNED.Entidades.Cliente;

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Ventana principal del cliente de boletaría UNED.
/// Orquesta la UI con la capa de red para validar clientes, comprar entradas y consultar ventas.
/// </summary>
public partial class Form1 : Form
{
    /// <summary>Instancia única del cliente TCP usada en todos los eventos del formulario.</summary>
    private readonly ClienteRed _clienteRed = new();

    /// <summary>Cliente autenticado tras validación exitosa; null hasta que el usuario se identifique.</summary>
    private ClienteEntidad? _clienteAutenticado;

    /// <summary>
    /// Constructor: inicializa controles, configura la UI y muestra instrucciones al usuario.
    /// </summary>
    public Form1()
    {
        InitializeComponent();
        ConfigurarInterfazInicial();
        MostrarSolicitudIdentificacion();
    }

    /// <summary>
    /// Muestra un cuadro de diálogo inicial con el flujo esperado: identificación, conexión y validación.
    /// </summary>
    private void MostrarSolicitudIdentificacion()
    {
        MessageBox.Show(
            "Ingrese su identificación o Id de cliente, conéctese al servidor y presione Validar cliente.",
            "Boletaría UNED",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        txtIdentificacion.Focus();
    }

    /// <summary>
    /// Deja la interfaz en estado seguro: grid de solo lectura y funcionalidades de compra deshabilitadas.
    /// </summary>
    private void ConfigurarInterfazInicial()
    {
        // El grid muestra historial; el usuario no edita filas directamente.
        dgvCompras.ReadOnly = true;
        dgvCompras.AllowUserToAddRows = false;
        dgvCompras.AllowUserToDeleteRows = false;

        // Sin autenticación no se permiten compras ni consultas.
        HabilitarFuncionalidades(false);
    }

    /// <summary>
    /// Habilita o deshabilita controles de compra y consulta según el estado de autenticación.
    /// </summary>
    /// <param name="habilitado">True si el cliente ya fue validado contra el servidor.</param>
    private void HabilitarFuncionalidades(bool habilitado)
    {
        cboCompraPartido.Enabled = habilitado;
        cboCompraLocalidad.Enabled = habilitado;
        numCompraCantidad.Enabled = habilitado;
        btnComprar.Enabled = habilitado;
        btnConsultarCompras.Enabled = habilitado;
        dgvCompras.Enabled = habilitado;

        // El botón de validar permanece activo solo cuando no hay cliente autenticado.
        btnValidarCliente.Enabled = !habilitado || _clienteAutenticado == null;
    }

    /// <summary>
    /// Conecta o desconecta del servidor TCP. Al conectar envía Ping para verificar que el servidor responde.
    /// </summary>
    private void btnConectar_Click(object sender, EventArgs e)
    {
        try
        {
            // Si ya hay conexión, el mismo botón actúa como desconexión y restablece la UI.
            if (_clienteRed.EstaConectado)
            {
                _clienteRed.Desconectar();
                btnConectar.Text = "Conectar al servidor";
                lblEstadoConexion.Text = "Desconectado";
                AppleTheme.EstilizarEstado(lblEstadoConexion, false);
                _clienteAutenticado = null;
                lblClienteAutenticado.Text = "Sin autenticar";
                lblClienteAutenticado.ForeColor = AppleTheme.TextoSecundario;
                HabilitarFuncionalidades(false);
                txtIdentificacion.Enabled = true;
                btnValidarCliente.Enabled = true;
                return;
            }

            // Abre socket TCP hacia ServidorConfig (127.0.0.1:14500 por defecto).
            _clienteRed.Conectar();

            // Comando Ping: verifica disponibilidad del servidor antes de habilitar autenticación/compras.
            var respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
            {
                Comando = ComandosRed.Ping,
                Parametros = new Dictionary<string, string>()
            });

            if (!respuesta.Exito)
            {
                throw new InvalidOperationException(respuesta.Mensaje);
            }

            // Actualiza indicadores visuales de conexión exitosa en la tarjeta de conexión.
            btnConectar.Text = "Desconectar";
            lblEstadoConexion.Text = "Conectado al servidor";
            AppleTheme.EstilizarEstado(lblEstadoConexion, true);
            MessageBox.Show("Conexión establecida correctamente.", "Boletaría UNED", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Valida al cliente en el servidor por identificación (cédula) o por Id numérico.
    /// Tras éxito, carga partidos activos y el historial de compras.
    /// </summary>
    private void btnValidarCliente_Click(object sender, EventArgs e)
    {
        try
        {
            // La autenticación requiere una sesión TCP activa con el servidor.
            if (!_clienteRed.EstaConectado)
            {
                throw new InvalidOperationException("Debe conectarse al servidor primero.");
            }

            var identificacion = txtIdentificacion.Text.Trim();
            if (string.IsNullOrWhiteSpace(identificacion) && string.IsNullOrWhiteSpace(txtIdCliente.Text))
            {
                throw new InvalidOperationException("Ingrese la identificación o el Id del cliente.");
            }

            MensajeRespuesta respuesta;

            // Autenticación por cédula/identificación (comando ValidarCliente).
            if (!string.IsNullOrWhiteSpace(identificacion))
            {
                respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
                {
                    Comando = ComandosRed.ValidarCliente,
                    Parametros = new Dictionary<string, string> { { "Identificacion", identificacion } }
                });
            }
            else
            {
                // Autenticación alternativa por IdCliente numérico (comando ValidarClientePorId).
                respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
                {
                    Comando = ComandosRed.ValidarClientePorId,
                    Parametros = new Dictionary<string, string> { { "IdCliente", txtIdCliente.Text.Trim() } }
                });
            }

            if (!respuesta.Exito || string.IsNullOrWhiteSpace(respuesta.Datos))
            {
                throw new InvalidOperationException(respuesta.Mensaje);
            }

            // Deserializa el JSON del cliente devuelto por el servidor y lo guarda en sesión local.
            _clienteAutenticado = JsonSerializer.Deserialize<ClienteEntidad>(respuesta.Datos, JsonConfig.Opciones);
            if (_clienteAutenticado == null)
            {
                throw new InvalidOperationException("No se pudo obtener la información del cliente.");
            }

            // UI post-autenticación: saludo, bloqueo de campos de identificación y habilitación de compras.
            lblClienteAutenticado.Text = $"Bienvenido, {_clienteAutenticado.NombreCompleto}";
            lblClienteAutenticado.ForeColor = AppleTheme.Exito;
            txtIdentificacion.Enabled = false;
            txtIdCliente.Enabled = false;
            btnValidarCliente.Enabled = false;
            HabilitarFuncionalidades(true);
            CargarPartidosActivos();
            CargarCompras(mostrarError: false);
            MessageBox.Show($"Bienvenido, {_clienteAutenticado.NombreCompleto}.", "Validación exitosa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            // Ante fallo de autenticación, limpia sesión y mantiene compras deshabilitadas.
            _clienteAutenticado = null;
            HabilitarFuncionalidades(false);
            lblClienteAutenticado.Text = "Validación fallida";
            MessageBox.Show(ex.Message, "Validación de cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Solicita al servidor la lista de partidos activos y la enlaza al ComboBox de compra.
    /// </summary>
    private void CargarPartidosActivos()
    {
        var respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
        {
            Comando = ComandosRed.ConsultarPartidosActivos,
            Parametros = new Dictionary<string, string>()
        });

        if (!respuesta.Exito || string.IsNullOrWhiteSpace(respuesta.Datos))
        {
            throw new InvalidOperationException(respuesta.Mensaje);
        }

        var partidos = JsonSerializer.Deserialize<List<Partido>>(respuesta.Datos, JsonConfig.Opciones) ?? new List<Partido>();
        cboCompraPartido.DataSource = partidos;
        cboCompraPartido.DisplayMember = "DescripcionPartido";
        cboCompraPartido.ValueMember = "IdPartido";
    }

    /// <summary>
    /// Al cambiar el partido seleccionado, carga las localidades disponibles y recalcula el monto.
    /// </summary>
    private void cboCompraPartido_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cboCompraPartido.SelectedItem is not Partido partido)
            {
                return;
            }

            lblCompraInfoPartido.Text = $"Partido: {partido.DescripcionPartido}";

            // Consulta inventario de localidades (LocalidadPorPartido) para el partido elegido.
            var respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
            {
                Comando = ComandosRed.ConsultarLocalidadesPartido,
                Parametros = new Dictionary<string, string> { { "IdPartido", partido.IdPartido.ToString() } }
            });

            if (!respuesta.Exito || string.IsNullOrWhiteSpace(respuesta.Datos))
            {
                cboCompraLocalidad.DataSource = null;
                return;
            }

            var localidades = JsonSerializer.Deserialize<List<LocalidadPorPartido>>(respuesta.Datos, JsonConfig.Opciones) ?? new List<LocalidadPorPartido>();
            cboCompraLocalidad.DataSource = localidades;
            cboCompraLocalidad.DisplayMember = "DescripcionLocalidad";
            cboCompraLocalidad.ValueMember = "IdLocalidadPartido";
            CalcularMontoCompra();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>Recalcula monto al cambiar la localidad seleccionada.</summary>
    private void cboCompraLocalidad_SelectedIndexChanged(object sender, EventArgs e) => CalcularMontoCompra();

    /// <summary>Recalcula monto al cambiar la cantidad de entradas.</summary>
    private void numCompraCantidad_ValueChanged(object sender, EventArgs e) => CalcularMontoCompra();

    /// <summary>
    /// Calcula el monto total (precio × cantidad) y muestra entradas disponibles para la localidad.
    /// </summary>
    private void CalcularMontoCompra()
    {
        if (cboCompraLocalidad.SelectedItem is LocalidadPorPartido lp)
        {
            txtCompraMonto.Text = (lp.Localidad.Precio * numCompraCantidad.Value).ToString("N2");
            lblCompraDisponible.Text = $"Disponibles: {lp.CantidadDisponible}";
        }
    }

    /// <summary>
    /// Ejecuta la compra en línea: envía ComprarEnLinea al servidor con cliente, partido, localidad y cantidad.
    /// </summary>
    private void btnComprar_Click(object sender, EventArgs e)
    {
        try
        {
            if (_clienteAutenticado == null)
            {
                throw new InvalidOperationException("Debe autenticarse primero.");
            }

            if (cboCompraPartido.SelectedItem is not Partido partido ||
                cboCompraLocalidad.SelectedItem is not LocalidadPorPartido lp)
            {
                throw new InvalidOperationException("Seleccione partido y localidad.");
            }

            // Comando de compra en línea: el servidor valida stock, cliente activo y registra la venta.
            var respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
            {
                Comando = ComandosRed.ComprarEnLinea,
                Parametros = new Dictionary<string, string>
                {
                    { "IdCliente", _clienteAutenticado.Id.ToString() },
                    { "IdPartido", partido.IdPartido.ToString() },
                    { "IdLocalidad", lp.Localidad.IdLocalidad.ToString() },
                    { "Cantidad", ((int)numCompraCantidad.Value).ToString() }
                }
            });

            if (!respuesta.Exito)
            {
                throw new InvalidOperationException(respuesta.Mensaje);
            }

            MessageBox.Show(respuesta.Mensaje, "Compra exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Refresca disponibilidad y el grid de compras tras la transacción.
            cboCompraPartido_SelectedIndexChanged(sender, e);
            CargarCompras(mostrarError: false);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error en compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>Recarga el historial de compras del cliente autenticado desde el servidor.</summary>
    private void btnConsultarCompras_Click(object sender, EventArgs e) => CargarCompras();

    /// <summary>
    /// Consulta las ventas del cliente autenticado y las muestra en el DataGridView.
    /// </summary>
    /// <param name="mostrarError">Si es false, suprime MessageBox en errores (útil en carga automática inicial).</param>
    private void CargarCompras(bool mostrarError = true)
    {
        try
        {
            if (_clienteAutenticado == null)
            {
                throw new InvalidOperationException("Debe autenticarse primero.");
            }

            var respuesta = _clienteRed.EnviarSolicitud(new MensajeSolicitud
            {
                Comando = ComandosRed.ConsultarCompras,
                Parametros = new Dictionary<string, string> { { "IdCliente", _clienteAutenticado.Id.ToString() } }
            });

            if (!respuesta.Exito)
            {
                throw new InvalidOperationException(respuesta.Mensaje);
            }

            var compras = ConsultaComprasParser.DesdeJson(respuesta.Datos);
            dgvCompras.DataSource = null;
            dgvCompras.DataSource = compras;
            lblConsultaInfo.Text = ConsultaComprasParser.ResumenCantidad(compras.Count);
        }
        catch (Exception ex)
        {
            if (mostrarError)
            {
                MessageBox.Show(ex.Message, "Error en consulta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }

    /// <summary>
    /// Al cerrar la ventana, cierra la conexión TCP para liberar recursos en el servidor y cliente.
    /// </summary>
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _clienteRed.Desconectar();
    }
}
