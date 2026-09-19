/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Formulario principal de administración del servidor.
 */

using BOLETERIAUNED.Entidades;
using BOLETERIAUNED.LogicaNegocio;

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Formulario principal del servidor de boletaría UNED.
/// Integra la administración de catálogos, ventas en boletería física, consultas en grillas
/// y la bitácora en tiempo real del servicio TCP multihilo.
/// </summary>
public partial class Form1 : Form
{
    /// <summary>Instancia del servidor TCP que atiende clientes remotos en 127.0.0.1:14500.</summary>
    private readonly ServidorTCP _servidorTcp = new();

    /// <summary>Capa de negocio para operaciones CRUD de localidades.</summary>
    private readonly LocalidadLN _localidadLN = new();

    /// <summary>Capa de negocio para operaciones CRUD y consulta de partidos.</summary>
    private readonly PartidoLN _partidoLN = new();

    /// <summary>Capa de negocio para operaciones CRUD de vendedores.</summary>
    private readonly VendedorLN _vendedorLN = new();

    /// <summary>Capa de negocio para operaciones CRUD de clientes.</summary>
    private readonly ClienteLN _clienteLN = new();

    /// <summary>Capa de negocio para asociación localidad-partido e inventario.</summary>
    private readonly LocalidadPorPartidoLN _localidadPartidoLN = new();

    /// <summary>Capa de negocio para registro y consulta de ventas.</summary>
    private readonly VentaLN _ventaLN = new();

    /// <summary>
    /// Inicializa el formulario, configura grillas, suscribe eventos TCP y arranca el servidor.
    /// </summary>
    public Form1()
    {
        InitializeComponent();
        ConfigurarGrids();

        // Suscripción a eventos del servidor TCP (disparados desde hilos de red).
        _servidorTcp.EventoBitacora += AgregarBitacora;
        _servidorTcp.DatosActualizados += ActualizarConsultasDesdeEvento;

        // El servidor TCP inicia en segundo plano al abrir la aplicación.
        _servidorTcp.Iniciar();
        CargarCombosVenta();
        CargarTodasLasConsultas();
    }

    /// <summary>
    /// Aplica configuración común de solo lectura y estilo visual a todas las grillas de consulta.
    /// </summary>
    private void ConfigurarGrids()
    {
        ConfigurarGrid(dgvLocalidades);
        ConfigurarGrid(dgvPartidos);
        ConfigurarGrid(dgvVendedores);
        ConfigurarGrid(dgvClientes);
        ConfigurarGrid(dgvLp);
        ConfigurarGrid(dgvVentas);
    }

    /// <summary>
    /// Configura un <see cref="DataGridView"/> como grilla de consulta de solo lectura con tema Apple.
    /// </summary>
    /// <param name="grid">Grilla a estilizar y configurar.</param>
    private static void ConfigurarGrid(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoGenerateColumns = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        // Estilos visuales: encabezados, filas alternas y colores del tema AppleTheme.
        AppleTheme.EstilizarGrid(grid);
    }

    /// <summary>
    /// Ejecuta todas las consultas iniciales simulando clic en cada botón "Actualizar".
    /// </summary>
    private void CargarTodasLasConsultas()
    {
        btnConsultarLocalidades_Click(this, EventArgs.Empty);
        btnConsultarPartidos_Click(this, EventArgs.Empty);
        btnConsultarVendedores_Click(this, EventArgs.Empty);
        btnConsultarClientes_Click(this, EventArgs.Empty);
        btnConsultarLp_Click(this, EventArgs.Empty);
        btnConsultarVentas_Click(this, EventArgs.Empty);
    }

    /// <summary>
    /// Agrega una línea a la bitácora (<see cref="lstBitacora"/>).
    /// Thread-safe: si se invoca desde un hilo TCP, redirige al hilo de UI con BeginInvoke.
    /// </summary>
    /// <param name="mensaje">Texto con marca de tiempo generado por <see cref="ServidorTCP"/>.</param>
    private void AgregarBitacora(string mensaje)
    {
        // InvokeRequired=true cuando el callback proviene de hilos de escucha o de cliente TCP.
        if (InvokeRequired)
        {
            // BeginInvoke encola la actualización en el hilo de UI sin bloquear el hilo TCP.
            BeginInvoke(new Action<string>(AgregarBitacora), mensaje);
            return;
        }

        // Insertar al inicio: los eventos más recientes aparecen arriba en la bitácora.
        lstBitacora.Items.Insert(0, mensaje);
    }

