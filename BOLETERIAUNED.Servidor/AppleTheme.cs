/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Tema visual y helpers de layout (contraste legible, scroll).
 */

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Clase estática con paleta de colores, tipografías y métodos auxiliares de layout
/// inspirados en un estilo visual limpio (tipo Apple). Centraliza la apariencia del servidor WinForms.
/// </summary>
public static class AppleTheme
{
    /// <summary>Color de fondo general de ventanas y paneles contenedores.</summary>
    public static readonly Color Fondo = Color.FromArgb(245, 245, 247);

    /// <summary>Color de fondo de tarjetas y paneles elevados sobre el fondo general.</summary>
    public static readonly Color Tarjeta = Color.White;

    /// <summary>Color principal del texto sobre fondos claros.</summary>
    public static readonly Color Texto = Color.FromArgb(20, 20, 22);

    /// <summary>Color secundario para etiquetas, subtítulos y texto de apoyo.</summary>
    public static readonly Color TextoSecundario = Color.FromArgb(70, 70, 76);

    /// <summary>Color claro para texto sobre fondos oscuros (bitácora).</summary>
    public static readonly Color TextoClaro = Color.FromArgb(245, 245, 247);

    /// <summary>Color de acento para botones primarios y elementos interactivos destacados.</summary>
    public static readonly Color Acento = Color.FromArgb(0, 113, 227);

    /// <summary>Color de acento al pasar el cursor sobre botones primarios (hover).</summary>
    public static readonly Color AcentoHover = Color.FromArgb(0, 91, 196);

    /// <summary>Color de bordes sutiles en tarjetas, inputs y separadores.</summary>
    public static readonly Color Borde = Color.FromArgb(190, 190, 198);

    /// <summary>Color de fondo del encabezado de columnas en DataGridView.</summary>
    public static readonly Color EncabezadoGrid = Color.FromArgb(235, 235, 240);

    /// <summary>Color de fondo del panel inferior de bitácora en tiempo real.</summary>
    public static readonly Color BitacoraFondo = Color.FromArgb(28, 28, 30);

    /// <summary>Altura en píxeles de cada fila de campo en formularios de tarjeta.</summary>
    public const int AltoFilaFormulario = 40;

    /// <summary>Altura en píxeles reservada para la fila del botón Guardar en formularios.</summary>
    public const int AltoBotonFormulario = 46;

    /// <summary>Ancho fijo de la columna de etiquetas en formularios de dos columnas.</summary>
    public const int AnchoEtiquetaFormulario = 132;

    /// <summary>Margen derecho aplicado a controles de entrada para separación visual.</summary>
    public const int MargenDerechoCampo = 8;

    /// <summary>Fuente para títulos principales del encabezado de la ventana.</summary>
    public static Font FuenteTitulo => new("Segoe UI", 18F, FontStyle.Bold);

    /// <summary>Fuente para subtítulos del encabezado (información secundaria).</summary>
    public static Font FuenteSubtitulo => new("Segoe UI", 11F, FontStyle.Regular);

    /// <summary>Fuente para títulos de sección dentro de tarjetas.</summary>
    public static Font FuenteSeccion => new("Segoe UI", 12F, FontStyle.Bold);

    /// <summary>Fuente estándar para etiquetas, entradas y texto de cuerpo.</summary>
    public static Font FuenteCuerpo => new("Segoe UI", 10F, FontStyle.Regular);

    /// <summary>Fuente en negrita para texto de botones primarios.</summary>
    public static Font FuenteBoton => new("Segoe UI", 10F, FontStyle.Bold);

    /// <summary>Fuente para encabezados de columnas en grillas de consulta.</summary>
    public static Font FuenteGridEncabezado => new("Segoe UI", 9.5F, FontStyle.Bold);

    /// <summary>Altura fija del panel de encabezado superior del shell principal.</summary>
    public const int AltoEncabezado = 72;

    /// <summary>Altura fija del panel inferior de bitácora en tiempo real.</summary>
    public const int AltoBitacora = 160;

    /// <summary>Altura de la fila de título dentro de cada tarjeta de registro.</summary>
    public const int AltoTituloTarjeta = 32;

