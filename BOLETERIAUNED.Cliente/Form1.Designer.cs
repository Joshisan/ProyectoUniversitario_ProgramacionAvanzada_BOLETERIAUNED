/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Diseño del formulario cliente — layout responsivo pantalla completa con tarjetas temáticas.
 *              Define controles, disposición visual y enlaces a eventos de lógica en Form1.cs.
 */

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Parte parcial de Form1 responsable únicamente del diseño visual y la construcción de controles.
/// </summary>
partial class Form1
{
    // --- Controles de la tarjeta Conexión TCP ---
    private Button btnConectar;
    private Label lblEstadoConexion;

    // --- Controles de la tarjeta Identificación / autenticación ---
    private TextBox txtIdentificacion;
    private TextBox txtIdCliente;
    private Button btnValidarCliente;
    private Label lblClienteAutenticado;

    // --- Controles de la tarjeta Comprar entradas ---
    private ComboBox cboCompraPartido;
    private ComboBox cboCompraLocalidad;
    private NumericUpDown numCompraCantidad;
    private TextBox txtCompraMonto;
    private Label lblCompraInfoPartido;
    private Label lblCompraDisponible;
    private Button btnComprar;

    // --- Controles de la tarjeta Mis compras ---
    private DataGridView dgvCompras;
    private Button btnConsultarCompras;
    private Label lblConsultaInfo;

    // --- Contenedores de layout responsivo ---
    private Panel _layoutTarjetas;
    private Panel _tarjetaConexion;
    private Label _lblTituloConexion;
    private Panel _tarjetaIdentificacion;
    private Label _lblTituloIdentificacion;

    /// <summary>
    /// Construye la jerarquía visual completa: shell, scroll, tarjetas apiladas y sus controles hijos.
    /// </summary>
    private void InitializeComponent()
    {
        SuspendLayout();
        FormClosing += Form1_FormClosing;

        // Ventana maximizada con título y tamaño mínimo para legibilidad en distintas resoluciones.
        AppleTheme.ConfigurarVentanaCompleta(this, "Boletaría UNED - Cliente");

        // Shell: encabezado fijo + área de contenido con scroll vertical.
        var shell = AppleTheme.CrearShell(
            "Boletaría UNED",
            "Compra de entradas en línea | TCP 127.0.0.1:14500",
            out var areaContenido);

        var scroll = AppleTheme.CrearPanelScroll();
        _layoutTarjetas = new Panel
        {
            Dock = DockStyle.Top,
            BackColor = AppleTheme.Fondo
        };

        // Orden vertical: conexión → autenticación → compra → consulta de historial.
        _layoutTarjetas.Controls.Add(CrearTarjetaConexion());
        _layoutTarjetas.Controls.Add(CrearTarjetaAutenticacion());
        _layoutTarjetas.Controls.Add(CrearTarjetaCompra());
        _layoutTarjetas.Controls.Add(CrearTarjetaConsulta());

        ActualizarAlturaLayout();
        AppleTheme.EnlazarContenidoScroll(scroll, _layoutTarjetas);
        areaContenido.Controls.Add(scroll);
        Controls.Add(shell);

        // Recalcula alturas al redimensionar la ventana para mantener scroll correcto.
        Resize += (_, _) => ActualizarAlturaLayout();
        Shown += (_, _) =>
        {
            ActualizarAlturaLayout();
            AppleTheme.RefrescarScrolls(this);
        };
        ResumeLayout(true);
    }

    /// <summary>
    /// Ajusta la altura del panel apilado según la suma de alturas de las tarjetas hijas.
    /// </summary>
    private void ActualizarAlturaLayout()
    {
        var altura = AppleTheme.CalcularAlturaContenedorApilado(_layoutTarjetas);
        _layoutTarjetas.Height = altura;
    }

    /// <summary>
    /// Crea la tarjeta superior con botón Conectar/Desconectar e indicador de estado TCP.
    /// </summary>
    private Panel CrearTarjetaConexion()
    {
        const int altoBarra = 52;
        _tarjetaConexion = AppleTheme.CrearTarjetaApilada("Conexión", altoBarra, out var cuerpo, out _lblTituloConexion);

        btnConectar = new Button { Text = "Conectar al servidor" };
        lblEstadoConexion = new Label { Text = "Desconectado" };
        btnConectar.Click += btnConectar_Click;

        var barra = AppleTheme.CrearBarraConexion(btnConectar, lblEstadoConexion);
        AppleTheme.EstilizarEstado(lblEstadoConexion, false);
        cuerpo.Controls.Add(barra);
        return _tarjetaConexion;
    }

