/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Diseño del formulario servidor — layout responsivo pantalla completa.
 */

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Parte parcial de <see cref="Form1"/> que define la construcción visual del formulario:
/// pestañas, tarjetas de registro, grillas de consulta, venta en boletería y bitácora.
/// </summary>
partial class Form1
{
    /// <summary>Pestaña principal que agrupa Registros, Consultas y Venta boletería.</summary>
    private TabControl tabPrincipal;

    /// <summary>Pestaña con tarjetas de registro CRUD (localidad, partido, vendedor, cliente, LP).</summary>
    private TabPage tabRegistros;

    /// <summary>Pestaña con sub-pestañas de consultas en grillas de solo lectura.</summary>
    private TabPage tabConsultas;

    /// <summary>Pestaña para registrar ventas en boletería física.</summary>
    private TabPage tabVenta;

    /// <summary>ListBox inferior que muestra la bitácora en tiempo real del servidor TCP.</summary>
    private ListBox lstBitacora;

    /// <summary>Etiqueta titular del panel de bitácora.</summary>
    private Label lblBitacora;

    /// <summary>Grilla de consulta de localidades.</summary>
    private DataGridView dgvLocalidades;

    /// <summary>Grilla de consulta de partidos.</summary>
    private DataGridView dgvPartidos;

    /// <summary>Grilla de consulta de vendedores.</summary>
    private DataGridView dgvVendedores;

    /// <summary>Grilla de consulta de clientes.</summary>
    private DataGridView dgvClientes;

    /// <summary>Grilla de consulta de localidades por partido.</summary>
    private DataGridView dgvLp;

    /// <summary>Grilla de consulta de ventas.</summary>
    private DataGridView dgvVentas;

    /// <summary>Sub-pestañas internas dentro de la pestaña Consultas.</summary>
    private TabControl tabConsultasInner;

    /// <summary>Campo numérico Id de localidad en tarjeta de registro.</summary>
    private NumericUpDown numLocId;

    /// <summary>Campo de texto nombre de localidad.</summary>
    private TextBox txtLocNombre;

    /// <summary>Campo numérico precio unitario de la localidad.</summary>
    private NumericUpDown numLocPrecio;

    /// <summary>Botón Guardar localidad; enlazado a <see cref="btnRegistrarLocalidad_Click"/>.</summary>
    private Button btnRegistrarLocalidad;

    /// <summary>Campo numérico Id de partido.</summary>
    private NumericUpDown numParId;

    /// <summary>Campo de texto nombre del rival.</summary>
    private TextBox txtParRival;

    /// <summary>Selector de fecha del partido.</summary>
    private DateTimePicker dtpParFecha;

    /// <summary>Campo de texto hora del partido (formato libre, ej. 15:00).</summary>
    private TextBox txtParHora;

    /// <summary>CheckBox que indica si el partido está activo para venta.</summary>
    private CheckBox chkParActivo;

    /// <summary>Botón Guardar partido; enlazado a <see cref="btnRegistrarPartido_Click"/>.</summary>
    private Button btnRegistrarPartido;

    /// <summary>Campo numérico Id de vendedor.</summary>
    private NumericUpDown numVenId;

    /// <summary>Campo de texto identificación del vendedor.</summary>
    private TextBox txtVenIdentificacion;

    /// <summary>Campo de texto nombre del vendedor.</summary>
    private TextBox txtVenNombre;

    /// <summary>Campo de texto apellido del vendedor.</summary>
    private TextBox txtVenApellido;

    /// <summary>Selector de fecha de nacimiento del vendedor.</summary>
    private DateTimePicker dtpVenNacimiento;

    /// <summary>Selector de fecha de ingreso del vendedor.</summary>
    private DateTimePicker dtpVenIngreso;

    /// <summary>Botón Guardar vendedor; enlazado a <see cref="btnRegistrarVendedor_Click"/>.</summary>
    private Button btnRegistrarVendedor;

    /// <summary>Campo numérico Id de cliente.</summary>
    private NumericUpDown numCliId;

    /// <summary>Campo de texto identificación del cliente.</summary>
    private TextBox txtCliIdentificacion;

    /// <summary>Campo de texto nombre del cliente.</summary>
    private TextBox txtCliNombre;

    /// <summary>Campo de texto apellido del cliente.</summary>
    private TextBox txtCliApellido;

    /// <summary>Selector de fecha de nacimiento del cliente.</summary>
    private DateTimePicker dtpCliNacimiento;

