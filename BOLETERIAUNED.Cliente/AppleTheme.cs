/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Tema visual y helpers de layout para la UI del cliente (paleta, tarjetas, scroll, grids).
 *              Proporciona apariencia consistente y layout responsivo en pantalla completa.
 */

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Utilidades estáticas de presentación: colores, fuentes, tarjetas apiladas, formularios y estilos de controles.
/// Inspirado en diseño limpio tipo Apple para mejorar legibilidad y contraste en WinForms.
/// </summary>
public static class AppleTheme
{
    // --- Paleta de colores de la interfaz ---

    /// <summary>Fondo general de la ventana y paneles con scroll.</summary>
    public static readonly Color Fondo = Color.FromArgb(245, 245, 247);

    /// <summary>Fondo de tarjetas y controles sobre el fondo gris claro.</summary>
    public static readonly Color Tarjeta = Color.White;

    /// <summary>Color principal del texto.</summary>
    public static readonly Color Texto = Color.FromArgb(20, 20, 22);

    /// <summary>Color para etiquetas secundarias y textos de ayuda.</summary>
    public static readonly Color TextoSecundario = Color.FromArgb(70, 70, 76);

    /// <summary>Color de acento para botones primarios (Conectar, Validar, Comprar).</summary>
    public static readonly Color Acento = Color.FromArgb(0, 113, 227);

    /// <summary>Color de acento al pasar el mouse sobre botones primarios.</summary>
    public static readonly Color AcentoHover = Color.FromArgb(0, 91, 196);

    /// <summary>Color de bordes de tarjetas y separadores.</summary>
    public static readonly Color Borde = Color.FromArgb(190, 190, 198);

    /// <summary>Fondo del encabezado del DataGridView.</summary>
    public static readonly Color EncabezadoGrid = Color.FromArgb(235, 235, 240);

    /// <summary>Color verde para estados exitosos (conectado, autenticado).</summary>
    public static readonly Color Exito = Color.FromArgb(28, 140, 55);

    /// <summary>Color rojo para estados de error o desconexión.</summary>
    public static readonly Color Error = Color.FromArgb(200, 30, 30);

    // --- Constantes de dimensiones de formularios ---

    /// <summary>Altura estándar de cada fila de campo en formularios.</summary>
    public const int AltoFilaFormulario = 40;

    /// <summary>Altura reservada para la fila del botón en formularios.</summary>
    public const int AltoBotonFormulario = 46;

    /// <summary>Ancho fijo de la columna de etiquetas en formularios.</summary>
    public const int AnchoEtiquetaFormulario = 145;

    /// <summary>Fuente del título principal del encabezado de la ventana.</summary>
    public static Font FuenteTitulo => new("Segoe UI", 18F, FontStyle.Bold);

    /// <summary>Fuente del subtítulo bajo el título (muestra info TCP, etc.).</summary>
    public static Font FuenteSubtitulo => new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>Fuente de títulos de sección/tarjetas.</summary>
    public static Font FuenteSeccion => new("Segoe UI", 12F, FontStyle.Bold);

    /// <summary>Fuente de cuerpo para etiquetas, entradas y textos generales.</summary>
    public static Font FuenteCuerpo => new("Segoe UI", 10F, FontStyle.Regular);

    /// <summary>Fuente de botones primarios.</summary>
    public static Font FuenteBoton => new("Segoe UI", 10F, FontStyle.Bold);

    /// <summary>Fuente del encabezado de columnas del grid de compras.</summary>
    public static Font FuenteGridEncabezado => new("Segoe UI", 9.5F, FontStyle.Bold);

    /// <summary>
    /// Configura propiedades globales del formulario: título, colores, fuente y ventana maximizada.
    /// </summary>
    /// <param name="formulario">Formulario principal a estilizar.</param>
    /// <param name="tituloVentana">Texto de la barra de título de la ventana.</param>
    public static void ConfigurarVentanaCompleta(Form formulario, string tituloVentana)
    {
        formulario.Text = tituloVentana;
        formulario.BackColor = Fondo;
        formulario.Font = FuenteCuerpo;
        formulario.ForeColor = Texto;
        formulario.StartPosition = FormStartPosition.CenterScreen;
        formulario.WindowState = FormWindowState.Maximized;
        formulario.MinimumSize = new Size(900, 600);
    }