    /// <summary>
    /// Crea la tarjeta de identificación con campos de cédula/Id y botón Validar cliente.
    /// </summary>
    private Panel CrearTarjetaAutenticacion()
    {
        const int filasFormulario = 2;
        const int altoEstado = 28;
        var alturaContenido = AppleTheme.AlturaFormulario(filasFormulario) + altoEstado;
        _tarjetaIdentificacion = AppleTheme.CrearTarjetaApilada(
            "Identificación",
            alturaContenido,
            out var cuerpo,
            out _lblTituloIdentificacion);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppleTheme.Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFormulario(filasFormulario)));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, altoEstado));

        // Formulario de tres columnas: etiqueta | campo | espacio/botón.
        var form = AppleTheme.CrearFormularioTresColumnas(filasFormulario);
        form.Dock = DockStyle.Fill;

        txtIdentificacion = new TextBox();
        txtIdCliente = new TextBox();
        btnValidarCliente = new Button { Text = "Validar cliente" };
        btnValidarCliente.Click += btnValidarCliente_Click;
        AppleTheme.EstilizarEntrada(txtIdentificacion);
        AppleTheme.EstilizarEntrada(txtIdCliente);
        AppleTheme.AgregarCampo(form, 0, "Identificación", txtIdentificacion);
        AppleTheme.AgregarCampo(form, 1, "Id cliente", txtIdCliente);
        AppleTheme.AgregarBoton(form, 2, btnValidarCliente);

        // Etiqueta de estado de autenticación (Sin autenticar / Bienvenido / Validación fallida).
        lblClienteAutenticado = new Label
        {
            Text = "Sin autenticar",
            ForeColor = AppleTheme.TextoSecundario,
            Font = AppleTheme.FuenteSeccion,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(AppleTheme.AnchoEtiquetaFormulario, 0, 0, 0)
        };

        layout.Controls.Add(form, 0, 0);
        layout.Controls.Add(lblClienteAutenticado, 0, 1);
        cuerpo.Controls.Add(layout);
        return _tarjetaIdentificacion;
    }

    /// <summary>
    /// Crea la tarjeta de compra en línea: partido, localidad, cantidad, monto y botón confirmar.
    /// </summary>
    private Panel CrearTarjetaCompra()
    {
        const int filasFormulario = 4;
        var tarjeta = AppleTheme.CrearTarjeta("Comprar entradas", filasFormulario, alturaExtra: 28);
        var cuerpo = AppleTheme.AgregarCuerpo(tarjeta);
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AppleTheme.AlturaFormulario(filasFormulario) + 28,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppleTheme.Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AlturaFormulario(filasFormulario)));
        contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));

        // Grid 3 columnas: etiqueta | control principal | información auxiliar (disponibles, etc.).
        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 5,
            BackColor = AppleTheme.Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AppleTheme.AnchoEtiquetaFormulario));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        for (var i = 0; i < 4; i++) form.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AltoFilaFormulario));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, AppleTheme.AltoBotonFormulario));

        cboCompraPartido = new ComboBox();
        cboCompraLocalidad = new ComboBox();
        numCompraCantidad = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1 };
        txtCompraMonto = new TextBox { ReadOnly = true, BackColor = Color.FromArgb(250, 250, 252) };
        lblCompraDisponible = new Label
        {
            Text = "Disponibles: -",
            ForeColor = AppleTheme.TextoSecundario,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        btnComprar = new Button { Text = "Confirmar compra" };

        // Eventos enlazados a la lógica de compra en Form1.cs.
        cboCompraPartido.SelectedIndexChanged += cboCompraPartido_SelectedIndexChanged;
        cboCompraLocalidad.SelectedIndexChanged += cboCompraLocalidad_SelectedIndexChanged;
        numCompraCantidad.ValueChanged += numCompraCantidad_ValueChanged;
        btnComprar.Click += btnComprar_Click;

        AppleTheme.EstilizarCombo(cboCompraPartido);
        AppleTheme.EstilizarCombo(cboCompraLocalidad);
        AppleTheme.EstilizarEntrada(txtCompraMonto);
        AppleTheme.EstilizarBotonPrimario(btnComprar);
        btnComprar.Dock = DockStyle.Left;
        btnComprar.Width = 170;
        btnComprar.Height = 34;

        AgregarCampoCompra(form, 0, "Partido", cboCompraPartido, new Panel { Dock = DockStyle.Fill, BackColor = AppleTheme.Tarjeta });
        AgregarCampoCompra(form, 1, "Localidad", cboCompraLocalidad, new Panel { Dock = DockStyle.Fill, BackColor = AppleTheme.Tarjeta });
        AgregarCampoCompra(form, 2, "Cantidad", numCompraCantidad, lblCompraDisponible);
        AgregarCampoCompra(form, 3, "Monto total", txtCompraMonto, new Panel { Dock = DockStyle.Fill, BackColor = AppleTheme.Tarjeta });
        form.Controls.Add(btnComprar, 1, 4);
        form.SetColumnSpan(btnComprar, 2);

        lblCompraInfoPartido = new Label
        {
            Text = "Seleccione un partido para ver fecha y hora",
            ForeColor = AppleTheme.TextoSecundario,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        contenedor.Controls.Add(form, 0, 0);
        contenedor.Controls.Add(lblCompraInfoPartido, 0, 1);
        AppleTheme.AgregarContenido(cuerpo, contenedor);
        return tarjeta;
    }

    /// <summary>
    /// Agrega una fila al formulario de compra: etiqueta, control principal y control auxiliar en columna 3.
    /// </summary>
    private static void AgregarCampoCompra(TableLayoutPanel form, int fila, string etiqueta, Control control, Control extra)
    {
        form.Controls.Add(new Label
        {
            Text = etiqueta,
            Font = AppleTheme.FuenteCuerpo,
            ForeColor = AppleTheme.TextoSecundario,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, fila);
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 4, 8, 4);
        form.Controls.Add(control, 1, fila);
        extra.Dock = DockStyle.Fill;
        extra.Margin = new Padding(0, 4, 0, 4);
        form.Controls.Add(extra, 2, fila);
    }

    /// <summary>
    /// Crea la tarjeta inferior con botón Actualizar, resumen y grid de historial de compras.
    /// </summary>
    private Panel CrearTarjetaConsulta()
    {
        var tarjeta = AppleTheme.CrearTarjetaApilada("Mis compras", AppleTheme.AltoBarraCompras + AppleTheme.AltoMinimoGridCompras);
        var cuerpo = AppleTheme.AgregarCuerpo(tarjeta);
        var contenedor = new Panel
        {
            Dock = DockStyle.Top,
            Height = AppleTheme.AltoBarraCompras + AppleTheme.AltoMinimoGridCompras,
            BackColor = AppleTheme.Tarjeta,
            MinimumSize = new Size(0, AppleTheme.AltoBarraCompras + AppleTheme.AltoMinimoGridCompras)
        };

        var barra = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AppleTheme.AltoBarraCompras,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 4, 0, 4)
        };
        btnConsultarCompras = new Button { Text = "Actualizar", Width = 120, Height = 34, Margin = new Padding(0, 0, 12, 4) };
        lblConsultaInfo = new Label
        {
            Text = "Consulte las entradas adquiridas con su cuenta",
            AutoSize = true,
            ForeColor = AppleTheme.TextoSecundario,
            Padding = new Padding(0, 8, 0, 4),
            MaximumSize = new Size(720, 0)
        };
        btnConsultarCompras.Click += btnConsultarCompras_Click;
        AppleTheme.EstilizarBotonSecundario(btnConsultarCompras);
        barra.Controls.Add(btnConsultarCompras);
        barra.Controls.Add(lblConsultaInfo);

        dgvCompras = new DataGridView();
        AppleTheme.ConfigurarGridCompras(dgvCompras);
        dgvCompras.Dock = DockStyle.Fill;

        contenedor.Controls.Add(dgvCompras);
        contenedor.Controls.Add(barra);
        AppleTheme.AgregarContenido(cuerpo, contenedor);
        return tarjeta;
    }
}