    /// <summary>Selector de fecha de registro del cliente.</summary>
    private DateTimePicker dtpCliRegistro;

    /// <summary>CheckBox que indica si el cliente está activo.</summary>
    private CheckBox chkCliActivo;

    /// <summary>Botón Guardar cliente; enlazado a <see cref="btnRegistrarCliente_Click"/>.</summary>
    private Button btnRegistrarCliente;

    /// <summary>Campo numérico Id de registro localidad-partido.</summary>
    private NumericUpDown numLpId;

    /// <summary>ComboBox de selección de partido para asociación LP.</summary>
    private ComboBox cboLpPartido;

    /// <summary>ComboBox de selección de localidad para asociación LP.</summary>
    private ComboBox cboLpLocalidad;

    /// <summary>Campo numérico cantidad de entradas disponibles en LP.</summary>
    private NumericUpDown numLpCantidad;

    /// <summary>Etiqueta informativa con descripción y estado del partido seleccionado en LP.</summary>
    private Label lblLpInfoPartido;

    /// <summary>Botón Guardar asociación LP; enlazado a <see cref="btnRegistrarLp_Click"/>.</summary>
    private Button btnRegistrarLp;

    /// <summary>ComboBox de cliente en formulario de venta boletería.</summary>
    private ComboBox cboVentaCliente;

    /// <summary>ComboBox de vendedor en formulario de venta boletería.</summary>
    private ComboBox cboVentaVendedor;

    /// <summary>ComboBox de partido en formulario de venta boletería.</summary>
    private ComboBox cboVentaPartido;

    /// <summary>ComboBox de localidad disponible para el partido seleccionado.</summary>
    private ComboBox cboVentaLocalidad;

    /// <summary>Campo numérico cantidad de entradas a vender.</summary>
    private NumericUpDown numVentaCantidad;

    /// <summary>Campo de solo lectura con monto total calculado.</summary>
    private TextBox txtVentaMonto;

    /// <summary>Etiqueta con descripción del partido seleccionado en venta.</summary>
    private Label lblVentaInfoPartido;

    /// <summary>Etiqueta con cantidad de entradas disponibles para la localidad seleccionada.</summary>
    private Label lblVentaDisponible;

    /// <summary>Botón Registrar venta; enlazado a <see cref="btnRegistrarVenta_Click"/>.</summary>
    private Button btnRegistrarVenta;

    /// <summary>
    /// Construye la jerarquía visual completa del formulario: shell, bitácora, pestañas y controles.
    /// </summary>
    private void InitializeComponent()
    {
        SuspendLayout();
        FormClosing += Form1_FormClosing;

        // Estilos visuales de ventana: maximizada, fondo claro y tamaño mínimo responsivo.
        AppleTheme.ConfigurarVentanaCompleta(this, "Boletaría UNED - Servidor");

        // Shell de tres filas: encabezado, contenido principal y panel de bitácora inferior.
        var shell = AppleTheme.CrearShell(
            "Boletaría UNED",
            "Panel administrativo | TCP 127.0.0.1:14500",
            out var areaContenido,
            out var areaBitacora);

        lblBitacora = new Label();
        lstBitacora = new ListBox();
        // Panel oscuro inferior para bitácora en tiempo real (eventos TCP y operaciones UI).
        AppleTheme.ConfigurarBitacora(areaBitacora, lblBitacora, lstBitacora);

        tabPrincipal = new TabControl();
        tabRegistros = new TabPage("Registros");
        tabConsultas = new TabPage("Consultas");
        tabVenta = new TabPage("Venta boletería");

        CrearTabRegistros();
        CrearTabConsultas();
        CrearTabVenta();
        // Pestañas con dibujo personalizado (OwnerDraw) y colores del tema.
        AppleTheme.EstilizarTabControl(tabPrincipal);

        areaContenido.Controls.Add(tabPrincipal);
        Controls.Add(shell);

        // Al mostrarse la ventana, recalcula scrolls en paneles anidados.
        Shown += (_, _) => AppleTheme.RefrescarScrolls(this);
        ResumeLayout(true);
    }