    /// <summary>Margen vertical adicional entre tarjetas en el grid de registros.</summary>
    public const int MargenTarjetaGrid = 12;

    /// <summary>
    /// Configura propiedades de ventana maximizada con fondo y tipografía del tema.
    /// </summary>
    /// <param name="formulario">Formulario principal a configurar.</param>
    /// <param name="tituloVentana">Texto mostrado en la barra de título del sistema.</param>
    public static void ConfigurarVentanaCompleta(Form formulario, string tituloVentana)
    {
        formulario.Text = tituloVentana;
        formulario.BackColor = Fondo;
        formulario.Font = FuenteCuerpo;
        formulario.ForeColor = Texto;
        formulario.StartPosition = FormStartPosition.CenterScreen;
        formulario.WindowState = FormWindowState.Maximized;
        formulario.MinimumSize = new Size(1024, 640);
    }

    /// <summary>
    /// Crea un panel con scroll vertical automático para contenido que excede el área visible.
    /// </summary>
    /// <returns>Panel con Dock Fill y AutoScroll habilitado.</returns>
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
    /// Enlaza un control hijo a un panel con scroll, ajustando ancho y alto mínimo al redimensionar.
    /// </summary>
    /// <param name="scroll">Panel contenedor con AutoScroll.</param>
    /// <param name="contenido">Control hijo (típicamente un grid o tarjeta) anclado arriba.</param>
    /// <param name="altoMinimo">Alto mínimo opcional para AutoScrollMinSize.</param>
    public static void EnlazarContenidoScroll(Panel scroll, Control contenido, int? altoMinimo = null)
    {
        contenido.Dock = DockStyle.Top;
        scroll.Controls.Add(contenido);

        // Función local que recalcula ancho/alto al cambiar tamaño del panel o layout del contenido.
        void refrescar()
        {
            // Resta ancho de barra de scroll vertical si está visible para evitar desbordamiento horizontal.
            var barra = scroll.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            var ancho = scroll.ClientSize.Width - barra - scroll.Padding.Horizontal;
            if (ancho > 0)
            {
                contenido.Width = ancho;
            }

            contenido.PerformLayout();
            var alto = altoMinimo ?? contenido.Height;
            if (alto <= 0)
            {
                alto = contenido.GetPreferredSize(new Size(Math.Max(1, contenido.Width), 0)).Height;
            }

            if (altoMinimo.HasValue)
            {
                contenido.Height = altoMinimo.Value;
            }

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
    /// Calcula la altura total de un formulario según cantidad de filas y presencia de botón.
    /// </summary>
    /// <param name="filas">Número de filas de campos (sin contar botón).</param>
    /// <param name="incluirBoton">Si se reserva espacio para fila de botón Guardar.</param>
    /// <returns>Altura en píxeles.</returns>
    public static int AlturaFormulario(int filas, bool incluirBoton = true)
    {
        var alturaFilas = filas * AltoFilaFormulario;
        var alturaBoton = incluirBoton ? AltoBotonFormulario : 0;
        return alturaFilas + alturaBoton;
    }

    /// <summary>
    /// Calcula la altura total de una tarjeta incluyendo título, padding y formulario interno.
    /// </summary>
    /// <param name="filasFormulario">Filas del formulario dentro de la tarjeta.</param>
    /// <param name="incluirBoton">Si el formulario incluye botón.</param>
    /// <param name="alturaExtra">Píxeles adicionales (etiquetas informativas, etc.).</param>
    /// <returns>Altura total de la tarjeta en píxeles.</returns>
    public static int AlturaTarjeta(int filasFormulario, bool incluirBoton = true, int alturaExtra = 0)
    {
        return AlturaTarjetaContenido(AlturaFormulario(filasFormulario, incluirBoton) + alturaExtra);
    }

    /// <summary>
    /// Suma título, paddings y altura del contenido para obtener altura de tarjeta.
    /// </summary>
    /// <param name="alturaContenido">Altura del cuerpo interno de la tarjeta.</param>
    /// <returns>Altura total en píxeles.</returns>
    public static int AlturaTarjetaContenido(int alturaContenido)
    {
        const int paddingTarjeta = 24;
        const int paddingCuerpo = 4;
        return AltoTituloTarjeta + paddingCuerpo + alturaContenido + paddingTarjeta;
    }

    /// <summary>
    /// Calcula la altura total del grid 2×3 de tarjetas en la pestaña Registros.
    /// </summary>
    /// <returns>Suma de alturas de las tres filas más padding.</returns>
    public static int AlturaGridRegistros()
    {
        var fila0 = Math.Max(AlturaTarjeta(3), AlturaTarjeta(4)) + MargenTarjetaGrid;
        var fila1 = Math.Max(AlturaTarjeta(6), AlturaTarjeta(7)) + MargenTarjetaGrid;
        var fila2 = AlturaTarjeta(4, alturaExtra: 28) + MargenTarjetaGrid;
        return fila0 + fila1 + fila2 + 24;
    }

    /// <summary>
    /// Devuelve la altura de una fila específica del grid de registros (0, 1 o 2).
    /// </summary>
    /// <param name="fila">Índice de fila (0=localidad/partido, 1=vendedor/cliente, 2=LP).</param>
    /// <returns>Altura en píxeles de esa fila del TableLayoutPanel.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Si el índice de fila no es 0, 1 o 2.</exception>
    public static int AlturaFilaRegistros(int fila)
    {
        return fila switch
        {
            0 => Math.Max(AlturaTarjeta(3), AlturaTarjeta(4)) + MargenTarjetaGrid,
            1 => Math.Max(AlturaTarjeta(6), AlturaTarjeta(7)) + MargenTarjetaGrid,
            2 => AlturaTarjeta(4, alturaExtra: 28) + MargenTarjetaGrid,
            _ => throw new ArgumentOutOfRangeException(nameof(fila))
        };
    }

    /// <summary>
    /// Crea el contenedor principal (shell) de tres filas: encabezado, contenido y bitácora.
    /// </summary>
    /// <param name="titulo">Título principal del encabezado.</param>
    /// <param name="subtitulo">Subtítulo con información del panel (ej. dirección TCP).</param>
    /// <param name="areaContenido">Panel central donde se insertan las pestañas.</param>
    /// <param name="areaBitacora">Panel inferior oscuro para la bitácora en tiempo real.</param>
    /// <returns>TableLayoutPanel raíz con Dock Fill.</returns>
    public static TableLayoutPanel CrearShell(string titulo, string subtitulo, out Panel areaContenido, out Panel areaBitacora)
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Fondo,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoEncabezado));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoBitacora));

        var encabezado = CrearEncabezado(titulo, subtitulo);
        encabezado.Dock = DockStyle.Fill;

        areaContenido = new Panel { Dock = DockStyle.Fill, BackColor = Fondo, Padding = new Padding(8), AutoScroll = false };
        areaBitacora = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BitacoraFondo,
            Padding = new Padding(16, 8, 16, 12)
        };

        shell.Controls.Add(encabezado, 0, 0);
        shell.Controls.Add(areaContenido, 0, 1);
        shell.Controls.Add(areaBitacora, 0, 2);
        return shell;
    }

    /// <summary>
    /// Crea el panel de encabezado con título, subtítulo y línea inferior de separación.
    /// </summary>
    /// <param name="titulo">Texto del título principal.</param>
    /// <param name="subtitulo">Texto del subtítulo.</param>
    /// <returns>Panel estilizado para la fila superior del shell.</returns>
    public static Panel CrearEncabezado(string titulo, string subtitulo)
    {
        var panel = new Panel { BackColor = Tarjeta, Padding = new Padding(24, 14, 24, 10) };
        // Dibuja línea de borde inferior al pintar el panel (estilo visual de separación).
        panel.Paint += (_, e) =>
        {
            using var pen = new Pen(Borde);
            e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
        };
        panel.Controls.Add(new Label
        {
            Text = titulo,
            Font = FuenteTitulo,
            ForeColor = Texto,
            BackColor = Tarjeta,
            AutoSize = true,
            Location = new Point(20, 8)
        });
        panel.Controls.Add(new Label
        {
            Text = subtitulo,
            Font = FuenteSubtitulo,
            ForeColor = TextoSecundario,
            BackColor = Tarjeta,
            AutoSize = true,
            Location = new Point(22, 38)
        });
        return panel;
    }

    /// <summary>
    /// Crea una tarjeta visual con título, borde y área de contenido para formularios.
    /// </summary>
    /// <param name="titulo">Título mostrado en la parte superior de la tarjeta.</param>
    /// <param name="filasFormulario">Cantidad de filas del formulario interno.</param>
    /// <param name="incluirBoton">Si el formulario incluye botón Guardar.</param>
    /// <param name="alturaExtra">Píxeles extra para controles informativos adicionales.</param>
    /// <param name="apilada">Si true, la tarjeta usa Dock Top con altura fija; si false, Fill en grid.</param>
    /// <returns>Panel tipo tarjeta con layout interno de título + cuerpo.</returns>
    public static Panel CrearTarjeta(string titulo, int filasFormulario, bool incluirBoton = true, int alturaExtra = 0, bool apilada = false)
    {
        var alturaContenido = AlturaFormulario(filasFormulario, incluirBoton) + alturaExtra;
        var alturaTotal = AlturaTarjetaContenido(alturaContenido);
        var tarjeta = new Panel
        {
            Dock = apilada ? DockStyle.Top : DockStyle.Fill,
            Height = apilada ? alturaTotal : 0,
            MinimumSize = new Size(0, alturaTotal),
            BackColor = Tarjeta,
            Margin = new Padding(6),
            Padding = new Padding(14, 12, 18, 12)
        };
        // Borde rectangular sutil alrededor de la tarjeta.
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

        layout.Controls.Add(new Label
        {
            Text = titulo,
            Font = FuenteSeccion,
            ForeColor = Texto,
            BackColor = Tarjeta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        var cuerpo = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Tarjeta,
            Padding = new Padding(0, 4, MargenDerechoCampo, 0)
        };
        layout.Controls.Add(cuerpo, 0, 1);
        tarjeta.Controls.Add(layout);
        return tarjeta;
    }

    /// <summary>
    /// Obtiene o crea el panel cuerpo dentro de una tarjeta para agregar formularios.
    /// </summary>
    /// <param name="tarjeta">Tarjeta creada con <see cref="CrearTarjeta"/>.</param>
    /// <returns>Panel cuerpo donde se insertan controles del formulario.</returns>
    public static Panel AgregarCuerpoTarjeta(Panel tarjeta)
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
    /// Localiza el panel cuerpo (fila 1) dentro del TableLayoutPanel interno de una tarjeta.
    /// </summary>
    /// <param name="tarjeta">Tarjeta a inspeccionar.</param>
    /// <returns>Panel cuerpo o null si la estructura no coincide.</returns>
    public static Panel? ObtenerCuerpoTarjeta(Panel tarjeta)
    {
        foreach (Control control in tarjeta.Controls)
        {
            if (control is TableLayoutPanel layout &&
                layout.GetControlFromPosition(0, 1) is Panel cuerpo)
            {
                return cuerpo;
            }
        }

        return null;
    }

    /// <summary>
    /// Ajusta el ancho de todos los hijos de un contenedor al ancho cliente menos padding.
    /// </summary>
    /// <param name="contenedor">Panel cuyos hijos deben expandirse horizontalmente.</param>
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
    /// Agrega un control hijo al contenedor y enlaza el ajuste de ancho responsivo.
    /// </summary>
    /// <param name="contenedor">Panel destino.</param>
    /// <param name="contenido">Control a insertar (formulario, layout, etc.).</param>
    public static void AgregarContenido(Panel contenedor, Control contenido)
    {
        contenido.Dock = DockStyle.Top;
        contenido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        contenedor.Controls.Add(contenido);
        EnlazarAnchoContenedor(contenedor);
    }

    /// <summary>
    /// Crea un TableLayoutPanel de formulario con columna de etiquetas y columna de controles.
    /// </summary>
    /// <param name="filas">Cantidad de filas de campos (sin contar botón).</param>
    /// <param name="incluirBoton">Si se agrega fila adicional para botón.</param>
    /// <returns>Formulario con estilos de fila fijos.</returns>
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
    /// Agrega una fila etiqueta + control al formulario de tarjeta.
    /// </summary>
    /// <param name="form">Formulario destino.</param>
    /// <param name="fila">Índice de fila (0-based).</param>
    /// <param name="etiqueta">Texto de la etiqueta izquierda.</param>
    /// <param name="control">Control de entrada derecho.</param>
    public static void AgregarCampo(TableLayoutPanel form, int fila, string etiqueta, Control control)
    {
        var lbl = new Label
        {
            Text = etiqueta,
            Font = FuenteCuerpo,
            ForeColor = TextoSecundario,
            BackColor = Tarjeta,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 4, MargenDerechoCampo, 4);
        control.ForeColor = Texto;
        control.Font = FuenteCuerpo;
        form.Controls.Add(lbl, 0, fila);
        form.Controls.Add(control, 1, fila);
    }

    /// <summary>
    /// Agrega un botón primario estilizado en la columna derecha del formulario.
    /// </summary>
    /// <param name="form">Formulario destino.</param>
    /// <param name="fila">Índice de fila del botón.</param>
    /// <param name="boton">Instancia del botón (texto y handler se configuran externamente).</param>
    public static void AgregarBoton(TableLayoutPanel form, int fila, Button boton)
    {
        boton.Dock = DockStyle.Left;
        boton.Width = 180;
        boton.Margin = new Padding(0, 6, 0, 0);
        EstilizarBotonPrimario(boton);
        form.Controls.Add(boton, 1, fila);
    }

    /// <summary>
    /// Configura el panel de bitácora inferior con título y ListBox de eventos en tiempo real.
    /// </summary>
    /// <param name="contenedor">Panel oscuro del shell (areaBitacora).</param>
    /// <param name="titulo">Label titular de la bitácora.</param>
    /// <param name="lista">ListBox donde se insertan mensajes del servidor TCP y la UI.</param>
    public static void ConfigurarBitacora(Panel contenedor, Label titulo, ListBox lista)
    {
        titulo.Text = "Bitácora en tiempo real";
        titulo.Font = FuenteSeccion;
        titulo.ForeColor = TextoClaro;
        titulo.Dock = DockStyle.Top;
        titulo.Height = 28;
        titulo.BackColor = BitacoraFondo;

        lista.Dock = DockStyle.Fill;
        lista.Font = new Font("Segoe UI", 9.5F);
        lista.BackColor = BitacoraFondo;
        lista.ForeColor = TextoClaro;
        lista.BorderStyle = BorderStyle.None;
        lista.IntegralHeight = false;
        lista.HorizontalScrollbar = true;
        lista.ScrollAlwaysVisible = true;

        // Orden de Controls: lista primero (Fill), título encima (Top) — WinForms pinta en orden Z.
        contenedor.Controls.Add(lista);
        contenedor.Controls.Add(titulo);
    }

    /// <summary>
    /// Aplica estilo visual de botón primario (fondo acento, texto blanco, hover).
    /// </summary>
    /// <param name="boton">Botón a estilizar (Guardar, Registrar venta, etc.).</param>
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
        // Efecto hover: oscurece el acento al entrar el cursor.
        boton.MouseEnter += (_, _) => boton.BackColor = AcentoHover;
        boton.MouseLeave += (_, _) => boton.BackColor = Acento;
    }

    /// <summary>
    /// Aplica estilo visual de botón secundario (fondo claro con borde).
    /// </summary>
    /// <param name="boton">Botón a estilizar (Actualizar en consultas).</param>
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
    /// Aplica estilo visual uniforme a cajas de texto editables.
    /// </summary>
    /// <param name="caja">TextBox a estilizar.</param>
    public static void EstilizarEntrada(TextBox caja)
    {
        caja.BorderStyle = BorderStyle.FixedSingle;
        caja.BackColor = Color.White;
        caja.ForeColor = Texto;
        caja.Font = FuenteCuerpo;
    }

    /// <summary>
    /// Aplica estilo visual a selectores de fecha con calendario acentuado.
    /// </summary>
    /// <param name="selector">DateTimePicker a estilizar.</param>
    public static void EstilizarFecha(DateTimePicker selector)
    {
        selector.Format = DateTimePickerFormat.Short;
        selector.Font = FuenteCuerpo;
        selector.ForeColor = Texto;
        selector.CalendarForeColor = Texto;
        selector.CalendarTitleBackColor = Acento;
        selector.CalendarTitleForeColor = Color.White;
    }

    /// <summary>
    /// Aplica estilo visual a controles NumericUpDown.
    /// </summary>
    /// <param name="numerico">NumericUpDown a estilizar.</param>
    public static void EstilizarNumerico(NumericUpDown numerico)
    {
        numerico.BorderStyle = BorderStyle.FixedSingle;
        numerico.BackColor = Color.White;
        numerico.ForeColor = Texto;
        numerico.Font = FuenteCuerpo;
    }

    /// <summary>
    /// Aplica estilo visual a ComboBox de solo selección (DropDownList).
    /// </summary>
    /// <param name="combo">ComboBox a estilizar.</param>
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
    /// Configura un TabControl con dibujo personalizado de pestañas (OwnerDrawFixed).
    /// </summary>
    /// <param name="tabs">TabControl principal o interno de consultas.</param>
    public static void EstilizarTabControl(TabControl tabs)
    {
        tabs.Dock = DockStyle.Fill;
        tabs.Font = FuenteCuerpo;
        tabs.Padding = new Point(14, 6);
        tabs.ItemSize = new Size(120, 32);
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.DrawItem += DibujarPestana;
        foreach (TabPage pagina in tabs.TabPages)
        {
            pagina.BackColor = Fondo;
            pagina.ForeColor = Texto;
            pagina.Padding = new Padding(4);
            pagina.AutoScroll = false;
        }
    }

    /// <summary>
    /// Dibuja cada pestaña con fondo diferenciado, línea de acento en la seleccionada y texto centrado.
    /// </summary>
    /// <param name="sender">TabControl que dispara el evento DrawItem.</param>
    /// <param name="e">Argumentos con índice y área de dibujo de la pestaña.</param>
    private static void DibujarPestana(object? sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tabs)
        {
            return;
        }

        var pagina = tabs.TabPages[e.Index];
        var seleccionada = e.Index == tabs.SelectedIndex;
        var fondo = seleccionada ? Tarjeta : Color.FromArgb(225, 225, 230);

        using var brocha = new SolidBrush(fondo);
        e.Graphics.FillRectangle(brocha, e.Bounds);

        if (seleccionada)
        {
            // Línea inferior azul indica pestaña activa.
            using var acento = new Pen(Acento, 2);
            e.Graphics.DrawLine(acento, e.Bounds.Left + 4, e.Bounds.Bottom - 1, e.Bounds.Right - 4, e.Bounds.Bottom - 1);
        }

        TextRenderer.DrawText(
            e.Graphics,
            pagina.Text,
            tabs.Font,
            e.Bounds,
            Texto,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    /// <summary>
    /// Aplica estilo visual completo a un DataGridView de consulta (colores, filas alternas, selección).
    /// </summary>
    /// <param name="grid">Grilla a estilizar.</param>
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
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.DefaultCellStyle.ForeColor = Texto;
        grid.DefaultCellStyle.BackColor = Tarjeta;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 228, 255);
        grid.DefaultCellStyle.SelectionForeColor = Texto;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 250);
        grid.AlternatingRowsDefaultCellStyle.ForeColor = Texto;
        grid.RowTemplate.Height = 32;
    }

    /// <summary>
    /// Recorre recursivamente el árbol de controles y fuerza recálculo de AutoScroll en paneles.
    /// Útil al mostrar el formulario por primera vez (evento Shown).
    /// </summary>
    /// <param name="raiz">Control raíz desde donde iniciar la búsqueda.</param>
    public static void RefrescarScrolls(Control raiz)
    {
        foreach (Control control in raiz.Controls)
        {
            if (control is Panel { AutoScroll: true } panel)
            {
                panel.PerformLayout();
                // Toggle AutoScroll fuerza recálculo del área desplazable en WinForms.
                panel.AutoScroll = false;
                panel.AutoScroll = true;
            }

            RefrescarScrolls(control);
        }
    }
}