    /// <summary>
    /// Carga los ComboBox de venta y localidad-partido con datos activos de la base de datos.
    /// </summary>
    private void CargarCombosVenta()
    {
        try
        {
            cboVentaCliente.DataSource = _clienteLN.ConsultarActivos();
            cboVentaCliente.DisplayMember = "NombreCompleto";
            cboVentaCliente.ValueMember = "Id";

            cboVentaVendedor.DataSource = _vendedorLN.ConsultarTodos();
            cboVentaVendedor.DisplayMember = "NombreCompleto";
            cboVentaVendedor.ValueMember = "Id";

            cboVentaPartido.DataSource = _partidoLN.ConsultarActivos();
            cboVentaPartido.DisplayMember = "DescripcionPartido";
            cboVentaPartido.ValueMember = "IdPartido";

            cboLpPartido.DataSource = _partidoLN.ConsultarTodos();
            cboLpPartido.DisplayMember = "DescripcionPartido";
            cboLpPartido.ValueMember = "IdPartido";

            cboLpLocalidad.DataSource = _localidadLN.ConsultarTodos();
            cboLpLocalidad.DisplayMember = "NombreLocalidad";
            cboLpLocalidad.ValueMember = "IdLocalidad";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Handler del botón Guardar en la tarjeta de registro de localidades.
    /// Persiste la entidad y refresca combos, consultas y bitácora.
    /// </summary>
    private void btnRegistrarLocalidad_Click(object sender, EventArgs e)
    {
        try
        {
            var localidad = new Localidad(
                (int)numLocId.Value,
                txtLocNombre.Text.Trim(),
                numLocPrecio.Value);

            _localidadLN.Registrar(localidad);
            MessageBox.Show("Localidad registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Localidad registrada: {localidad.NombreLocalidad}");
            CargarCombosVenta();
            CargarTodasLasConsultas();
            LimpiarLocalidad();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error registro localidad: {ex.Message}");
        }
    }

    /// <summary>
    /// Handler del botón Guardar en la tarjeta de registro de partidos.
    /// </summary>
    private void btnRegistrarPartido_Click(object sender, EventArgs e)
    {
        try
        {
            var partido = new Partido(
                (int)numParId.Value,
                txtParRival.Text.Trim(),
                dtpParFecha.Value.Date,
                txtParHora.Text.Trim(),
                chkParActivo.Checked);

            _partidoLN.Registrar(partido);
            MessageBox.Show("Partido registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Partido registrado: {partido.Rival}");
            CargarCombosVenta();
            CargarTodasLasConsultas();
            LimpiarPartido();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error registro partido: {ex.Message}");
        }
    }

    /// <summary>
    /// Handler del botón Guardar en la tarjeta de registro de vendedores.
    /// </summary>
    private void btnRegistrarVendedor_Click(object sender, EventArgs e)
    {
        try
        {
            var vendedor = new Vendedor(
                (int)numVenId.Value,
                txtVenIdentificacion.Text.Trim(),
                txtVenNombre.Text.Trim(),
                txtVenApellido.Text.Trim(),
                dtpVenNacimiento.Value.Date,
                dtpVenIngreso.Value.Date);

            _vendedorLN.Registrar(vendedor);
            MessageBox.Show("Vendedor registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Vendedor registrado: {vendedor.NombreCompleto}");
            CargarCombosVenta();
            CargarTodasLasConsultas();
            LimpiarVendedor();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error registro vendedor: {ex.Message}");
        }
    }

    /// <summary>
    /// Handler del botón Guardar en la tarjeta de registro de clientes.
    /// </summary>
    private void btnRegistrarCliente_Click(object sender, EventArgs e)
    {
        try
        {
            var cliente = new Cliente(
                (int)numCliId.Value,
                txtCliIdentificacion.Text.Trim(),
                txtCliNombre.Text.Trim(),
                txtCliApellido.Text.Trim(),
                dtpCliNacimiento.Value.Date,
                dtpCliRegistro.Value.Date,
                chkCliActivo.Checked);

            _clienteLN.Registrar(cliente);
            MessageBox.Show("Cliente registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Cliente registrado: {cliente.NombreCompleto}");
            CargarCombosVenta();
            CargarTodasLasConsultas();
            LimpiarCliente();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error registro cliente: {ex.Message}");
        }
    }

    /// <summary>
    /// Handler del botón Guardar asociación en la tarjeta localidad por partido.
    /// </summary>
    private void btnRegistrarLp_Click(object sender, EventArgs e)
    {
        try
        {
            if (cboLpPartido.SelectedItem is not Partido partido || cboLpLocalidad.SelectedItem is not Localidad localidad)
            {
                throw new InvalidOperationException("Debe seleccionar partido y localidad.");
            }

            var registro = new LocalidadPorPartido(
                (int)numLpId.Value,
                partido,
                localidad,
                (int)numLpCantidad.Value);

            _localidadPartidoLN.Registrar(registro);
            lblLpInfoPartido.Text = $"Partido: {partido.DescripcionPartido} | Activo: {(partido.Activo ? "Sí" : "No")}";
            MessageBox.Show("Localidad por partido registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Localidad por partido registrada: {partido.Rival} - {localidad.NombreLocalidad}");
            CargarTodasLasConsultas();
            LimpiarLp();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error registro localidad/partido: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza la etiqueta informativa al cambiar el partido seleccionado en localidad-partido.
    /// </summary>
    private void cboLpPartido_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboLpPartido.SelectedItem is Partido partido)
        {
            lblLpInfoPartido.Text = $"Partido: {partido.DescripcionPartido} | Activo: {(partido.Activo ? "Sí" : "No")}";
        }
    }

    /// <summary>
    /// Al seleccionar partido en venta, carga las localidades disponibles para ese partido.
    /// </summary>
    private void cboVentaPartido_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cboVentaPartido.SelectedItem is Partido partido)
            {
                lblVentaInfoPartido.Text = $"Partido seleccionado: {partido.DescripcionPartido}";
                var localidades = _localidadPartidoLN.ConsultarPorPartido(partido.IdPartido);
                cboVentaLocalidad.DataSource = localidades;
                cboVentaLocalidad.DisplayMember = "DescripcionLocalidad";
                cboVentaLocalidad.ValueMember = "IdLocalidadPartido";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Recalcula monto y disponibilidad al cambiar la localidad seleccionada en venta.
    /// </summary>
    private void cboVentaLocalidad_SelectedIndexChanged(object sender, EventArgs e)
    {
        CalcularMontoVenta();
    }

    /// <summary>
    /// Recalcula monto y disponibilidad al modificar la cantidad de entradas.
    /// </summary>
    private void numVentaCantidad_ValueChanged(object sender, EventArgs e)
    {
        CalcularMontoVenta();
    }

    /// <summary>
    /// Calcula el monto total (precio × cantidad) y muestra entradas disponibles.
    /// </summary>
    private void CalcularMontoVenta()
    {
        if (cboVentaLocalidad.SelectedItem is LocalidadPorPartido lp)
        {
            var monto = lp.Localidad.Precio * numVentaCantidad.Value;
            txtVentaMonto.Text = monto.ToString("N2");
            lblVentaDisponible.Text = $"Disponibles: {lp.CantidadDisponible}";
        }
    }

    /// <summary>
    /// Handler del botón Registrar venta en boletería física.
    /// Valida selección de combos y persiste la venta con vendedor asignado.
    /// </summary>
    private void btnRegistrarVenta_Click(object sender, EventArgs e)
    {
        try
        {
            if (cboVentaCliente.SelectedItem is not Cliente cliente ||
                cboVentaVendedor.SelectedItem is not Vendedor vendedor ||
                cboVentaPartido.SelectedItem is not Partido partido ||
                cboVentaLocalidad.SelectedItem is not LocalidadPorPartido lp)
            {
                throw new InvalidOperationException("Complete todos los campos de la venta.");
            }

            var venta = new Venta
            {
                Cliente = cliente,
                Vendedor = vendedor,
                Partido = partido,
                Localidad = lp.Localidad,
                Cantidad = (int)numVentaCantidad.Value
            };

            var idVenta = _ventaLN.RegistrarVentaBoleteria(venta);
            MessageBox.Show($"Venta registrada correctamente. IdVenta: {idVenta}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AgregarBitacora($"Venta boletería procesada. IdVenta: {idVenta}");
            cboVentaPartido_SelectedIndexChanged(sender, e);
            CalcularMontoVenta();
            CargarTodasLasConsultas();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error venta boletería: {ex.Message}");
        }
    }

    /// <summary>Handler del botón Actualizar en la pestaña de consulta de localidades.</summary>
    private void btnConsultarLocalidades_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvLocalidades, _localidadLN.ConsultarTodos(), "Localidades");

    /// <summary>Handler del botón Actualizar en la pestaña de consulta de partidos.</summary>
    private void btnConsultarPartidos_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvPartidos, _partidoLN.ConsultarTodos(), "Partidos");

    /// <summary>Handler del botón Actualizar en la pestaña de consulta de vendedores.</summary>
    private void btnConsultarVendedores_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvVendedores, _vendedorLN.ConsultarTodos(), "Vendedores");

