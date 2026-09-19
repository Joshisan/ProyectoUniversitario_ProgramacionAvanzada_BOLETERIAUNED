# -*- coding: utf-8 -*-
"""
Genera el manual PDF de implementación del proyecto BOLETERIAUNED (~20 páginas).

UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
"""

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_JUSTIFY, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import cm
from reportlab.platypus import (
    HRFlowable,
    PageBreak,
    Paragraph,
    Preformatted,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)

OUTPUT = r"c:\Users\steve\OneDrive\Documents\academy-material\Projects\Jossue Sanabria\BOLETERIAUNED\Manual_Implementacion_BOLETERIAUNED.pdf"


def build_styles():
    base = getSampleStyleSheet()
    styles = {
        "title": ParagraphStyle(
            "DocTitle",
            parent=base["Title"],
            fontName="Helvetica-Bold",
            fontSize=22,
            leading=28,
            alignment=TA_CENTER,
            spaceAfter=18,
            textColor=colors.HexColor("#1a1a2e"),
        ),
        "subtitle": ParagraphStyle(
            "DocSubtitle",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=13,
            leading=18,
            alignment=TA_CENTER,
            spaceAfter=8,
            textColor=colors.HexColor("#444444"),
        ),
        "h1": ParagraphStyle(
            "H1",
            parent=base["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=16,
            leading=20,
            spaceBefore=14,
            spaceAfter=10,
            textColor=colors.HexColor("#0f3460"),
        ),
        "h2": ParagraphStyle(
            "H2",
            parent=base["Heading2"],
            fontName="Helvetica-Bold",
            fontSize=13,
            leading=16,
            spaceBefore=10,
            spaceAfter=6,
            textColor=colors.HexColor("#16213e"),
        ),
        "body": ParagraphStyle(
            "Body",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=10.5,
            leading=14,
            alignment=TA_JUSTIFY,
            spaceAfter=8,
        ),
        "bullet": ParagraphStyle(
            "Bullet",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=10.5,
            leading=14,
            leftIndent=18,
            bulletIndent=8,
            spaceAfter=4,
        ),
        "code": ParagraphStyle(
            "CodeBlock",
            parent=base["Code"],
            fontName="Courier",
            fontSize=8.5,
            leading=11,
            backColor=colors.HexColor("#f4f4f8"),
            borderPadding=6,
            spaceAfter=8,
        ),
        "footer": ParagraphStyle(
            "Footer",
            parent=base["Normal"],
            fontName="Helvetica-Oblique",
            fontSize=9,
            alignment=TA_CENTER,
            textColor=colors.grey,
        ),
    }
    return styles


def footer(canvas, doc):
    canvas.saveState()
    canvas.setFont("Helvetica", 9)
    canvas.setFillColor(colors.grey)
    canvas.drawCentredString(A4[0] / 2, 1.2 * cm, f"Página {doc.page}")
    canvas.drawString(2 * cm, 1.2 * cm, "BOLETERIAUNED — Manual de Implementación")
    canvas.drawRightString(A4[0] - 2 * cm, 1.2 * cm, "Jossue Sanabria — UNED 00830")
    canvas.restoreState()


def P(text, style):
    return Paragraph(text, style)


def code(text, style):
    return Preformatted(text, style)


def bullet(items, style):
    return [P(f"• {item}", style) for item in items]


def table(data, col_widths=None):
    t = Table(data, colWidths=col_widths, hAlign="LEFT")
    t.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#0f3460")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
                ("FONTSIZE", (0, 0), (-1, -1), 9),
                ("FONTNAME", (0, 1), (-1, -1), "Helvetica"),
                ("GRID", (0, 0), (-1, -1), 0.5, colors.lightgrey),
                ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#f8f9fc")]),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 6),
                ("RIGHTPADDING", (0, 0), (-1, -1), 6),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
            ]
        )
    )
    return t