    /// <summary>
    /// Crea la pestaña Registros con grid 2×3 de tarjetas y scroll vertical responsivo.
    /// </summary>
    private void CrearTabRegistros()
    {
        var scroll = AppleTheme.CrearPanelScroll();
        var grid = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 3,
            BackColor = AppleTheme.Fondo,
            Padding = new Padding(4),
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            Dock = DockStyle.Top
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFilaRegistros(0)));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFilaRegistros(1)));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFilaRegistros(2)));
        grid.Height = AppleTheme.AlturaGridRegistros();

        grid.Controls.Add(CrearTarjetaLocalidad(), 0, 0);
        grid.Controls.Add(CrearTarjetaPartido(), 1, 0);
        grid.Controls.Add(CrearTarjetaVendedor(), 0, 1);
        grid.Controls.Add(CrearTarjetaCliente(), 1, 1);
        grid.Controls.Add(CrearTarjetaLp(), 0, 2);
        // La tarjeta LP ocupa el ancho completo de la fila inferior.
        grid.SetColumnSpan(grid.GetControlFromPosition(0, 2), 2);

        AppleTheme.EnlazarContenidoScroll(scroll, grid, AppleTheme.AlturaGridRegistros());
        tabRegistros.Controls.Add(scroll);
        tabPrincipal.TabPages.Add(tabRegistros);
    }

    /// <summary>
    /// Construye la tarjeta visual de registro de localidades con campos Id, Nombre y Precio.
    /// </summary>
    /// <returns>Panel tipo tarjeta listo para insertar en el grid de registros.</returns>
    private Panel CrearTarjetaLocalidad()
    {
        var tarjeta = AppleTheme.CrearTarjeta("Localidad", 3);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var form = AppleTheme.CrearFormulario(3);
        numLocId = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1 };
        txtLocNombre = new TextBox();
        numLocPrecio = new NumericUpDown { Minimum = 1, Maximum = 9999999, DecimalPlaces = 2, Value = 1000 };
        btnRegistrarLocalidad = new Button { Text = "Guardar" };
        // Handler de botón: persiste localidad vía lógica de negocio en Form1.cs.
        btnRegistrarLocalidad.Click += btnRegistrarLocalidad_Click;
        AppleTheme.EstilizarNumerico(numLocId);
        AppleTheme.EstilizarEntrada(txtLocNombre);
        AppleTheme.EstilizarNumerico(numLocPrecio);
        AppleTheme.AgregarCampo(form, 0, "Id", numLocId);
        AppleTheme.AgregarCampo(form, 1, "Nombre", txtLocNombre);
        AppleTheme.AgregarCampo(form, 2, "Precio (col.)", numLocPrecio);
        AppleTheme.AgregarBoton(form, 3, btnRegistrarLocalidad);
        AppleTheme.AgregarContenido(cuerpo, form);
        return tarjeta;
    }

    /// <summary>
    /// Construye la tarjeta visual de registro de partidos con rival, fecha, hora y estado activo.
    /// </summary>
    /// <returns>Panel tipo tarjeta listo para insertar en el grid de registros.</returns>
    private Panel CrearTarjetaPartido()
    {
        var tarjeta = AppleTheme.CrearTarjeta("Partido", 4);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var form = AppleTheme.CrearFormulario(4);
        numParId = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1 };
        txtParRival = new TextBox();
        dtpParFecha = new DateTimePicker();
        txtParHora = new TextBox { Text = "15:00" };
        chkParActivo = new CheckBox { Text = "Activo", Checked = true, ForeColor = AppleTheme.Texto, Dock = DockStyle.Left };
        btnRegistrarPartido = new Button { Text = "Guardar" };
        btnRegistrarPartido.Click += btnRegistrarPartido_Click;
        AppleTheme.EstilizarNumerico(numParId);
        AppleTheme.EstilizarEntrada(txtParRival);
        AppleTheme.EstilizarFecha(dtpParFecha);
        AppleTheme.EstilizarEntrada(txtParHora);
        AppleTheme.AgregarCampo(form, 0, "Id", numParId);
        AppleTheme.AgregarCampo(form, 1, "Rival", txtParRival);
        AppleTheme.AgregarCampo(form, 2, "Fecha", dtpParFecha);
        // Fila compuesta: hora y checkbox activo en la misma fila del formulario.
        var filaHora = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppleTheme.Tarjeta,
            Margin = new Padding(0, 4, AppleTheme.MargenDerechoCampo, 4)
        };
        filaHora.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        filaHora.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        txtParHora.Dock = DockStyle.Fill;
        chkParActivo.Dock = DockStyle.Fill;
        filaHora.Controls.Add(txtParHora, 0, 0);
        filaHora.Controls.Add(chkParActivo, 1, 0);
        AppleTheme.AgregarCampo(form, 3, "Hora / Activo", filaHora);
        AppleTheme.AgregarBoton(form, 4, btnRegistrarPartido);
        AppleTheme.AgregarContenido(cuerpo, form);
        return tarjeta;
    }

    /// <summary>
    /// Construye la tarjeta visual de registro de vendedores con datos personales y fechas.
    /// </summary>
    /// <returns>Panel tipo tarjeta listo para insertar en el grid de registros.</returns>
    private Panel CrearTarjetaVendedor()
    {
        var tarjeta = AppleTheme.CrearTarjeta("Vendedor", 6);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var form = AppleTheme.CrearFormulario(6);
        numVenId = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1 };
        txtVenIdentificacion = new TextBox();
        txtVenNombre = new TextBox();
        txtVenApellido = new TextBox();
        dtpVenNacimiento = new DateTimePicker { Value = DateTime.Today.AddYears(-25) };
        dtpVenIngreso = new DateTimePicker { Value = DateTime.Today };
        btnRegistrarVendedor = new Button { Text = "Guardar" };
        btnRegistrarVendedor.Click += btnRegistrarVendedor_Click;
        AppleTheme.EstilizarNumerico(numVenId);
        AppleTheme.EstilizarEntrada(txtVenIdentificacion);
        AppleTheme.EstilizarEntrada(txtVenNombre);
        AppleTheme.EstilizarEntrada(txtVenApellido);
        AppleTheme.EstilizarFecha(dtpVenNacimiento);
        AppleTheme.EstilizarFecha(dtpVenIngreso);
        AppleTheme.AgregarCampo(form, 0, "Id", numVenId);
        AppleTheme.AgregarCampo(form, 1, "Identificación", txtVenIdentificacion);
        AppleTheme.AgregarCampo(form, 2, "Nombre", txtVenNombre);
        AppleTheme.AgregarCampo(form, 3, "Apellido", txtVenApellido);
        AppleTheme.AgregarCampo(form, 4, "Fecha nac.", dtpVenNacimiento);
        AppleTheme.AgregarCampo(form, 5, "Fecha ingreso", dtpVenIngreso);
        AppleTheme.AgregarBoton(form, 6, btnRegistrarVendedor);
        AppleTheme.AgregarContenido(cuerpo, form);
        return tarjeta;
    }

    /// <summary>
    /// Construye la tarjeta visual de registro de clientes con identificación, fechas y estado activo.
    /// </summary>
    /// <returns>Panel tipo tarjeta listo para insertar en el grid de registros.</returns>
    private Panel CrearTarjetaCliente()
    {
        var tarjeta = AppleTheme.CrearTarjeta("Cliente", 7);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var form = AppleTheme.CrearFormulario(7);

        numCliId = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1 };
        txtCliIdentificacion = new TextBox();
        txtCliNombre = new TextBox();
        txtCliApellido = new TextBox();
        dtpCliNacimiento = new DateTimePicker { Value = DateTime.Today.AddYears(-20) };
        dtpCliRegistro = new DateTimePicker { Value = DateTime.Today };
        chkCliActivo = new CheckBox { Text = "Activo", Checked = true, ForeColor = AppleTheme.Texto, Dock = DockStyle.Left };
        btnRegistrarCliente = new Button { Text = "Guardar" };
        btnRegistrarCliente.Click += btnRegistrarCliente_Click;
        AppleTheme.EstilizarNumerico(numCliId);
        AppleTheme.EstilizarEntrada(txtCliIdentificacion);
        AppleTheme.EstilizarEntrada(txtCliNombre);
        AppleTheme.EstilizarEntrada(txtCliApellido);
        AppleTheme.EstilizarFecha(dtpCliNacimiento);
        AppleTheme.EstilizarFecha(dtpCliRegistro);
        AppleTheme.AgregarCampo(form, 0, "Id", numCliId);
        AppleTheme.AgregarCampo(form, 1, "Identificación", txtCliIdentificacion);
        AppleTheme.AgregarCampo(form, 2, "Nombre", txtCliNombre);
        AppleTheme.AgregarCampo(form, 3, "Apellido", txtCliApellido);
        AppleTheme.AgregarCampo(form, 4, "Fecha nac.", dtpCliNacimiento);
        AppleTheme.AgregarCampo(form, 5, "Fecha registro", dtpCliRegistro);
        AppleTheme.AgregarCampo(form, 6, "Estado", chkCliActivo);
        AppleTheme.AgregarBoton(form, 7, btnRegistrarCliente);
        AppleTheme.AgregarContenido(cuerpo, form);
        return tarjeta;
    }

    /// <summary>
    /// Construye la tarjeta de asociación localidad-partido con combos y etiqueta informativa.
    /// </summary>
    /// <returns>Panel tipo tarjeta que ocupa el ancho completo de la fila inferior del grid.</returns>
    private Panel CrearTarjetaLp()
    {
        var tarjeta = AppleTheme.CrearTarjeta("Localidad por partido", 4, alturaExtra: 28);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AppleTheme.AlturaFormulario(4) + 28,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppleTheme.Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFormulario(4)));

        lblLpInfoPartido = new Label
        {
            Text = "Seleccione un partido para ver fecha y hora",
            ForeColor = AppleTheme.TextoSecundario,
            BackColor = AppleTheme.Tarjeta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(lblLpInfoPartido, 0, 0);

        var form = AppleTheme.CrearFormulario(4);
        numLpId = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 1 };
        cboLpPartido = new ComboBox();
        cboLpLocalidad = new ComboBox();
        numLpCantidad = new NumericUpDown { Minimum = 1, Maximum = 99999, Value = 10 };
        btnRegistrarLp = new Button { Text = "Guardar asociación" };
        cboLpPartido.SelectedIndexChanged += cboLpPartido_SelectedIndexChanged;
        btnRegistrarLp.Click += btnRegistrarLp_Click;
        AppleTheme.EstilizarNumerico(numLpId);
        AppleTheme.EstilizarCombo(cboLpPartido);
        AppleTheme.EstilizarCombo(cboLpLocalidad);
        AppleTheme.EstilizarNumerico(numLpCantidad);
        AppleTheme.AgregarCampo(form, 0, "Id", numLpId);
        AppleTheme.AgregarCampo(form, 1, "Partido", cboLpPartido);
        AppleTheme.AgregarCampo(form, 2, "Localidad", cboLpLocalidad);
        AppleTheme.AgregarCampo(form, 3, "Cantidad", numLpCantidad);
        AppleTheme.AgregarBoton(form, 4, btnRegistrarLp);
        layout.Controls.Add(form, 0, 1);
        AppleTheme.AgregarContenido(cuerpo, layout);
        return tarjeta;
    }

    /// <summary>
    /// Crea la pestaña Consultas con sub-pestañas y una grilla por entidad.
    /// </summary>
    private void CrearTabConsultas()
    {
        tabConsultasInner = new TabControl();
        AppleTheme.EstilizarTabControl(tabConsultasInner);
        dgvLocalidades = CrearGridConsulta("Localidades", btnConsultarLocalidades_Click);
        dgvPartidos = CrearGridConsulta("Partidos", btnConsultarPartidos_Click);
        dgvVendedores = CrearGridConsulta("Vendedores", btnConsultarVendedores_Click);
        dgvClientes = CrearGridConsulta("Clientes", btnConsultarClientes_Click);
        dgvLp = CrearGridConsulta("Localidad / Partido", btnConsultarLp_Click);
        dgvVentas = CrearGridConsulta("Ventas", btnConsultarVentas_Click);
        tabConsultas.Controls.Add(tabConsultasInner);
        tabPrincipal.TabPages.Add(tabConsultas);
    }

    /// <summary>
    /// Crea una sub-pestaña de consulta con barra de herramientas y grilla estilizada.
    /// </summary>
    /// <param name="titulo">Texto de la pestaña y etiqueta descriptiva.</param>
    /// <param name="handlerActualizar">Handler del botón Actualizar (consulta en Form1.cs).</param>
    /// <returns>Instancia del DataGridView creado para enlazar datos.</returns>
    private DataGridView CrearGridConsulta(string titulo, EventHandler handlerActualizar)
    {
        var pagina = new TabPage(titulo) { BackColor = AppleTheme.Fondo };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, BackColor = AppleTheme.Fondo, Padding = new Padding(4) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
        var btnActualizar = new Button { Text = "Actualizar", Width = 110, Height = 32 };
        // Handler de consulta: recarga datos desde lógica de negocio y actualiza la grilla.
        btnActualizar.Click += handlerActualizar;
        AppleTheme.EstilizarBotonSecundario(btnActualizar);
        barra.Controls.Add(btnActualizar);
        barra.Controls.Add(new Label
        {
            Text = titulo,
            AutoSize = true,
            ForeColor = AppleTheme.TextoSecundario,
            Font = AppleTheme.FuenteCuerpo,
            Padding = new Padding(12, 8, 0, 0)
        });

        var grid = new DataGridView();
        AppleTheme.EstilizarGrid(grid);
        layout.Controls.Add(barra, 0, 0);
        layout.Controls.Add(grid, 0, 1);
        pagina.Controls.Add(layout);
        tabConsultasInner.TabPages.Add(pagina);
        return grid;
    }

    /// <summary>
    /// Crea la pestaña Venta boletería con formulario de venta física y cálculo de monto.
    /// </summary>
    private void CrearTabVenta()
    {
        var scroll = AppleTheme.CrearPanelScroll();

        var tarjeta = AppleTheme.CrearTarjeta("Venta en boletería física", 6, alturaExtra: 28, apilada: true);
        tarjeta.Margin = new Padding(8, 8, 8, 16);
        var cuerpo = AppleTheme.AgregarCuerpoTarjeta(tarjeta);
        var contenedorForm = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AppleTheme.AlturaFormulario(6) + 28,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppleTheme.Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        contenedorForm.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFormulario(6)));
        contenedorForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        var form = AppleTheme.CrearFormulario(6);

        cboVentaCliente = new ComboBox();
        cboVentaVendedor = new ComboBox();
        cboVentaPartido = new ComboBox();
        cboVentaLocalidad = new ComboBox();
        numVentaCantidad = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1 };
        // Campo de monto calculado: solo lectura con fondo ligeramente distinto.
        txtVentaMonto = new TextBox { ReadOnly = true, BackColor = Color.FromArgb(250, 250, 252) };
        lblVentaInfoPartido = new Label { Text = "Seleccione un partido", ForeColor = AppleTheme.TextoSecundario, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        lblVentaDisponible = new Label { Text = "Disponibles: -", ForeColor = AppleTheme.TextoSecundario, BackColor = AppleTheme.Tarjeta, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        btnRegistrarVenta = new Button { Text = "Registrar venta" };
        cboVentaPartido.SelectedIndexChanged += cboVentaPartido_SelectedIndexChanged;
        cboVentaLocalidad.SelectedIndexChanged += cboVentaLocalidad_SelectedIndexChanged;
        numVentaCantidad.ValueChanged += numVentaCantidad_ValueChanged;
        btnRegistrarVenta.Click += btnRegistrarVenta_Click;
        AppleTheme.EstilizarCombo(cboVentaCliente);
        AppleTheme.EstilizarCombo(cboVentaVendedor);
        AppleTheme.EstilizarCombo(cboVentaPartido);
        AppleTheme.EstilizarCombo(cboVentaLocalidad);
        AppleTheme.EstilizarEntrada(txtVentaMonto);

        AppleTheme.AgregarCampo(form, 0, "Cliente", cboVentaCliente);
        AppleTheme.AgregarCampo(form, 1, "Vendedor", cboVentaVendedor);
        AppleTheme.AgregarCampo(form, 2, "Partido", cboVentaPartido);
        AppleTheme.AgregarCampo(form, 3, "Localidad", cboVentaLocalidad);
        // Fila compuesta: cantidad numérica y etiqueta de disponibilidad en la misma fila.
        var filaCantidad = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppleTheme.Tarjeta,
            Margin = new Padding(0, 4, 0, 4)
        };
        filaCantidad.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        filaCantidad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        numVentaCantidad.Dock = DockStyle.Fill;
        lblVentaDisponible.Dock = DockStyle.Fill;
        lblVentaDisponible.TextAlign = ContentAlignment.MiddleLeft;
        filaCantidad.Controls.Add(numVentaCantidad, 0, 0);
        filaCantidad.Controls.Add(lblVentaDisponible, 1, 0);
        AppleTheme.AgregarCampo(form, 4, "Cantidad", filaCantidad);
        AppleTheme.AgregarCampo(form, 5, "Monto total", txtVentaMonto);
        AppleTheme.AgregarBoton(form, 6, btnRegistrarVenta);

        contenedorForm.Controls.Add(form, 0, 0);
        contenedorForm.Controls.Add(lblVentaInfoPartido, 0, 1);
        AppleTheme.AgregarContenido(cuerpo, contenedorForm);
        AppleTheme.EnlazarContenidoScroll(scroll, tarjeta, AppleTheme.AlturaTarjeta(6, alturaExtra: 28));
        tabVenta.Controls.Add(scroll);
        tabPrincipal.TabPages.Add(tabVenta);
    }
}