    /// <summary>Handler del botón Actualizar en la pestaña de consulta de clientes.</summary>
    private void btnConsultarClientes_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvClientes, _clienteLN.ConsultarTodos(), "Clientes");

    /// <summary>Handler del botón Actualizar en la pestaña de localidades por partido.</summary>
    private void btnConsultarLp_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvLp, _localidadPartidoLN.ConsultarTodosDetalle(), "Localidades por Partido");

    /// <summary>Handler del botón Actualizar en la pestaña de ventas.</summary>
    private void btnConsultarVentas_Click(object sender, EventArgs e) =>
        MostrarConsulta(dgvVentas, _ventaLN.ConsultarTodosDetalle(), "Ventas");

    /// <summary>
    /// Enlaza una lista genérica a una grilla y registra el resultado en la bitácora.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad o DTO mostrado en la grilla.</typeparam>
    /// <param name="grid">Grilla destino.</param>
    /// <param name="datos">Lista de registros a mostrar.</param>
    /// <param name="titulo">Nombre descriptivo de la consulta para la bitácora.</param>
    private void MostrarConsulta<T>(DataGridView grid, List<T> datos, string titulo)
    {
        try
        {
            grid.DataSource = null;
            grid.DataSource = datos;
            AgregarBitacora($"Consulta ejecutada: {titulo} ({datos.Count} registros)");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AgregarBitacora($"Error consulta {titulo}: {ex.Message}");
        }
    }

    /// <summary>Restablece los campos del formulario de registro de localidad a valores por defecto.</summary>
    private void LimpiarLocalidad()
    {
        numLocId.Value = 1;
        txtLocNombre.Clear();
        numLocPrecio.Value = 1;
    }

    /// <summary>Restablece los campos del formulario de registro de partido a valores por defecto.</summary>
    private void LimpiarPartido()
    {
        numParId.Value = 1;
        txtParRival.Clear();
        dtpParFecha.Value = DateTime.Today;
        txtParHora.Text = "15:00";
        chkParActivo.Checked = true;
    }

    /// <summary>Restablece los campos del formulario de registro de vendedor a valores por defecto.</summary>
    private void LimpiarVendedor()
    {
        numVenId.Value = 1;
        txtVenIdentificacion.Clear();
        txtVenNombre.Clear();
        txtVenApellido.Clear();
        dtpVenNacimiento.Value = DateTime.Today.AddYears(-25);
        dtpVenIngreso.Value = DateTime.Today;
    }

    /// <summary>Restablece los campos del formulario de registro de cliente a valores por defecto.</summary>
    private void LimpiarCliente()
    {
        numCliId.Value = 1;
        txtCliIdentificacion.Clear();
        txtCliNombre.Clear();
        txtCliApellido.Clear();
        dtpCliNacimiento.Value = DateTime.Today.AddYears(-20);
        dtpCliRegistro.Value = DateTime.Today;
        chkCliActivo.Checked = true;
    }

    /// <summary>Restablece los campos del formulario de localidad por partido a valores por defecto.</summary>
    private void LimpiarLp()
    {
        numLpId.Value = 1;
        numLpCantidad.Value = 1;
    }

    /// <summary>
    /// Refresca todas las grillas cuando el servidor TCP notifica cambios en datos
    /// (por ejemplo, venta en línea desde cliente remoto).
    /// Thread-safe mediante BeginInvoke al hilo de UI.
    /// </summary>
    private void ActualizarConsultasDesdeEvento()
    {
        // Mismo patrón que AgregarBitacora: el evento DatosActualizados puede llegar desde hilos TCP.
        if (InvokeRequired)
        {
            BeginInvoke(ActualizarConsultasDesdeEvento);
            return;
        }

        CargarTodasLasConsultas();
    }

    /// <summary>
    /// Detiene el servidor TCP de forma ordenada al cerrar la ventana principal.
    /// </summary>
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _servidorTcp.Detener();
    }
}