def build_story(styles):
    s = styles
    story = []

    # --- PÁGINA 1: PORTADA ---
    story += [Spacer(1, 3 * cm)]
    story += [P("Universidad Estatal a Distancia (UNED)", s["subtitle"])]
    story += [P("Programación Avanzada con C# — Código 00830", s["subtitle"])]
    story += [P("II Cuatrimestre 2026", s["subtitle"])]
    story += [Spacer(1, 1.5 * cm)]
    story += [P("Manual Técnico de Implementación", s["title"])]
    story += [P("Sistema de Boletaría UNED — Proyecto #2", s["title"])]
    story += [Spacer(1, 1 * cm)]
    story += [HRFlowable(width="80%", thickness=1, color=colors.HexColor("#0f3460"))]
    story += [Spacer(1, 1 * cm)]
    story += [P("Estudiante: <b>Jossue Sanabria</b>", s["subtitle"])]
    story += [P("Fecha de desarrollo: 04/07/2026", s["subtitle"])]
    story += [Spacer(1, 2 * cm)]
    story += [
        P(
            "Este documento describe en profundidad la arquitectura, el diseño orientado a objetos, "
            "la base de datos, la comunicación TCP/JSON, la lógica de negocio, el control de concurrencia "
            "y el funcionamiento de cada componente del sistema BOLETERIAUNED.",
            s["body"],
        )
    ]
    story += [PageBreak()]

    # --- PÁGINA 2: ÍNDICE Y VISIÓN GENERAL ---
    story += [P("1. Introducción y objetivos", s["h1"])]
    story += [
        P(
            "El proyecto <b>BOLETERIAUNED</b> es un sistema distribuido para la venta de entradas "
            "a partidos de fútbol del equipo UNED. Combina una aplicación de escritorio para la "
            "administración en boletería física (servidor) con una aplicación cliente que permite "
            "compras en línea mediante comunicación TCP. El sistema persiste toda la información "
            "en Microsoft SQL Server y está organizado en una arquitectura de capas clásica: "
            "<b>Entidades</b>, <b>Acceso a Datos</b>, <b>Lógica de Negocio</b>, más dos "
            "aplicaciones de presentación (<b>Servidor</b> y <b>Cliente</b>).",
            s["body"],
        )
    ]
    story += [P("1.1 Objetivos funcionales", s["h2"])]
    story += bullet(
        [
            "Registrar y consultar localidades del estadio con sus precios unitarios.",
            "Registrar y consultar partidos (rival, fecha, hora, estado activo/inactivo).",
            "Registrar vendedores y clientes con validaciones de integridad.",
            "Asociar localidades a partidos con cantidad disponible de entradas.",
            "Procesar ventas en boletería (con vendedor) y en línea (sin vendedor).",
            "Consultar historial de compras por cliente desde la aplicación en línea.",
            "Atender múltiples clientes TCP simultáneamente con autenticación por sesión.",
        ],
        s["bullet"],
    )
    story += [P("1.2 Objetivos técnicos (POO y arquitectura)", s["h2"])]
    story += bullet(
        [
            "Aplicar herencia: clase abstracta Persona base de Cliente y Vendedor.",
            "Aplicar polimorfismo: interfaz IValidable y clase utilitaria ValidacionEntidad.",
            "Separar responsabilidades en proyectos independientes (.NET 10).",
            "Usar ADO.NET parametrizado contra SQL Server (Microsoft.Data.SqlClient).",
            "Implementar protocolo propio JSON sobre TCP (puerto 14500, IP 127.0.0.1).",
            "Controlar concurrencia en ventas con SemaphoreSlim y transacciones SQL.",
        ],
        s["bullet"],
    )
    story += [P("1.3 Estructura de la solución", s["h2"])]
    story += [
        table(
            [
                ["Proyecto", "Tipo", "Responsabilidad"],
                ["BOLETERIAUNED.Entidades", "Biblioteca", "Modelos, validaciones, protocolo JSON"],
                ["BOLETERIAUNED.AccesoDatos", "Biblioteca", "Consultas e inserciones SQL"],
                ["BOLETERIAUNED.LogicaNegocio", "Biblioteca", "Reglas de negocio y transacciones"],
                ["BOLETERIAUNED.Servidor", "WinForms", "Admin + TCP multihilo + ventas boletería"],
                ["BOLETERIAUNED.Cliente", "WinForms", "Compras en línea vía TCP"],
            ],
            [5 * cm, 3 * cm, 9 * cm],
        )
    ]
    story += [PageBreak()]

    # --- PÁGINA 3: ARQUITECTURA ---
    story += [P("2. Arquitectura del sistema", s["h1"])]
    story += [
        P(
            "La arquitectura sigue el patrón <b>N-Capas</b> (presentación, lógica, datos). "
            "Las dependencias fluyen en una sola dirección: las aplicaciones WinForms dependen "
            "de LogicaNegocio; LogicaNegocio depende de AccesoDatos y Entidades; AccesoDatos "
            "depende solo de Entidades. Ninguna capa inferior conoce la capa superior.",
            s["body"],
        )
    ]
    story += [P("2.1 Diagrama de capas", s["h2"])]
    story += [
        code(
            """  +------------------+       +------------------+
  | BOLETERIAUNED    |       | BOLETERIAUNED    |
  | .Cliente         |       | .Servidor        |
  | (WinForms + TCP) |       | (WinForms + TCP) |
  +--------+---------+       +--------+---------+
           |                          |
           |    JSON/TCP :14500       |
           +------------+-------------+
                        |
           +------------v-------------+
           | BOLETERIAUNED.LogicaNegocio|
           | ClienteLN, VentaLN, etc.   |
           +------------+-------------+
                        |
           +------------v-------------+
           | BOLETERIAUNED.AccesoDatos  |
           | ClienteAD, VentaAD, etc.   |
           +------------+-------------+
                        |
           +------------v-------------+
           | BOLETERIAUNED.Entidades    |
           | Cliente, Venta, IValidable |
           +------------+-------------+
                        |
           +------------v-------------+
           | SQL Server (BOLETERIAUNED) |
           +----------------------------+""",
            s["code"],
        )
    ]
    story += [P("2.2 Flujo de una venta en boletería", s["h2"])]
    story += bullet(
        [
            "El usuario completa el formulario de venta en Form1 del Servidor.",
            "Se construye un objeto Venta con Cliente, Vendedor, Partido, Localidad y Cantidad.",
            "VentaLN.RegistrarVentaBoleteria valida entidad, cliente activo, partido activo/futuro, "
            "disponibilidad y vendedor registrado.",
            "EjecutarVentaTransaccional adquiere un semáforo por par (partido, localidad).",
            "En una transacción SQL: decrementa CantidadDisponible e inserta la fila en Venta.",
            "El grid de ventas se refresca automáticamente en la interfaz.",
        ],
        s["bullet"],
    )
    story += [P("2.3 Flujo de una compra en línea", s["h2"])]
    story += bullet(
        [
            "El cliente WinForms se conecta al servidor TCP y envía PING.",
            "Valida identificación o IdCliente; el servidor guarda sesión (_clienteSesionId).",
            "Consulta partidos activos y localidades disponibles vía comandos JSON.",
            "COMPRAR_EN_LINEA verifica que IdCliente coincida con la sesión autenticada.",
            "VentaLN.RegistrarVentaEnLinea procesa la venta sin vendedor (TipoVenta = 'En línea').",
            "El servidor notifica DatosActualizados para refrescar grids del administrador.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINA 4-5: BASE DE DATOS ---
    story += [P("3. Base de datos SQL Server", s["h1"])]
    story += [
        P(
            "La base de datos <b>BOLETERIAUNED</b> se crea con el script "
            "<i>Script Base Datos BOLETERIAUNED proyecto 2.sql</i>. "
            "Utiliza seis tablas relacionadas mediante claves foráneas. "
            "La cadena de conexión se define en App.config del proyecto Servidor "
            "con Integrated Security contra SQLEXPRESS.",
            s["body"],
        )
    ]
    story += [P("3.1 Modelo entidad-relación", s["h2"])]
    story += [
        code(
            """  Localidad (1) ----< LocalidadPorPartido >---- (N) Partido
                              |
                              v
                            Venta >---- Cliente
                              |
                              +---- Vendedor (opcional, NULL en línea)
                              |
                              +---- Localidad
                              +---- Partido""",
            s["code"],
        )
    ]
    story += [P("3.2 Tabla Cliente", s["h2"])]
    story += [
        table(
            [
                ["Campo", "Tipo", "Restricción", "Descripción"],
                ["IdCliente", "int", "PK", "Identificador único del cliente"],
                ["Identificacion", "varchar(10)", "UNIQUE, NOT NULL", "Cédula o documento"],
                ["Nombre / Apellido", "varchar(25)", "NOT NULL", "Datos personales"],
                ["FechaNacimiento", "datetime", "NOT NULL", "Debe ser anterior a hoy"],
                ["FechaRegistro", "datetime", "NOT NULL", "Fecha de alta en el sistema"],
                ["Activo", "bit", "NOT NULL", "Solo clientes activos pueden comprar"],
            ],
            [3.2 * cm, 2.5 * cm, 3.5 * cm, 6.3 * cm],
        )
    ]
    story += [Spacer(1, 0.3 * cm)]
    story += [P("3.3 Tabla Partido", s["h2"])]
    story += [
        table(
            [
                ["Campo", "Tipo", "Descripción"],
                ["IdPartido", "int PK", "Identificador del partido"],
                ["Rival", "varchar(100)", "Nombre del equipo visitante"],
                ["Fecha", "datetime", "Fecha del encuentro (>= hoy para ventas)"],
                ["Hora", "varchar(10)", "Formato HH:mm (ej. 15:00)"],
                ["Activo", "bit", "Partidos inactivos no admiten ventas"],
            ],
            [3.5 * cm, 3 * cm, 8 * cm],
        )
    ]
    story += [PageBreak()]

    story += [P("3.4 Tabla Localidad", s["h2"])]
    story += [
        table(
            [
                ["Campo", "Tipo", "Descripción"],
                ["IdLocalidad", "int PK", "Identificador de la sección del estadio"],
                ["NombreLocalidad", "varchar(50)", "Ej: Palco, Preferencial, Sol, Sombra"],
                ["Precio", "decimal(10,2)", "Precio unitario por entrada (> 0)"],
            ],
            [3.5 * cm, 3 * cm, 8 * cm],
        )
    ]
    story += [Spacer(1, 0.3 * cm)]
    story += [P("3.5 Tabla LocalidadPorPartido", s["h2"])]
    story += [
        P(
            "Tabla puente que define cuántas entradas hay disponibles por localidad en cada partido. "
            "La restricción UNIQUE (IdPartido, IdLocalidad) impide duplicar la misma combinación. "
            "Al vender, se decrementa CantidadDisponible dentro de una transacción.",
            s["body"],
        ),
        table(
            [
                ["Campo", "Tipo", "Descripción"],
                ["IdLocalidadPartido", "int PK", "Identificador del registro"],
                ["IdPartido", "int FK", "Referencia a Partido"],
                ["IdLocalidad", "int FK", "Referencia a Localidad"],
                ["CantidadDisponible", "int", "Entradas restantes (> 0 al registrar)"],
            ],
            [3.5 * cm, 2.5 * cm, 8.5 * cm],
        ),
    ]
    story += [Spacer(1, 0.3 * cm)]
    story += [P("3.6 Tabla Vendedor y Venta", s["h2"])]
    story += [
        P(
            "<b>Vendedor</b> comparte estructura con Cliente (Persona): identificación única, "
            "nombre, apellido, fecha nacimiento y fecha ingreso. "
            "<b>Venta</b> usa IdVenta IDENTITY como clave autogenerada. IdVendedor es NULL "
            "para compras en línea. TipoVenta distingue 'Boletería' vs 'En línea'. "
            "MontoTotal = Cantidad × Precio de la localidad (calculado en lógica, no en BD).",
            s["body"],
        )
    ]
    story += [P("3.7 Datos de prueba", s["h2"])]
    story += bullet(
        [
            "4 localidades: Palco (₡15.000), Preferencial (₡10.000), Sol (₡5.000), Sombra (₡7.000).",
            "3 partidos activos contra UCR, TEC y LD Alajuelense.",
            "2 vendedores y 3 clientes (uno inactivo: Carlos Vega, Id 3).",
            "6 registros LocalidadPorPartido con inventarios variados por partido.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINA 6-7: ENTIDADES ---
    story += [P("4. Capa de Entidades (BOLETERIAUNED.Entidades)", s["h1"])]
    story += [
        P(
            "Esta capa contiene las clases del dominio, la interfaz de validación, "
            "el protocolo de red y DTOs de consulta. No tiene dependencias externas "
            "más allá de System.Text.Json.",
            s["body"],
        )
    ]
    story += [P("4.1 Herencia: clase abstracta Persona", s["h2"])]
    story += [
        P(
            "<b>Persona</b> es una clase abstracta que centraliza Id, Identificacion, Nombre, "
            "Apellido, FechaNacimiento y la propiedad calculada NombreCompleto. "
            "Define el método abstracto ObtenerTipoPersona() y el método protegido "
            "ValidarPersonaBase() con reglas comunes: identificación no vacía (máx. 10 chars), "
            "nombre/apellido obligatorios y fecha nacimiento estrictamente anterior a hoy.",
            s["body"],
        ),
        code(
            """public abstract class Persona {
    public int Id { get; set; }
    public string Identificacion { get; set; }
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
    public abstract string ObtenerTipoPersona();
    protected void ValidarPersonaBase() { /* reglas comunes */ }
}""",
            s["code"],
        ),
    ]
    story += [P("4.2 Cliente y Vendedor (especialización)", s["h2"])]
    story += [
        P(
            "<b>Cliente</b> agrega FechaRegistro y Activo. Su Validar() invoca ValidarPersonaBase() "
            "y verifica que FechaRegistro no sea futura. "
            "<b>Vendedor</b> agrega FechaIngreso y valida que sea posterior a FechaNacimiento "
            "y no futura. Ambas implementan IValidable y sobrescriben ObtenerTipoPersona().",
            s["body"],
        )
    ]
    story += [P("4.3 Polimorfismo con IValidable", s["h2"])]
    story += [
        P(
            "La interfaz <b>IValidable</b> declara void Validar(). "
            "Las clases Cliente, Vendedor, Partido, Localidad, LocalidadPorPartido y Venta "
            "la implementan con reglas específicas. "
            "La clase estática <b>ValidacionEntidad</b> expone Validar(IValidable) y "
            "ValidarVarios(params IValidable[]) para invocar validación polimórfica "
            "desde la capa de lógica sin conocer el tipo concreto.",
            s["body"],
        ),
        code(
            """public interface IValidable { void Validar(); }

public static class ValidacionEntidad {
    public static void Validar(IValidable entidad) => entidad.Validar();
}""",
            s["code"],
        ),
    ]
    story += [PageBreak()]

    story += [P("4.4 Entidades de negocio adicionales", s["h2"])]
    story += bullet(
        [
            "<b>Partido</b>: propiedad DescripcionPartido para combos; valida rival no vacío, "
            "fecha >= hoy y hora con regex ^([01]\\d|2[0-3]):[0-5]\\d$.",
            "<b>Localidad</b>: valida nombre y precio > 0; ToString() muestra nombre y precio.",
            "<b>LocalidadPorPartido</b>: compone objetos Partido y Localidad; "
            "DescripcionLocalidad incluye precio y disponibles para la UI.",
            "<b>Venta</b>: compone Cliente, Partido, Localidad y Vendedor opcional; "
            "almacena Cantidad, FechaVenta, MontoTotal y TipoVenta.",
        ],
        s["bullet"],
    )
    story += [P("4.5 Protocolo de comunicación JSON", s["h2"])]
    story += [
        P(
            "Las clases <b>MensajeSolicitud</b> y <b>MensajeRespuesta</b> serializan el protocolo TCP. "
            "Cada solicitud tiene Comando (string) y Parametros (Dictionary&lt;string,string&gt;). "
            "Cada respuesta tiene Exito (bool), Mensaje (string) y Datos (JSON opcional). "
            "<b>JsonConfig</b> usa UnsafeRelaxedJsonEscaping para preservar tildes y eñes en español.",
            s["body"],
        ),
        table(
            [
                ["Comando", "Parámetros", "Función"],
                ["PING", "—", "Verificar conectividad (responde PONG)"],
                ["VALIDAR_CLIENTE", "Identificacion", "Autenticar por cédula"],
                ["VALIDAR_CLIENTE_ID", "IdCliente", "Autenticar por Id numérico"],
                ["CONSULTAR_PARTIDOS_ACTIVOS", "—", "Listar partidos con Activo=true"],
                ["CONSULTAR_LOCALIDADES_PARTIDO", "IdPartido", "Localidades con stock > 0"],
                ["COMPRAR_EN_LINEA", "IdCliente, IdPartido, IdLocalidad, Cantidad", "Registrar venta"],
                ["CONSULTAR_COMPRAS", "IdCliente", "Historial del cliente autenticado"],
            ],
            [4.5 * cm, 4 * cm, 6 * cm],
        ),
    ]
    story += [PageBreak()]

    # --- PÁGINA 8-9: ACCESO A DATOS ---
    story += [P("5. Capa de Acceso a Datos (BOLETERIAUNED.AccesoDatos)", s["h1"])]
    story += [
        P(
            "Implementa el patrón <b>Active Record simplificado</b>: una clase AD por entidad "
            "con métodos Insertar, Consultar, ExisteId, etc. Usa Microsoft.Data.SqlClient "
            "con consultas parametrizadas (prevención de inserción SQL).",
            s["body"],
        )
    ]
    story += [P("5.1 ConexionBD", s["h2"])]
    story += [
        P(
            "Clase estática que lee la cadena <b>BoletariaUNED</b> desde ConfigurationManager "
            "(App.config). Cachea la cadena en memoria y expone CrearConexion() "
            "que retorna un SqlConnection listo para abrir.",
            s["body"],
        ),
        code(
            """Data Source=.\\SQLEXPRESS;Initial Catalog=BOLETERIAUNED;
Integrated Security=True;TrustServerCertificate=True""",
            s["code"],
        ),
    ]
    story += [P("5.2 ClienteAD — operaciones principales", s["h2"])]
    story += bullet(
        [
            "Insertar(Cliente): INSERT parametrizado con fechas normalizadas a .Date.",
            "ExisteId / ExisteIdentificacion: SELECT COUNT(1) para unicidad.",
            "ConsultarPorIdentificacion / ConsultarPorId: mapeo a objeto Cliente.",
            "ConsultarTodos / ConsultarActivos: listas ordenadas por IdCliente.",
            "MapearCliente(SqlDataReader): constructor completo del objeto entidad.",
        ],
        s["bullet"],
    )
    story += [P("5.3 VentaAD — inserción transaccional y consultas", s["h2"])]
    story += [
        P(
            "Insertar(Venta, conexion, transaccion) usa OUTPUT INSERTED.IdVenta para obtener "
            "el Id autogenerado dentro de la transacción. IdVendedor se envía como DBNull.Value "
            "cuando es venta en línea. ConsultarTodosDetalle y ConsultarPorCliente ejecutan "
            "JOINs con Cliente, Partido, Localidad y LEFT JOIN Vendedor, retornando objetos "
            "ConsultaVenta optimizados para DataGridView.",
            s["body"],
        )
    ]
    story += [P("5.4 LocalidadPorPartidoAD — inventario", s["h2"])]
    story += bullet(
        [
            "ConsultarPorPartido: solo localidades con CantidadDisponible > 0.",
            "ConsultarPorPartidoLocalidad: usado antes y durante la venta.",
            "ActualizarDisponibilidad: UPDATE atómico con condición CantidadDisponible >= @Cantidad; "
            "si filas afectadas = 0, lanza excepción de stock insuficiente.",
            "MapearCompleto: reconstruye objetos Partido y Localidad anidados.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINA 10-11: LÓGICA DE NEGOCIO ---
    story += [P("6. Capa de Lógica de Negocio", s["h1"])]
    story += [
        P(
            "Contiene las clases *LN (LocalidadLN, PartidoLN, ClienteLN, VendedorLN, "
            "LocalidadPorPartidoLN, VentaLN) y ValidadorComun. "
            "Orquesta validaciones de entidad, reglas de negocio y acceso a datos.",
            s["body"],
        )
    ]
    story += [P("6.1 ValidadorComun", s["h2"])]
    story += [
        P(
            "Clase estática con validaciones reutilizables: identificación, texto no vacío, "
            "fechas no futuras, fecha nacimiento, hora HH:mm (regex), precio > 0 y cantidad > 0. "
            "Evita duplicar lógica entre entidades y capa LN.",
            s["body"],
        )
    ]
    story += [P("6.2 Registro de entidades (patrón común)", s["h2"])]
    story += [
        P(
            "Cada método Registrar sigue el mismo flujo: (1) ValidacionEntidad.Validar(entidad), "
            "(2) verificar unicidad de Id y campos únicos, (3) validaciones cruzadas si aplica, "
            "(4) Insertar vía AD, (5) capturar SqlException y relanzar como InvalidOperationException "
            "con mensaje comprensible para el usuario.",
            s["body"],
        )
    ]
    story += [P("6.3 ClienteLN — autenticación", s["h2"])]
    story += bullet(
        [
            "ValidarClienteActivo(identificacion): valida formato, busca por identificación, "
            "verifica Activo=true o lanza excepción descriptiva.",
            "ValidarClienteActivoPorId(id): equivalente por Id numérico.",
            "Usado por ManejadorClienteTCP al iniciar sesión TCP.",
        ],
        s["bullet"],
    )
    story += [P("6.4 VentaLN — núcleo del negocio", s["h2"])]
    story += [
        P(
            "Expone dos puntos de entrada: <b>RegistrarVentaBoleteria</b> (TipoVenta='Boletería', "
            "requiere vendedor) y <b>RegistrarVentaEnLinea</b> (TipoVenta='En línea', Vendedor=null). "
            "Ambos convergen en ValidarVentaComun y EjecutarVentaTransaccional.",
            s["body"],
        )
    ]
    story += [P("6.5 ValidarVentaComun — reglas detalladas", s["h2"])]
    story += bullet(
        [
            "Cantidad > 0 (ValidadorComun).",
            "Cliente existe y Activo=true; se reemplaza el stub por el objeto completo de BD.",
            "Partido existe, Activo=true y Fecha >= DateTime.Today.",
            "Localidad existe en catálogo.",
            "Existe asociación LocalidadPorPartido y CantidadDisponible >= Cantidad solicitada.",
            "Si boletería: vendedor obligatorio y debe existir en BD.",
            "FechaVenta = DateTime.Now; MontoTotal = Cantidad × localidad.Precio.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    story += [P("6.6 Control de concurrencia", s["h2"])]
    story += [
        P(
            "Para evitar condiciones de carrera cuando dos hilos TCP o boletería+TCP venden "
            "simultáneamente la misma localidad del mismo partido, VentaLN implementa "
            "doble protección:",
            s["body"],
        ),
    ]
    story += bullet(
        [
            "<b>SemaphoreSlim</b> por clave \"{IdPartido}_{IdLocalidad}\" — serializa ventas "
            "del mismo inventario en memoria del proceso.",
            "<b>Transacción SQL</b> con revalidación de stock y UPDATE condicional "
            "(CantidadDisponible >= @Cantidad) — garantía a nivel base de datos.",
        ],
        s["bullet"],
    )
    story += [
        code(
            """semaforo.Wait();
try {
    using var transaccion = conexion.BeginTransaction();
    // Revalidar stock + ActualizarDisponibilidad + Insertar venta
    transaccion.Commit();
} catch { transaccion.Rollback(); throw; }
finally { semaforo.Release(); }""",
            s["code"],
        ),
    ]
    story += [P("6.7 LocalidadPorPartidoLN", s["h2"])]
    story += bullet(
        [
            "Verifica partido activo antes de asociar localidad.",
            "Impide duplicar par (partido, localidad).",
            "ConsultarPorPartido usado por servidor TCP y UI de ventas.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINA 12-13: SERVIDOR TCP ---
    story += [P("7. Servidor TCP multihilo", s["h1"])]
    story += [
        P(
            "La clase <b>ServidorTCP</b> escucha en 127.0.0.1:14500. Se inicia automáticamente "
            "al abrir la aplicación Servidor. Utiliza System.Net.Sockets.TcpListener y "
            "codificación UTF-8 para soportar caracteres en español.",
            s["body"],
        )
    ]
    story += [P("7.1 Ciclo de vida del servidor", s["h2"])]
    story += bullet(
        [
            "Iniciar(): crea TcpListener, Start(), lanza hilo background EscucharClientes.",
            "EscucharClientes(): bucle AcceptTcpClient(); por cada conexión crea un hilo "
            "background AtenderCliente (modelo un hilo por cliente).",
            "AtenderCliente(): lee líneas JSON con StreamReader.ReadLine(); procesa con "
            "ManejadorClienteTCP; escribe respuesta con WriteLine + AutoFlush.",
            "Detener(): _activo=false, listener.Stop(); usado al cerrar Form1.",
        ],
        s["bullet"],
    )
    story += [P("7.2 ManejadorClienteTCP", s["h2"])]
    story += [
        P(
            "Cada conexión TCP tiene su propia instancia de ManejadorClienteTCP con estado de "
            "sesión privado (_clienteSesionId). Deserializa MensajeSolicitud y enruta por switch "
            "sobre ComandosRed. Registra cada operación en la bitácora del servidor.",
            s["body"],
        )
    ]
    story += [P("7.3 Seguridad de sesión", s["h2"])]
    story += bullet(
        [
            "Tras VALIDAR_CLIENTE o VALIDAR_CLIENTE_ID se guarda _clienteSesionId.",
            "COMPRAR_EN_LINEA y CONSULTAR_COMPRAS verifican que IdCliente del parámetro "
            "coincida con la sesión (evita suplantación).",
            "CONSULTAR_PARTIDOS_ACTIVOS y CONSULTAR_LOCALIDADES_PARTIDO requieren sesión activa.",
            "Cada conexión TCP es independiente: dos clientes = dos sesiones separadas.",
        ],
        s["bullet"],
    )
    story += [P("7.4 Eventos y sincronización UI", s["h2"])]
    story += [
        P(
            "EventoBitacora actualiza lstBitacora con BeginInvoke para thread-safety WinForms. "
            "DatosActualizados refresca todos los grids cuando una venta en línea modifica BD, "
            "manteniendo al administrador sincronizado sin intervención manual.",
            s["body"],
        )
    ]
    story += [PageBreak()]

    # --- PÁGINA 14-15: APP SERVIDOR ---
    story += [P("8. Aplicación Servidor (WinForms)", s["h1"])]
    story += [
        P(
            "Form1 es el panel de administración completo. Organizado en pestañas para registro, "
            "consultas, ventas de boletería y bitácora TCP. Aplica AppleTheme para estilizado "
            "visual de grids y etiquetas de estado.",
            s["body"],
        )
    ]
    story += [P("8.1 Módulos de registro", s["h2"])]
    story += bullet(
        [
            "<b>Localidad</b>: Id, nombre, precio → LocalidadLN.Registrar.",
            "<b>Partido</b>: Id, rival, fecha (DateTimePicker), hora, checkbox activo.",
            "<b>Vendedor</b>: datos Persona + fecha ingreso.",
            "<b>Cliente</b>: datos Persona + fecha registro + checkbox activo.",
            "<b>Localidad por Partido</b>: combos de partido/localidad + cantidad disponible.",
        ],
        s["bullet"],
    )
    story += [P("8.2 Módulo de venta en boletería", s["h2"])]
    story += [
        P(
            "Combos cargados al inicio: clientes activos, todos los vendedores, partidos activos. "
            "Al seleccionar partido, cboVentaLocalidad se llena con LocalidadPorPartidoLN.ConsultarPorPartido. "
            "CalcularMontoVenta muestra precio × cantidad y entradas disponibles. "
            "btnRegistrarVenta construye Venta y llama VentaLN.RegistrarVentaBoleteria.",
            s["body"],
        )
    ]
    story += [P("8.3 Módulos de consulta", s["h2"])]
    story += [
        P(
            "Seis DataGridView de solo lectura muestran: localidades, partidos, vendedores, "
            "clientes, localidades por partido (ConsultaLocalidadPorPartido con detalle) "
            "y ventas (ConsultaVenta con joins). CargarTodasLasConsultas se ejecuta al inicio "
            "y tras cada registro o venta.",
            s["body"],
        )
    ]
    story += [P("8.4 Bitácora del servidor", s["h2"])]
    story += bullet(
        [
            "Conexiones y desconexiones TCP con endpoint remoto.",
            "Comandos recibidos y resultados (cliente validado, venta procesada).",
            "Errores capturados durante procesamiento de solicitudes.",
            "Consultas ejecutadas desde la UI administrativa.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINA 16-17: APP CLIENTE ---
    story += [P("9. Aplicación Cliente en línea (WinForms)", s["h1"])]
    story += [
        P(
            "Aplicación independiente que se comunica exclusivamente vía TCP. "
            "No accede directamente a la base de datos. La clase ClienteRed encapsula "
            "TcpClient, conexión, serialización JSON y lectura de respuestas.",
            s["body"],
        )
    ]
    story += [P("9.1 Flujo de autenticación", s["h2"])]
    story += bullet(
        [
            "Al iniciar, muestra MessageBox instructivo y deshabilita funciones de compra.",
            "btnConectar: establece TCP, envía PING, actualiza lblEstadoConexion.",
            "btnValidarCliente: envía VALIDAR_CLIENTE (identificación) o VALIDAR_CLIENTE_ID.",
            "Deserializa Cliente de respuesta.Datos; habilita compras y consultas.",
            "Bloquea campos de identificación tras autenticación exitosa.",
        ],
        s["bullet"],
    )
    story += [P("9.2 Flujo de compra", s["h2"])]
    story += bullet(
        [
            "CargarPartidosActivos → CONSULTAR_PARTIDOS_ACTIVOS → combo DescripcionPartido.",
            "Al cambiar partido → CONSULTAR_LOCALIDADES_PARTIDO → combo DescripcionLocalidad.",
            "CalcularMontoCompra: precio unitario × NumericUpDown cantidad.",
            "btnComprar → COMPRAR_EN_LINEA con IdCliente autenticado.",
            "Tras éxito: recarga localidades (stock actualizado) y grid de compras.",
        ],
        s["bullet"],
    )
    story += [P("9.3 Consulta de compras", s["h2"])]
    story += [
        P(
            "CONSULTAR_COMPRAS retorna JSON deserializado por ConsultaComprasParser.DesdeJson "
            "en una List&lt;ConsultaVenta&gt; enlazada al dgvCompras. "
            "ConsultaComprasParser.ResumenCantidad genera el texto informativo del panel.",
            s["body"],
        )
    ]
    story += [P("9.4 ClienteRed — detalle de implementación", s["h2"])]
    story += [
        code(
            """public MensajeRespuesta EnviarSolicitud(MensajeSolicitud solicitud) {
    _escritor.WriteLine(solicitud.Serializar());
    var respuestaJson = _lector.ReadLine();
    return MensajeRespuesta.Deserializar(respuestaJson);
}""",
            s["code"],
        ),
        P(
            "Protocolo request-response síncrono: cada solicitud espera exactamente una línea de "
            "respuesta. Desconectar limpia reader, writer, stream y TcpClient.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    # --- PÁGINA 18: VALIDACIONES ---
    story += [P("10. Catálogo completo de validaciones", s["h1"])]
    story += [
        table(
            [
                ["Regla", "Capa", "Mensaje / Comportamiento"],
                ["Identificación ≤ 10 chars", "Persona / ValidadorComun", "ArgumentException"],
                ["Fecha nacimiento < hoy", "Persona", "ArgumentException"],
                ["Fecha registro ≤ hoy", "Cliente", "ArgumentException"],
                ["Fecha ingreso > nacimiento", "Vendedor", "ArgumentException"],
                ["Rival no vacío", "Partido", "ArgumentException"],
                ["Fecha partido ≥ hoy", "Partido / VentaLN", "No ventas pasadas"],
                ["Hora formato HH:mm", "Partido", "Regex 00:00–23:59"],
                ["Precio > 0", "Localidad", "ArgumentException"],
                ["Cantidad > 0", "Venta / ValidadorComun", "ArgumentException"],
                ["Id único al registrar", "Todas LN", "InvalidOperationException"],
                ["Identificación única", "Cliente/Vendedor LN", "InvalidOperationException"],
                ["Cliente activo para comprar", "ClienteLN / VentaLN", "InvalidOperationException"],
                ["Stock suficiente", "VentaLN / AD", "InvalidOperationException"],
                ["Sesión TCP requerida", "ManejadorClienteTCP", "Debe autenticarse"],
                ["IdCliente = sesión", "ManejadorClienteTCP", "No autorizado otro cliente"],
            ],
            [4 * cm, 3.5 * cm, 7 * cm],
        )
    ]
    story += [Spacer(1, 0.5 * cm)]
    story += [P("10.1 Manejo de errores", s["h2"])]
    story += [
        P(
            "Las excepciones en lógica de negocio usan InvalidOperationException con mensajes "
            "orientados al usuario final en español. La UI captura Exception y muestra MessageBox. "
            "En TCP, ManejadorClienteTCP captura cualquier excepción y retorna MensajeRespuesta.Error "
            "serializado, manteniendo la conexión activa para nuevas solicitudes.",
            s["body"],
        )
    ]
    story += [PageBreak()]

    # --- PÁGINA 19: INSTALACIÓN ---
    story += [P("11. Configuración, compilación y ejecución", s["h1"])]
    story += [P("11.1 Requisitos del entorno", s["h2"])]
    story += bullet(
        [
            "Windows 10/11 con .NET 10 SDK.",
            "Microsoft SQL Server Express (instancia .\\SQLEXPRESS).",
            "Visual Studio 2022 o superior con carga de trabajo .NET desktop.",
            "Ejecutar script SQL de creación de BD y DatosPrueba.sql.",
        ],
        s["bullet"],
    )
    story += [P("11.2 Pasos de instalación", s["h2"])]
    story += bullet(
        [
            "1. Abrir BOLETERIAUNED.slnx en Visual Studio.",
            "2. Ejecutar Script Base Datos BOLETERIAUNED proyecto 2.sql en SSMS.",
            "3. Ejecutar DatosPrueba.sql para poblar datos de demostración.",
            "4. Verificar App.config: cadena BoletariaUNED apunta a su instancia SQL.",
            "5. Compilar solución (5 proyectos) en configuración Debug.",
            "6. Iniciar BOLETERIAUNED.Servidor (inicia TCP automáticamente).",
            "7. Iniciar BOLETERIAUNED.Cliente y conectar a 127.0.0.1:14500.",
        ],
        s["bullet"],
    )
    story += [P("11.3 Prueba funcional sugerida", s["h2"])]
    story += bullet(
        [
            "Cliente: identificación 108870456 (Johan Ramírez, activo) → validar → comprar 2 entradas "
            "Sol para partido vs TEC.",
            "Servidor: verificar venta en grid, stock decrementado, bitácora TCP.",
            "Cliente inactivo 107770654 (Carlos Vega) → debe rechazar validación.",
            "Intentar comprar más entradas que disponibles → mensaje de error.",
            "Consultar compras del cliente autenticado en dgvCompras.",
        ],
        s["bullet"],
    )
    story += [P("11.4 Dependencias NuGet", s["h2"])]
    story += bullet(
        [
            "Microsoft.Data.SqlClient — AccesoDatos y LogicaNegocio.",
            "System.Configuration.ConfigurationManager — lectura App.config.",
            "System.Text.Json — serialización del protocolo (incluido en .NET).",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    # --- PÁGINAS ADICIONALES: DETALLE TÉCNICO ---
    story += [P("13. Detalle de clases de acceso a datos restantes", s["h1"])]
    story += [P("13.1 PartidoAD y LocalidadAD", s["h2"])]
    story += bullet(
        [
            "PartidoAD.Insertar persiste IdPartido, Rival, Fecha (.Date), Hora y Activo.",
            "ConsultarActivos filtra WHERE Activo = 1; usado en combos de venta y cliente TCP.",
            "LocalidadAD mantiene catálogo maestro de secciones; precio decimal(10,2).",
            "Ambas clases implementan ExisteId antes del registro desde la capa LN.",
        ],
        s["bullet"],
    )
    story += [P("13.2 VendedorAD", s["h2"])]
    story += [
        P(
            "Similar a ClienteAD pero sin campo Activo. ConsultarPorId alimenta la validación "
            "de ventas en boletería. La identificación del vendedor también es única en base de datos "
            "mediante restricción UQ_Vendedor, reforzada en VendedorLN.ExisteIdentificacion.",
            s["body"],
        )
    ]
    story += [P("13.3 ConsultaLocalidadPorPartido y ConsultaVenta (DTOs)", s["h2"])]
    story += [
        P(
            "Son clases planas sin lógica, diseñadas exclusivamente para enlazar DataGridView "
            "sin exponer objetos anidados. ConsultaLocalidadPorPartido incluye columnas "
            "PartidoRival, PartidoFecha, PartidoHora, PartidoActivo (como 'Sí'/'No'), "
            "LocalidadNombre, LocalidadPrecio y CantidadDisponible. ConsultaVenta agrega "
            "ClienteNombre, VendedorNombre (o 'Sin vendedor') y TipoVenta para distinguir "
            "origen de la transacción.",
            s["body"],
        )
    ]
    story += [PageBreak()]

    story += [P("14. Ejemplos del protocolo JSON/TCP", s["h1"])]
    story += [P("14.1 Solicitud PING", s["h2"])]
    story += [
        code(
            """{"comando":"PING","parametros":{}}
→ Respuesta: {"exito":true,"mensaje":"PONG","datos":null}""",
            s["code"],
        )
    ]
    story += [P("14.2 Validación de cliente", s["h2"])]
    story += [
        code(
            """{"comando":"VALIDAR_CLIENTE","parametros":{"Identificacion":"108870456"}}
→ {"exito":true,"mensaje":"Cliente validado correctamente.",
   "datos":"{\"id\":1,\"identificacion\":\"108870456\",\"nombre\":\"Johan\",...}"}""",
            s["code"],
        )
    ]
    story += [P("14.3 Compra en línea", s["h2"])]
    story += [
        code(
            """{"comando":"COMPRAR_EN_LINEA","parametros":{
  "IdCliente":"1","IdPartido":"2","IdLocalidad":"3","Cantidad":"2"}}
→ {"exito":true,"mensaje":"Compra registrada correctamente. Número de venta: 4",
   "datos":"{\"idVenta\":4,\"montoTotal\":10000.00}"}""",
            s["code"],
        ),
        P(
            "Cada mensaje viaja como una única línea UTF-8 terminada en salto de línea. "
            "El cliente debe mantener la conexión abierta entre solicitudes para preservar "
            "la sesión autenticada en el ManejadorClienteTCP del hilo correspondiente.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("15. Interfaz gráfica y AppleTheme", s["h1"])]
    story += [
        P(
            "Ambas aplicaciones WinForms aplican la clase estática <b>AppleTheme</b> para "
            "unificar colores, tipografía y estilo de controles. Define paleta con fondos "
            "claros, acentos azul oscuro (#0f3460), texto secundario gris y verde para estados "
            "exitosos. EstilizarGrid configura encabezados, filas alternas y selección; "
            "EstilizarEstado colorea lblEstadoConexion según conexión activa o inactiva.",
            s["body"],
        )
    ]
    story += [P("15.1 Form1 Servidor — organización por pestañas", s["h2"])]
    story += bullet(
        [
            "Registro: formularios compactos con NumericUpDown para Ids y DateTimePicker para fechas.",
            "Consultas: botones que recargan grids sin modificar datos.",
            "Venta boletería: flujo guiado cliente → vendedor → partido → localidad → cantidad.",
            "Bitácora: ListBox con inserción al inicio (eventos más recientes arriba).",
        ],
        s["bullet"],
    )
    story += [P("15.2 Form1 Cliente — estados de la UI", s["h2"])]
    story += bullet(
        [
            "Estado inicial: solo conexión y validación habilitados.",
            "Post-autenticación: combos de compra, grid de historial y botón consultar.",
            "Desconexión: resetea sesión, limpia autenticación y rehabilita campos de Id.",
            "FormClosing: Desconectar() garantiza cierre limpio del socket.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    story += [P("16. Diagramas de secuencia", s["h1"])]
    story += [P("16.1 Venta en boletería (local)", s["h2"])]
    story += [
        code(
            """Usuario → Form1.Servidor → VentaLN → ValidarVentaComun
  → EjecutarVentaTransaccional → [Semáforo] → SQL Transaction
      → LocalidadPorPartidoAD.ActualizarDisponibilidad
      → VentaAD.Insertar → Commit → Release Semáforo
  ← IdVenta → MessageBox + refresco grids""",
            s["code"],
        )
    ]
    story += [P("16.2 Compra en línea (TCP)", s["h2"])]
    story += [
        code(
            """Cliente.Form1 → ClienteRed.EnviarSolicitud(COMPRAR_EN_LINEA)
  → TCP → ServidorTCP.AtenderCliente → ManejadorClienteTCP
      → VerificarSesionCliente → VentaLN.RegistrarVentaEnLinea
      → (misma transacción que boletería)
  ← MensajeRespuesta JSON → Cliente muestra éxito + recarga compras
  → Servidor dispara DatosActualizados → grids admin actualizados""",
            s["code"],
        )
    ]
    story += [P("16.3 Principios POO aplicados — resumen", s["h2"])]
    story += [
        table(
            [
                ["Principio", "Implementación en el proyecto"],
                ["Encapsulamiento", "Campos privados en ServidorTCP; sesión privada en ManejadorClienteTCP"],
                ["Herencia", "Persona → Cliente, Vendedor con ValidarPersonaBase compartido"],
                ["Polimorfismo", "IValidable.Validar() invocado vía ValidacionEntidad"],
                ["Abstracción", "Persona abstracta; ConexionBD oculta detalle de conexión"],
                ["Composición", "Venta contiene Cliente, Partido, Localidad; LocalidadPorPartido compone ambos"],
            ],
            [3.5 * cm, 12 * cm],
        )
    ]
    story += [PageBreak()]

    # --- SECCIÓN EXTENDIDA: CÓDIGO ARCHIVO POR ARCHIVO ---
    story += [P("17. Análisis del código — Capa Entidades (archivo por archivo)", s["h1"])]
    story += [P("17.1 Persona.cs — clase abstracta base", s["h2"])]
    story += [
        P(
            "Archivo: <b>BOLETERIAUNED.Entidades/Persona.cs</b>. Define la abstracción común "
            "entre Cliente y Vendedor. Las propiedades Id, Identificacion, Nombre, Apellido y "
            "FechaNacimiento son públicas con get/set automáticos. La propiedad calculada "
            "<b>NombreCompleto</b> concatena nombre y apellido con Trim() para evitar espacios "
            "sobrantes; se usa en combos del servidor y en la bienvenida del cliente TCP.",
            s["body"],
        ),
        P(
            "El método abstracto <b>ObtenerTipoPersona()</b> obliga a cada subclase a declarar "
            "su tipo ('Cliente' o 'Vendedor'), cumpliendo el contrato de herencia. "
            "<b>ValidarPersonaBase()</b> es protected: solo las subclases lo invocan desde "
            "su Validar(). Lanza ArgumentException con mensajes en español si falla identificación, "
            "nombre vacío o fecha de nacimiento >= hoy.",
            s["body"],
        ),
    ]
    story += [P("17.2 Cliente.cs", s["h2"])]
    story += bullet(
        [
            "Hereda Persona e implementa IValidable.",
            "Agrega FechaRegistro y Activo (bool); el constructor de 7 parámetros inicializa todo.",
            "Validar() llama ValidarPersonaBase() y verifica FechaRegistro <= DateTime.Today.",
            "Activo no se valida en Validar() porque un cliente inactivo puede existir en BD; "
            "la restricción de compra ocurre en ClienteLN y VentaLN.",
        ],
        s["bullet"],
    )
    story += [P("17.3 Vendedor.cs", s["h2"])]
    story += bullet(
        [
            "Agrega FechaIngreso; Validar() exige que sea posterior a FechaNacimiento y no futura.",
            "No tiene campo Activo: todo vendedor registrado puede procesar ventas en boletería.",
            "ObtenerTipoPersona() retorna literal 'Vendedor'.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    story += [P("17.4 Partido.cs, Localidad.cs y LocalidadPorPartido.cs", s["h2"])]
    story += [
        P(
            "<b>Partido.cs</b>: IdPartido es la PK manual (no IDENTITY). DescripcionPartido formatea "
            "'Rival - dd/MM/yyyy HH:mm' para DisplayMember en ComboBox. Validar() usa Regex "
            "para hora militar HH:mm. La validación Fecha >= hoy impide registrar partidos "
            "ya jugados desde la UI de registro.",
            s["body"],
        ),
        P(
            "<b>Localidad.cs</b>: catálogo de secciones del estadio. ToString() retorna "
            "'Nombre - Col. precio' con formato N2. Precio se usa en VentaLN para calcular MontoTotal.",
            s["body"],
        ),
        P(
            "<b>LocalidadPorPartido.cs</b>: modelo compuesto con objetos Partido y Localidad "
            "anidados (no solo IDs). DescripcionLocalidad muestra nombre, precio y disponibles "
            "para el combo de compra. Validar() solo exige CantidadDisponible > 0 al registrar.",
            s["body"],
        ),
    ]
    story += [P("17.5 Venta.cs — composición de objetos", s["h2"])]
    story += [
        P(
            "Venta es la entidad más rica: contiene referencias a Cliente, Partido, Localidad "
            "y Vendedor opcional (nullable). Antes de insertar, VentaLN reemplaza los stubs "
            "(solo Id) por objetos completos cargados de BD. TipoVenta se asigna en LN, no en UI. "
            "FechaVenta y MontoTotal se calculan en ValidarVentaComun, no los ingresa el usuario.",
            s["body"],
        ),
        code(
            """// Construcción típica en boletería (Form1 Servidor):
var venta = new Venta {
    Cliente = cliente,      // del ComboBox
    Vendedor = vendedor,
    Partido = partido,
    Localidad = lp.Localidad,
    Cantidad = (int)numVentaCantidad.Value
};
_ventaLN.RegistrarVentaBoleteria(venta);""",
            s["code"],
        ),
    ]
    story += [P("17.6 IValidable.cs y ValidacionEntidad.cs", s["h2"])]
    story += [
        P(
            "<b>IValidable</b> declara un único método void Validar(). "
            "<b>ValidacionEntidad</b> es estática y actúa como fachada polimórfica: "
            "Validar(IValidable entidad) delega en entidad.Validar(); ValidarVarios recorre "
            "un array params y valida cada uno. VentaLN invoca ValidacionEntidad.Validar(venta) "
            "sin importar si la venta proviene de boletería o TCP.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("17.7 Protocolo JSON: JsonConfig, MensajeSolicitud, MensajeRespuesta, ComandosRed", s["h2"])]
    story += bullet(
        [
            "<b>JsonConfig.Opciones</b>: JsonSerializerDefaults.Web + UnsafeRelaxedJsonEscaping "
            "para que 'Ramírez' no se escape como \\u00ed en la red.",
            "<b>MensajeSolicitud</b>: Comando + Dictionary Parametros; Serializar/Deserializar "
            "envuelven System.Text.Json con las opciones compartidas.",
            "<b>MensajeRespuesta</b>: factory methods Ok(mensaje, datos?) y Error(mensaje); "
            "Datos es string JSON embebido (no objeto anidado) para flexibilidad.",
            "<b>ComandosRed</b>: constantes string evitan typos; el switch en ManejadorClienteTCP "
            "usa pattern matching sobre estas constantes.",
        ],
        s["bullet"],
    )
    story += [P("17.8 ConsultaComprasParser.cs y DTOs de consulta", s["h2"])]
    story += [
        P(
            "<b>ConsultaComprasParser.DesdeJson</b> maneja respuesta vacía o 'null' retornando "
            "lista vacía (evita NullReference en el grid). ResumenCantidad genera texto amigable. "
            "<b>ConsultaVenta</b> y <b>ConsultaLocalidadPorPartido</b> son POCOs sin métodos: "
            "sus propiedades coinciden con alias SQL o columnas del reader.",
            s["body"],
        ),
    ]
    story += [P("17.9 EncabezadoProyecto.cs", s["h2"])]
    story += [
        P(
            "Clase estática con constantes de documentación UNED (Universidad, Curso, Estudiante, "
            "Fecha). Aparece en comentarios de encabezado de cada archivo .cs del proyecto "
            "como requisito académico de identificación del trabajo.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("18. Análisis del código — Capa Acceso a Datos", s["h1"])]
    story += [P("18.1 ConexionBD.cs", s["h2"])]
    story += [
        P(
            "Lee <b>ConfigurationManager.ConnectionStrings['BoletariaUNED']</b> del App.config "
            "del proyecto Servidor (único que referencia System.Configuration). Cachea en "
            "_cadenaConexion estática para no reparsear XML en cada operación. CrearConexion() "
            "retorna SqlConnection sin abrir; cada método AD abre y cierra con using.",
            s["body"],
        ),
    ]
    story += [P("18.2 ClienteAD.cs — métodos y mapeo", s["h2"])]
    story += [
        table(
            [
                ["Método", "SQL / Comportamiento"],
                ["Insertar", "INSERT parametrizado; fechas .Date"],
                ["ExisteId / ExisteIdentificacion", "COUNT(1) → bool"],
                ["ConsultarPorIdentificacion", "WHERE Identificacion; usado en login TCP"],
                ["ConsultarPorId", "WHERE IdCliente; usado en ventas"],
                ["ConsultarActivos", "WHERE Activo=1; combo venta boletería"],
                ["MapearCliente", "SqlDataReader → constructor Cliente"],
            ],
            [4.5 * cm, 11 * cm],
        )
    ]
    story += [P("18.3 VentaAD.cs — transacciones y JOINs", s["h2"])]
    story += [
        P(
            "<b>Insertar</b> recibe conexión y transacción externas (no crea las suyas): "
            "permite atomicidad con ActualizarDisponibilidad. Usa OUTPUT INSERTED.IdVenta "
            "para obtener el ID generado por IDENTITY. @IdVendedor recibe DBNull.Value "
            "cuando venta.Vendedor es null (venta en línea).",
            s["body"],
        ),
        P(
            "Las consultas detalladas usan INNER JOIN Cliente, Partido, Localidad y "
            "LEFT JOIN Vendedor. CASE WHEN IdVendedor IS NULL THEN 'Sin vendedor' "
            "normaliza la columna para el grid. LeerConsultaDetalle mapea por índice "
            "posicional (GetInt32(0), GetString(2), etc.).",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("18.4 LocalidadPorPartidoAD.cs — inventario crítico", s["h2"])]
    story += bullet(
        [
            "ConsultarPorPartido filtra CantidadDisponible > 0: el cliente en línea no ve agotadas.",
            "ConsultarPorPartidoLocalidad: JOIN triple para reconstruir objetos anidados.",
            "ActualizarDisponibilidad: UPDATE con guard CantidadDisponible >= @Cantidad; "
            "ExecuteNonQuery retorna 0 filas si otro hilo consumió el stock → excepción.",
            "MapearCompleto vs MapearConsulta: uno retorna entidad rica, otro DTO plano.",
        ],
        s["bullet"],
    )
    story += [P("18.5 PartidoAD.cs, LocalidadAD.cs, VendedorAD.cs", s["h2"])]
    story += [
        P(
            "<b>PartidoAD.ConsultarActivos</b> filtra además Fecha >= CAST(GETDATE() AS DATE) "
            "en SQL (doble filtro con la validación en VentaLN). "
            "<b>LocalidadAD</b> expone Insertar, ExisteId, ConsultarPorId, ConsultarTodos. "
            "<b>VendedorAD</b> es análogo a ClienteAD sin ConsultarActivos ni campo Activo.",
            s["body"],
        ),
    ]
    story += [P("18.6 Patrón común en todas las clases AD", s["h2"])]
    story += bullet(
        [
            "using var conexion = ConexionBD.CrearConexion() garantiza Dispose.",
            "SqlCommand con parámetros @Nombre (nunca concatenación de strings).",
            "ExecuteNonQuery para INSERT/UPDATE; ExecuteScalar para COUNT; ExecuteReader para SELECT.",
            "Mapeo privado estático desde SqlDataReader a constructor de entidad.",
        ],
        s["bullet"],
    )
    story += [PageBreak()]

    story += [P("19. Análisis del código — Capa Lógica de Negocio", s["h1"])]
    story += [P("19.1 ValidadorComun.cs", s["h2"])]
    story += [
        P(
            "Centraliza validaciones que se repiten fuera de las entidades: "
            "ValidarIdentificacion (usado en ClienteLN.ValidarClienteActivo), ValidarCantidad, "
            "ValidarHora, ValidarPrecio, ValidarFechaNacimiento. Separar esto de Persona.Validar "
            "permite validar identificación de login sin construir un Cliente completo.",
            s["body"],
        ),
    ]
    story += [P("19.2 ClienteLN.cs — flujo de Registrar y autenticación", s["h2"])]
    story += [
        code(
            """Registrar(cliente):
  1. ValidacionEntidad.Validar(cliente)
  2. if ExisteId → throw "IdCliente ya existe"
  3. if ExisteIdentificacion → throw duplicado
  4. _accesoDatos.Insertar(cliente)
  5. catch SqlException → InvalidOperationException

ValidarClienteActivo(identificacion):
  1. ValidadorComun.ValidarIdentificacion
  2. ConsultarPorIdentificacion → null → "no registrado"
  3. !Activo → "no activo"
  4. return cliente""",
            s["code"],
        )
    ]
    story += [P("19.3 PartidoLN, LocalidadLN, VendedorLN", s["h2"])]
    story += [
        P(
            "Las tres clases siguen el patrón idéntico: Validar → ExisteId → Insertar → "
            "ConsultarTodos. PartidoLN agrega ConsultarActivos delegando a PartidoAD. "
            "LocalidadPorPartidoLN es más compleja: valida partido activo, localidad existente "
            "y unicidad del par (partido, localidad) antes de insertar.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("19.4 VentaLN.cs — método por método", s["h2"])]
    story += [
        table(
            [
                ["Método", "Qué hace"],
                ["RegistrarVentaBoleteria", "TipoVenta='Boletería'; requiere vendedor"],
                ["RegistrarVentaEnLinea", "TipoVenta='En línea'; Vendedor=null"],
                ["ValidarVentaComun", "Carga entidades de BD; valida reglas; calcula monto"],
                ["EjecutarVentaTransaccional", "Semáforo + transacción SQL atómica"],
                ["ObtenerSemaforo", "Dictionary thread-safe de SemaphoreSlim(1,1)"],
                ["ConsultarTodosDetalle", "Delega a VentaAD para grid servidor"],
                ["ConsultarPorCliente", "Historial para comando TCP CONSULTAR_COMPRAS"],
            ],
            [5 * cm, 10.5 * cm],
        )
    ]
    story += [
        P(
            "El semáforo se obtiene con lock(BloqueoSemáforos) para crear entradas del "
            "Dictionary de forma thread-safe. La clave '{IdPartido}_{IdLocalidad}' asegura "
            "que ventas de distintas localidades no se bloqueen entre sí, pero ventas "
            "concurrentes del mismo inventario se serializan.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("20. Análisis del código — Servidor TCP y ManejadorClienteTCP", s["h1"])]
    story += [P("20.1 ServidorTCP.cs — campos y hilos", s["h2"])]
    story += bullet(
        [
            "_listener: TcpListener en 127.0.0.1:14500 (constantes públicas DireccionIp, Puerto).",
            "_activo: volatile bool controla el bucle de escucha.",
            "_hiloEscucha: Thread background 'HiloEscuchaTCP' que ejecuta AcceptTcpClient.",
            "EventoBitacora y DatosActualizados: eventos C# para UI thread-safe.",
            "AtenderCliente: using anidados sobre TcpClient, NetworkStream, StreamReader/Writer UTF-8.",
        ],
        s["bullet"],
    )
    story += [P("20.2 Bucle ReadLine del protocolo", s["h2"])]
    story += [
        code(
            """while ((linea = lector.ReadLine()) != null) {
    if (string.IsNullOrWhiteSpace(linea)) continue;
    var respuesta = manejador.ProcesarSolicitud(linea);
    escritor.WriteLine(respuesta);
}
// Al cerrar el cliente, ReadLine retorna null → sale del bucle""",
            s["code"],
        ),
        P(
            "Un ManejadorClienteTCP se crea por conexión; su _clienteSesionId persiste "
            "mientras dure el socket. Múltiples solicitudes en la misma conexión comparten sesión.",
            s["body"],
        ),
    ]
    story += [P("20.3 ManejadorClienteTCP — enrutamiento ProcesarSolicitud", s["h2"])]
    story += [
        P(
            "Deserializa JSON → switch por Comando → método privado → serializa MensajeRespuesta. "
            "El catch global convierte cualquier excepción en respuesta JSON de error sin "
            "cerrar la conexión. ObtenerParametro y ParsearEntero son helpers estáticos "
            "que lanzan ArgumentException si falta o es inválido un parámetro.",
            s["body"],
        ),
    ]
    story += [P("20.4 Métodos privados del manejador", s["h2"])]
    story += [
        table(
            [
                ["Método", "Lógica"],
                ["ValidarCliente", "ClienteLN.ValidarClienteActivo; guarda sesión; retorna JSON cliente"],
                ["ValidarClientePorId", "Igual por Id numérico"],
                ["ComprarEnLinea", "VerificarSesionCliente; construye Venta; VentaLN.RegistrarVentaEnLinea"],
                ["ConsultarCompras", "VerificarSesionCliente; VentaLN.ConsultarPorCliente"],
                ["ConsultarPartidosActivos", "VerificarSesionActiva; PartidoLN.ConsultarActivos"],
                ["ConsultarLocalidadesPartido", "VerificarSesionActiva; LocalidadPorPartidoLN.ConsultarPorPartido"],
                ["VerificarSesionCliente", "Sesión activa + IdCliente == _clienteSesionId"],
            ],
            [4.5 * cm, 11 * cm],
        )
    ]
    story += [PageBreak()]

    story += [P("21. Análisis del código — Aplicación Cliente", s["h1"])]
    story += [P("21.1 ServidorConfig.cs y ClienteRed.cs", s["h2"])]
    story += [
        P(
            "<b>ServidorConfig</b> duplica IP y puerto del ServidorTCP como constantes "
            "para que ClienteRed.Conectar() use valores por defecto sin hardcodear en Form1. "
            "<b>ClienteRed</b> mantiene TcpClient, NetworkStream, StreamReader y StreamWriter "
            "como campos privados. EstaConectado consulta _cliente?.Connected.",
            s["body"],
        ),
        P(
            "<b>Conectar()</b> llama Desconectar() primero (limpia conexión previa), crea "
            "TcpClient, Connect(ip, puerto), inicializa streams UTF-8 con AutoFlush en escritor. "
            "<b>EnviarSolicitud()</b> es síncrono: WriteLine + ReadLine; IOException se "
            "envuelve en InvalidOperationException.",
            s["body"],
        ),
    ]
    story += [P("21.2 Form1.cs (Cliente) — métodos principales", s["h2"])]
    story += [
        table(
            [
                ["Método / Evento", "Funcionamiento"],
                ["Constructor", "InitializeComponent; ConfigurarInterfazInicial; MessageBox guía"],
                ["HabilitarFuncionalidades", "Activa/desactiva compra según autenticación"],
                ["btnConectar_Click", "Toggle conectar/desconectar; PING de prueba"],
                ["btnValidarCliente_Click", "VALIDAR_CLIENTE o VALIDAR_CLIENTE_ID; deserializa Cliente"],
                ["CargarPartidosActivos", "CONSULTAR_PARTIDOS_ACTIVOS → cboCompraPartido"],
                ["cboCompraPartido_SelectedIndexChanged", "CONSULTAR_LOCALIDADES_PARTIDO → combo localidad"],
                ["CalcularMontoCompra", "Precio × numCompraCantidad; muestra disponibles"],
                ["btnComprar_Click", "COMPRAR_EN_LINEA; recarga stock y compras"],
                ["CargarCompras", "CONSULTAR_COMPRAS → dgvCompras vía ConsultaComprasParser"],
                ["Form1_FormClosing", "_clienteRed.Desconectar()"],
            ],
            [5 * cm, 10.5 * cm],
        )
    ]
    story += [PageBreak()]

    story += [P("22. Análisis del código — Aplicación Servidor (Form1.cs)", s["h1"])]
    story += [P("22.1 Constructor y ciclo de vida", s["h2"])]
    story += [
        P(
            "Al instanciar Form1: InitializeComponent → ConfigurarGrids → suscribe EventoBitacora "
            "y DatosActualizados → _servidorTcp.Iniciar() (TCP arranca automáticamente) → "
            "CargarCombosVenta → CargarTodasLasConsultas. Al cerrar: Form1_FormClosing "
            "llama _servidorTcp.Detener().",
            s["body"],
        ),
    ]
    story += [P("22.2 Handlers de registro (btnRegistrar*)", s["h2"])]
    story += bullet(
        [
            "Cada handler lee controles WinForms, construye entidad, llama *LN.Registrar.",
            "Éxito: MessageBox + AgregarBitacora + CargarCombosVenta + CargarTodasLasConsultas + Limpiar*.",
            "Error: MessageBox Warning + bitácora con mensaje de excepción.",
            "btnRegistrarLp valida SelectedItem de combos como Partido y Localidad.",
        ],
        s["bullet"],
    )
    story += [P("22.3 Venta en boletería en Form1", s["h2"])]
    story += [
        code(
            """cboVentaPartido_SelectedIndexChanged:
  → LocalidadPorPartidoLN.ConsultarPorPartido(id)
  → cboVentaLocalidad.DataSource = localidades

CalcularMontoVenta:
  → lp.Localidad.Precio * numVentaCantidad.Value

btnRegistrarVenta_Click:
  → new Venta { Cliente, Vendedor, Partido, Localidad, Cantidad }
  → _ventaLN.RegistrarVentaBoleteria(venta)""",
            s["code"],
        )
    ]
    story += [P("22.4 Consultas y bitácora thread-safe", s["h2"])]
    story += [
        P(
            "<b>MostrarConsulta&lt;T&gt;</b> asigna List&lt;T&gt; al DataSource del grid. "
            "<b>AgregarBitacora</b> verifica InvokeRequired: si el hilo TCP escribe, usa "
            "BeginInvoke para actualizar lstBitacora en el hilo UI. Insert(0, mensaje) "
            "muestra eventos recientes arriba. <b>ActualizarConsultasDesdeEvento</b> "
            "se dispara tras ventas en línea para sincronizar inventario visible del admin.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("23. Análisis del código — AppleTheme y Program.cs", s["h1"])]
    story += [P("23.1 AppleTheme.cs — sistema de diseño UI", s["h2"])]
    story += bullet(
        [
            "Paleta: Fondo #F5F5F7, Tarjeta blanca, Acento azul #0071E3, Exito verde, Error rojo.",
            "CrearShell: encabezado fijo 72px + área contenido con título y subtítulo UNED.",
            "CrearTarjetaApilada: panel con borde pintado en evento Paint; layout TableLayoutPanel.",
            "CrearFormulario / AgregarCampo / AgregarBoton: filas de 40px, etiqueta 145px.",
            "EstilizarGrid: filas alternas, encabezado gris, selección azul claro.",
            "ConfigurarGridCompras: columnas manuales con DataPropertyName y formatos fecha/monto.",
            "EnlazarContenidoScroll: recalcula altura al resize para scroll vertical correcto.",
        ],
        s["bullet"],
    )
    story += [P("23.2 Program.cs (Servidor y Cliente)", s["h2"])]
    story += [
        P(
            "Ambos proyectos WinForms usan el template estándar: [STAThread] en Main, "
            "ApplicationConfiguration.Initialize(), Application.Run(new Form1()). "
            "STAThread es requerido por controles Windows Forms. No hay lógica adicional "
            "en Program.cs; toda la inicialización ocurre en constructores de Form1.",
            s["body"],
        ),
    ]
    story += [P("23.3 Referencias entre proyectos (.csproj)", s["h2"])]
    story += [
        table(
            [
                ["Proyecto", "Referencia a"],
                ["Entidades", "(ninguna del dominio)"],
                ["AccesoDatos", "Entidades, Microsoft.Data.SqlClient, ConfigurationManager"],
                ["LogicaNegocio", "AccesoDatos, Entidades"],
                ["Servidor", "LogicaNegocio, Entidades (+ WinForms)"],
                ["Cliente", "Entidades (+ WinForms; sin AccesoDatos directo)"],
            ],
            [4 * cm, 11.5 * cm],
        )
    ]
    story += [PageBreak()]

    story += [P("24. Trazabilidad: del clic del usuario al SQL", s["h1"])]
    story += [P("24.1 Ejemplo completo — compra en línea de 2 entradas Sol", s["h2"])]
    story += bullet(
        [
            "1. Usuario autenticado presiona Comprar en Cliente.Form1.",
            "2. btnComprar_Click arma MensajeSolicitud COMPRAR_EN_LINEA con IdCliente=1, IdPartido=2, IdLocalidad=3, Cantidad=2.",
            "3. ClienteRed serializa JSON y envía por TCP.",
            "4. ServidorTCP.AtenderCliente recibe la línea en su hilo.",
            "5. ManejadorClienteTCP.ComprarEnLinea verifica sesión Id=1.",
            "6. VentaLN.RegistrarVentaEnLinea: Validar → ValidarVentaComun (cliente activo, partido TEC activo, stock Sol >= 2).",
            "7. MontoTotal = 2 × 5000 = 10000; TipoVenta = 'En línea'.",
            "8. Semáforo '2_3'.Wait(); BEGIN TRANSACTION.",
            "9. LocalidadPorPartidoAD.ActualizarDisponibilidad: UPDATE CantidadDisponible -= 2.",
            "10. VentaAD.Insertar: INSERT con IdVendedor NULL.",
            "11. COMMIT; semáforo.Release(); DatosActualizados(); respuesta JSON con IdVenta.",
            "12. Cliente muestra MessageBox, recarga localidades y dgvCompras.",
        ],
        s["bullet"],
    )
    story += [P("24.2 Ejemplo — rechazo de cliente inactivo", s["h2"])]
    story += [
        P(
            "Identificación 107770654 (Carlos Vega, Activo=0 en DatosPrueba.sql). "
            "btnValidarCliente_Click envía VALIDAR_CLIENTE. ManejadorClienteTCP.ValidarCliente "
            "invoca ClienteLN.ValidarClienteActivo que encuentra el registro pero lanza "
            "InvalidOperationException('El cliente no se encuentra activo.'). "
            "El catch en ProcesarSolicitud retorna Exito=false; Form1 Cliente muestra "
            "MessageBox Warning y mantiene funcionalidades deshabilitadas.",
            s["body"],
        ),
    ]
    story += [PageBreak()]

    story += [P("25. Índice de archivos fuente del proyecto", s["h1"])]
    story += [
        table(
            [
                ["Archivo", "Clase / Contenido", "Capa"],
                ["Persona.cs", "Persona (abstracta)", "Entidades"],
                ["Cliente.cs / Vendedor.cs", "Subclases de Persona", "Entidades"],
                ["Partido.cs / Localidad.cs", "Entidades de catálogo", "Entidades"],
                ["LocalidadPorPartido.cs", "Asociación partido-localidad", "Entidades"],
                ["Venta.cs", "Transacción de negocio", "Entidades"],
                ["IValidable.cs / ValidacionEntidad.cs", "Polimorfismo validación", "Entidades"],
                ["ComandosRed.cs", "Constantes protocolo", "Entidades"],
                ["MensajeSolicitud.cs / MensajeRespuesta.cs", "Serialización JSON", "Entidades"],
                ["JsonConfig.cs", "Opciones UTF-8 JSON", "Entidades"],
                ["ConsultaVenta.cs / ConsultaLocalidadPorPartido.cs", "DTOs consulta", "Entidades"],
                ["ConsultaComprasParser.cs", "Parseo respuesta cliente", "Entidades"],
                ["EncabezadoProyecto.cs", "Metadatos UNED", "Entidades"],
                ["ConexionBD.cs", "Cadena conexión SQL", "AccesoDatos"],
                ["ClienteAD.cs … VentaAD.cs", "6 clases AD", "AccesoDatos"],
                ["ValidadorComun.cs", "Validaciones compartidas", "LogicaNegocio"],
                ["ClienteLN.cs … VentaLN.cs", "6 clases LN", "LogicaNegocio"],
                ["ServidorTCP.cs", "Listener multihilo", "Servidor"],
                ["ManejadorClienteTCP.cs", "Router comandos JSON", "Servidor"],
                ["Form1.cs / Form1.Designer.cs", "UI administración", "Servidor"],
                ["AppleTheme.cs", "Tema visual servidor", "Servidor"],
                ["App.config", "Connection string SQL", "Servidor"],
                ["ClienteRed.cs / ServidorConfig.cs", "Capa red cliente", "Cliente"],
                ["Form1.cs / Form1.Designer.cs", "UI compras en línea", "Cliente"],
                ["AppleTheme.cs", "Tema visual cliente", "Cliente"],
                ["Program.cs", "Entry point WinForms", "Servidor / Cliente"],
            ],
            [4.5 * cm, 5.5 * cm, 3.5 * cm],
        )
    ]
    story += [PageBreak()]

    # --- CONCLUSIONES ---
    story += [P("26. Conclusiones y aspectos destacados", s["h1"])]
    story += [
        P(
            "El sistema BOLETERIAUNED cumple los requisitos del Proyecto #2 de Programación "
            "Avanzada con C# demostrando competencias en arquitectura multicapa, POO "
            "(herencia, polimorfismo, composición), acceso a datos ADO.NET, programación "
            "de redes con sockets TCP, serialización JSON, interfaces gráficas WinForms, "
            "programación multihilo y control de concurrencia en operaciones críticas.",
            s["body"],
        )
    ]
    story += [P("26.1 Fortalezas de la implementación", s["h2"])]
    story += bullet(
        [
            "Separación clara de responsabilidades entre cinco proyectos compilables.",
            "Validación polimórfica centralizada con IValidable y ValidacionEntidad.",
            "Protocolo de red extensible mediante constantes ComandosRed.",
            "Transacciones SQL + semáforos para integridad de inventario bajo concurrencia.",
            "Sesión TCP por conexión que protege operaciones del cliente autenticado.",
            "Soporte UTF-8 en red y JSON para nombres con tildes (Pérez, González, etc.).",
            "Bitácora en tiempo real y auto-refresco de consultas tras ventas en línea.",
        ],
        s["bullet"],
    )
    story += [P("26.2 Mapa de archivos clave", s["h2"])]
    story += [
        table(
            [
                ["Archivo", "Rol"],
                ["Persona.cs / Cliente.cs / Vendedor.cs", "Herencia y validación de personas"],
                ["IValidable.cs / ValidacionEntidad.cs", "Polimorfismo de validación"],
                ["VentaLN.cs", "Reglas de venta y concurrencia"],
                ["ServidorTCP.cs / ManejadorClienteTCP.cs", "Capa de red del servidor"],
                ["ClienteRed.cs / Form1.cs (Cliente)", "Aplicación en línea"],
                ["Form1.cs (Servidor)", "Administración y boletería física"],
                ["ConexionBD.cs / *AD.cs", "Persistencia SQL Server"],
                ["ComandosRed.cs / MensajeSolicitud.cs", "Protocolo JSON/TCP"],
            ],
            [7 * cm, 9.5 * cm],
        )
    ]
    story += [Spacer(1, 0.8 * cm)]
    story += [HRFlowable(width="60%", thickness=1, color=colors.HexColor("#0f3460"))]
    story += [Spacer(1, 0.5 * cm)]
    story += [
        P(
            "<i>Documento generado como manual técnico de implementación del Proyecto #2 — "
            "Boletaría UNED. Universidad Estatal a Distancia, Programación Avanzada con C# (00830), "
            "II Cuatrimestre 2026. Estudiante: Jossue Sanabria.</i>",
            s["footer"],
        )
    ]

    return story


def main():
    styles = build_styles()
    doc = SimpleDocTemplate(
        OUTPUT,
        pagesize=A4,
        leftMargin=2 * cm,
        rightMargin=2 * cm,
        topMargin=2 * cm,
        bottomMargin=2.2 * cm,
        title="Manual de Implementación BOLETERIAUNED",
        author="Jossue Sanabria",
    )
    story = build_story(styles)
    doc.build(story, onFirstPage=footer, onLaterPages=footer)
    print(f"PDF generado: {OUTPUT}")
    print(f"Páginas: {doc.page}")


if __name__ == "__main__":
    main()