    /// <summary>
    /// Crea un panel con scroll vertical para contenido que excede la altura de la ventana.
    /// </summary>
    public static Panel CrearPanelScroll()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Fondo
        };
    }

    /// <summary>
    /// Enlaza un control hijo al panel scroll: ajusta ancho/alto y recalcula AutoScrollMinSize al redimensionar.
    /// </summary>
    /// <param name="scroll">Panel contenedor con AutoScroll.</param>
    /// <param name="contenido">Control apilado (Dock Top) cuya altura se calcula dinámicamente.</param>
    /// <param name="altoMinimo">Altura mínima opcional para el contenido.</param>
    public static void EnlazarContenidoScroll(Panel scroll, Control contenido, int? altoMinimo = null)
    {
        contenido.Dock = DockStyle.Top;
        scroll.Controls.Add(contenido);

        void refrescar()
        {
            // Descuenta el ancho de la barra de scroll vertical si está visible.
            var barra = scroll.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            var ancho = scroll.ClientSize.Width - barra - scroll.Padding.Horizontal;
            if (ancho > 0)
            {
                contenido.Width = ancho;
            }

            contenido.PerformLayout();
            var alto = CalcularAlturaContenedorApilado(contenido);
            if (alto <= 0)
            {
                alto = contenido.GetPreferredSize(new Size(Math.Max(1, contenido.Width), 0)).Height;
            }

            if (altoMinimo.HasValue)
            {
                alto = Math.Max(altoMinimo.Value, alto);
            }

            contenido.Height = alto;
            scroll.AutoScrollMinSize = new Size(0, alto + scroll.Padding.Vertical + 16);
            scroll.PerformLayout();
        }

        scroll.Resize += (_, _) => refrescar();
        contenido.Layout += (_, _) => refrescar();
        scroll.HandleCreated += (_, _) => refrescar();
        contenido.HandleCreated += (_, _) => refrescar();
        refrescar();
    }

    /// <summary>
    /// Calcula la altura total de un formulario según número de filas y si incluye fila de botón.
    /// </summary>
    public static int AlturaFormulario(int filas, bool incluirBoton = true)
    {
        var alturaFilas = filas * AltoFilaFormulario;
        var alturaBoton = incluirBoton ? AltoBotonFormulario : 0;
        return alturaFilas + alturaBoton;
    }

    /// <summary>Altura mínima del grid de compras para mostrar varias filas legibles.</summary>
    public const int AltoMinimoGridCompras = 220;

    /// <summary>Altura de la barra superior del grid (botón Actualizar + resumen).</summary>
    public const int AltoBarraCompras = 48;

    /// <summary>Altura reservada para el título de cada tarjeta.</summary>
    public const int AltoTituloTarjeta = 32;

    /// <summary>Altura de la barra de conexión (botón + estado).</summary>
    public const int AltoBarraConexion = 52;

    /// <summary>Margen derecho entre campos en formularios de tres columnas.</summary>
    public const int MargenDerechoCampo = 8;

    /// <summary>
    /// Calcula altura total de una tarjeta a partir de filas de formulario y espacio extra opcional.
    /// </summary>
    public static int AlturaTarjeta(int filasFormulario, bool incluirBoton = true, int alturaExtra = 0)
    {
        const int paddingTarjeta = 24;
        const int paddingCuerpo = 4;
        return AltoTituloTarjeta + paddingCuerpo + AlturaFormulario(filasFormulario, incluirBoton) + alturaExtra + paddingTarjeta;
    }

    /// <summary>
    /// Calcula altura de tarjeta cuando el contenido tiene altura fija conocida (p. ej. grid + barra).
    /// </summary>
    public static int AlturaTarjetaContenido(int alturaContenido)
    {
        const int paddingTarjeta = 24;
        const int paddingCuerpo = 4;
        return AltoTituloTarjeta + paddingCuerpo + alturaContenido + paddingTarjeta;
    }

    /// <summary>Altura predefinida para la tarjeta de historial de compras.</summary>
    public static int AlturaTarjetaCompras()
    {
        return AlturaTarjetaContenido(AltoBarraCompras + AltoMinimoGridCompras);
    }

    /// <summary>Margen inferior entre tarjetas apiladas verticalmente.</summary>
    public const int MargenTarjetaApilada = 12;

    /// <summary>
    /// Suma las alturas de controles hijos con Dock Top dentro de un contenedor apilado.
    /// </summary>
    public static int CalcularAlturaContenedorApilado(Control contenedor)
    {
        var alto = contenedor.Padding.Vertical;
        foreach (Control hijo in contenedor.Controls)
        {
            if (hijo.Dock == DockStyle.Top)
            {
                alto += hijo.Height + hijo.Margin.Vertical;
            }
        }

        return alto + 16;
    }

    /// <summary>
    /// Crea el layout principal: encabezado fijo (título + subtítulo) y área de contenido expandible.
    /// </summary>
    /// <param name="titulo">Título principal visible en el encabezado.</param>
    /// <param name="subtitulo">Subtítulo (p. ej. dirección TCP del servidor).</param>
    /// <param name="areaContenido">Panel de salida donde se coloca el scroll con tarjetas.</param>
    public static TableLayoutPanel CrearShell(string titulo, string subtitulo, out Panel areaContenido)
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Fondo
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var encabezado = new Panel { Dock = DockStyle.Fill, BackColor = Tarjeta, Padding = new Padding(24, 14, 24, 10) };
        encabezado.Paint += (_, e) =>
        {
            using var pen = new Pen(Borde);
            e.Graphics.DrawLine(pen, 0, encabezado.Height - 1, encabezado.Width, encabezado.Height - 1);
        };
        encabezado.Controls.Add(new Label
        {
            Text = titulo,
            Font = FuenteTitulo,
            ForeColor = Texto,
            BackColor = Tarjeta,
            AutoSize = true,
            Location = new Point(20, 8)
        });
        encabezado.Controls.Add(new Label
        {
            Text = subtitulo,
            Font = FuenteSubtitulo,
            ForeColor = TextoSecundario,
            BackColor = Tarjeta,
            AutoSize = true,
            Location = new Point(22, 38)
        });

        areaContenido = new Panel { Dock = DockStyle.Fill, BackColor = Fondo, Padding = new Padding(16) };

        shell.Controls.Add(encabezado, 0, 0);
        shell.Controls.Add(areaContenido, 0, 1);
        return shell;
    }

    /// <summary>
    /// Crea una tarjeta con altura derivada del número de filas del formulario interno.
    /// </summary>
    public static Panel CrearTarjeta(string titulo, int filasFormulario, bool incluirBoton = true, int alturaExtra = 0)
    {
        var alturaContenido = AlturaFormulario(filasFormulario, incluirBoton) + alturaExtra;
        return CrearTarjetaApilada(titulo, alturaContenido);
    }

    /// <summary>
    /// Sobrecarga simplificada que crea tarjeta apilada con altura de contenido fija.
    /// </summary>
    public static Panel CrearTarjetaApilada(string titulo, int alturaContenido)
    {
        return CrearTarjetaApilada(titulo, alturaContenido, out _, out _);
    }

    /// <summary>
    /// Construye una tarjeta visual con borde, título y panel cuerpo para controles hijos.
    /// </summary>
    /// <param name="titulo">Texto del encabezado de la tarjeta.</param>
    /// <param name="alturaContenido">Altura del área de contenido debajo del título.</param>
    /// <param name="cuerpo">Panel de salida donde agregar controles del formulario.</param>
    /// <param name="lblTitulo">Etiqueta del título de la tarjeta (referencia opcional).</param>
    public static Panel CrearTarjetaApilada(string titulo, int alturaContenido, out Panel cuerpo, out Label lblTitulo)
    {
        var altura = AlturaTarjetaContenido(alturaContenido);
        var tarjeta = new Panel
        {
            Dock = DockStyle.Top,
            Height = altura,
            MinimumSize = new Size(0, altura),
            BackColor = Tarjeta,
            Margin = new Padding(0, 0, 0, MargenTarjetaApilada),
            Padding = new Padding(16, 12, 16, 12)
        };

        // Dibuja borde rectangular alrededor de la tarjeta.
        tarjeta.Paint += (_, e) =>
        {
            var rect = new Rectangle(0, 0, tarjeta.Width - 1, tarjeta.Height - 1);
            using var pen = new Pen(Borde);
            e.Graphics.DrawRectangle(pen, rect);
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Tarjeta,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoTituloTarjeta));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, alturaContenido + 4F));

        lblTitulo = new Label
        {
            Text = titulo,
            Font = FuenteSeccion,
            ForeColor = Texto,
            BackColor = Tarjeta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        cuerpo = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Tarjeta,
            Padding = new Padding(0, 4, MargenDerechoCampo, 0)
        };

        layout.Controls.Add(lblTitulo, 0, 0);
        layout.Controls.Add(cuerpo, 0, 1);
        tarjeta.Controls.Add(layout);
        return tarjeta;
    }

    /// <summary>
    /// Obtiene o crea el panel cuerpo dentro de una tarjeta para agregar contenido adicional.
    /// </summary>
    public static Panel AgregarCuerpo(Panel tarjeta)
    {
        if (ObtenerCuerpoTarjeta(tarjeta) is { } existente)
        {
            EnlazarAnchoContenedor(existente);
            return existente;
        }

        var cuerpo = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Tarjeta,
            Padding = new Padding(0, 4, MargenDerechoCampo, 0)
        };
        tarjeta.Controls.Add(cuerpo);
        EnlazarAnchoContenedor(cuerpo);
        return cuerpo;
    }

    /// <summary>
    /// Busca el panel cuerpo dentro de la estructura TableLayoutPanel de una tarjeta creada con CrearTarjetaApilada.
    /// </summary>
    public static Panel? ObtenerCuerpoTarjeta(Panel tarjeta)
    {
        foreach (Control control in tarjeta.Controls)
        {
            if (control is TableLayoutPanel layout && layout.Controls.Count >= 2 && layout.GetControlFromPosition(0, 1) is Panel cuerpo)
            {
                return cuerpo;
            }
        }

        return null;
    }

    /// <summary>
    /// Crea la barra horizontal de la tarjeta Conexión: botón TCP y etiqueta de estado.
    /// </summary>
    public static TableLayoutPanel CrearBarraConexion(Button boton, Label estado)
    {
        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            Height = AltoBarraConexion,
            MinimumSize = new Size(0, AltoBarraConexion)
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 188F));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        barra.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoBarraConexion));

        boton.Width = 180;
        boton.Height = 34;
        boton.Margin = new Padding(0, 9, 8, 9);
        boton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        boton.AutoSize = false;
        EstilizarBotonPrimario(boton);

        estado.Dock = DockStyle.Fill;
        estado.AutoSize = false;
        estado.TextAlign = ContentAlignment.MiddleLeft;
        estado.BackColor = Tarjeta;
        estado.Padding = new Padding(4, 0, 4, 0);

        barra.Controls.Add(boton, 0, 0);
        barra.Controls.Add(estado, 1, 0);
        return barra;
    }

    /// <summary>
    /// Ajusta el ancho de todos los hijos de un panel al ancho cliente disponible (layout responsivo).
    /// </summary>
    public static void EnlazarAnchoContenedor(Panel contenedor)
    {
        void ajustar()
        {
            var ancho = contenedor.ClientSize.Width - contenedor.Padding.Horizontal;
            if (ancho <= 0)
            {
                return;
            }

            foreach (Control hijo in contenedor.Controls)
            {
                hijo.Width = ancho;
            }
        }

        contenedor.HandleCreated += (_, _) => ajustar();
        contenedor.Resize += (_, _) => ajustar();
        ajustar();
    }

    /// <summary>
    /// Agrega un control hijo al contenedor con Dock Top y enlaza el ancho responsivo.
    /// </summary>
    public static void AgregarContenido(Panel contenedor, Control contenido)
    {
        contenido.Dock = DockStyle.Top;
        contenido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        contenedor.Controls.Add(contenido);
        EnlazarAnchoContenedor(contenedor);
    }

    /// <summary>
    /// Crea un TableLayoutPanel de formulario de dos columnas (etiqueta | control).
    /// </summary>
    public static TableLayoutPanel CrearFormulario(int filas, bool incluirBoton = true)
    {
        var filasTotal = incluirBoton ? filas + 1 : filas;
        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AlturaFormulario(filas, incluirBoton),
            ColumnCount = 2,
            RowCount = filasTotal,
            BackColor = Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoEtiquetaFormulario));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (var i = 0; i < filas; i++)
        {
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoFilaFormulario));
        }

        if (incluirBoton)
        {
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoBotonFormulario));
        }

        return form;
    }

    /// <summary>
    /// Crea formulario de tres columnas usado en autenticación: etiqueta | campo | botón/auxiliar.
    /// </summary>
    public static TableLayoutPanel CrearFormularioTresColumnas(int filas, bool incluirBoton = true)
    {
        var filasTotal = incluirBoton ? filas + 1 : filas;
        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = AlturaFormulario(filas, incluirBoton),
            ColumnCount = 3,
            RowCount = filasTotal,
            BackColor = Tarjeta,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoEtiquetaFormulario));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        for (var i = 0; i < filas; i++)
        {
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoFilaFormulario));
        }

        if (incluirBoton)
        {
            form.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoBotonFormulario));
        }

        return form;
    }

    /// <summary>
    /// Agrega una fila al formulario: etiqueta en columna 0 y control en columna 1.
    /// </summary>
    public static void AgregarCampo(TableLayoutPanel form, int fila, string etiqueta, Control control)
    {
        form.Controls.Add(new Label
        {
            Text = etiqueta,
            Font = FuenteCuerpo,
            ForeColor = TextoSecundario,
            BackColor = Tarjeta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, fila);
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 4, 0, 4);
        control.ForeColor = Texto;
        control.Font = FuenteCuerpo;
        form.Controls.Add(control, 1, fila);
    }

    /// <summary>
    /// Agrega un botón primario en la fila indicada del formulario (columna 1).
    /// </summary>
    public static void AgregarBoton(TableLayoutPanel form, int fila, Button boton)
    {
        boton.Dock = DockStyle.Left;
        boton.Width = 170;
        boton.Height = 34;
        boton.Margin = new Padding(0, 6, 0, 4);
        EstilizarBotonPrimario(boton);
        form.Controls.Add(boton, 1, fila);
    }

    /// <summary>
    /// Aplica estilo de botón primario (azul acento, texto blanco, hover).
    /// </summary>
    public static void EstilizarBotonPrimario(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.BackColor = Acento;
        boton.ForeColor = Color.White;
        boton.Font = FuenteBoton;
        boton.Cursor = Cursors.Hand;
        boton.Height = 34;
        boton.UseVisualStyleBackColor = false;
        boton.MouseEnter += (_, _) => boton.BackColor = AcentoHover;
        boton.MouseLeave += (_, _) => boton.BackColor = Acento;
    }

    /// <summary>
    /// Aplica estilo de botón secundario (borde gris, fondo blanco) para acciones como Actualizar.
    /// </summary>
    public static void EstilizarBotonSecundario(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderColor = Borde;
        boton.FlatAppearance.BorderSize = 1;
        boton.BackColor = Tarjeta;
        boton.ForeColor = Texto;
        boton.Font = FuenteCuerpo;
        boton.Cursor = Cursors.Hand;
        boton.Height = 34;
        boton.UseVisualStyleBackColor = false;
    }

    /// <summary>
    /// Estiliza TextBox para entradas de identificación, montos y campos de texto generales.
    /// </summary>
    public static void EstilizarEntrada(TextBox caja)
    {
        caja.BorderStyle = BorderStyle.FixedSingle;
        caja.BackColor = Color.White;
        caja.ForeColor = Texto;
        caja.Font = FuenteCuerpo;
    }

    /// <summary>
    /// Estiliza ComboBox de partidos y localidades (solo selección de lista, sin edición libre).
    /// </summary>
    public static void EstilizarCombo(ComboBox combo)
    {
        combo.FlatStyle = FlatStyle.Flat;
        combo.BackColor = Color.White;
        combo.ForeColor = Texto;
        combo.Font = FuenteCuerpo;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        combo.IntegralHeight = false;
    }

    /// <summary>
    /// Aplica estilos base comunes a cualquier DataGridView (colores, filas alternas, encabezado).
    /// </summary>
    public static void EstilizarGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Tarjeta;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Borde;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ScrollBars = ScrollBars.Both;
        grid.ColumnHeadersDefaultCellStyle.BackColor = EncabezadoGrid;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Texto;
        grid.ColumnHeadersDefaultCellStyle.Font = FuenteGridEncabezado;
        grid.ColumnHeadersHeight = 38;
        grid.DefaultCellStyle.ForeColor = Texto;
        grid.DefaultCellStyle.BackColor = Tarjeta;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 228, 255);
        grid.DefaultCellStyle.SelectionForeColor = Texto;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 250);
        grid.AlternatingRowsDefaultCellStyle.ForeColor = Texto;
        grid.RowTemplate.Height = 32;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
    }

    /// <summary>
    /// Configura columnas específicas del grid de historial de compras del cliente autenticado.
    /// </summary>
    public static void ConfigurarGridCompras(DataGridView grid)
    {
        EstilizarGrid(grid);
        grid.AutoGenerateColumns = false;
        grid.Columns.Clear();
        grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "IdVenta", DataPropertyName = "IdVenta", HeaderText = "N.º venta", FillWeight = 70, MinimumWidth = 60 },
            new DataGridViewTextBoxColumn { Name = "PartidoRival", DataPropertyName = "PartidoRival", HeaderText = "Partido", FillWeight = 120, MinimumWidth = 90 },
            new DataGridViewTextBoxColumn { Name = "PartidoFecha", DataPropertyName = "PartidoFecha", HeaderText = "Fecha partido", FillWeight = 90, MinimumWidth = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } },
            new DataGridViewTextBoxColumn { Name = "PartidoHora", DataPropertyName = "PartidoHora", HeaderText = "Hora", FillWeight = 55, MinimumWidth = 50 },
            new DataGridViewTextBoxColumn { Name = "LocalidadNombre", DataPropertyName = "LocalidadNombre", HeaderText = "Localidad", FillWeight = 100, MinimumWidth = 80 },
            new DataGridViewTextBoxColumn { Name = "Cantidad", DataPropertyName = "Cantidad", HeaderText = "Cant.", FillWeight = 50, MinimumWidth = 45 },
            new DataGridViewTextBoxColumn { Name = "MontoTotal", DataPropertyName = "MontoTotal", HeaderText = "Monto", FillWeight = 75, MinimumWidth = 65, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } },
            new DataGridViewTextBoxColumn { Name = "FechaVenta", DataPropertyName = "FechaVenta", HeaderText = "Fecha compra", FillWeight = 95, MinimumWidth = 85, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } },
            new DataGridViewTextBoxColumn { Name = "TipoVenta", DataPropertyName = "TipoVenta", HeaderText = "Tipo", FillWeight = 80, MinimumWidth = 70 });
    }

    /// <summary>
    /// Colorea la etiqueta de estado de conexión TCP: verde si conectado, rojo si desconectado.
    /// </summary>
    public static void EstilizarEstado(Label etiqueta, bool conectado)
    {
        etiqueta.ForeColor = conectado ? Exito : Error;
        etiqueta.BackColor = Tarjeta;
        etiqueta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        etiqueta.AutoEllipsis = true;
    }

    /// <summary>
    /// Fuerza recálculo del scroll en paneles anidados tras mostrar la ventana (corrige barras ocultas).
    /// </summary>
    public static void RefrescarScrolls(Control raiz)
    {
        foreach (Control control in raiz.Controls)
        {
            if (control is Panel { AutoScroll: true } panel)
            {
                panel.PerformLayout();
                panel.AutoScroll = false;
                panel.AutoScroll = true;
            }

            RefrescarScrolls(control);
        }
    }
}
