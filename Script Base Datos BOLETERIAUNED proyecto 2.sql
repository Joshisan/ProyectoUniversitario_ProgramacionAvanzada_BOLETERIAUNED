-- ============================================================================
-- Autor: Jossue Sanabria
-- Proyecto: BOLETERIAUNED - Proyecto 2 (00830 Programación Avanzada con C#)
-- Descripción: Script de creación de base de datos, tablas, constraints y relaciones
--              para el sistema de boletaría UNED (ventas presenciales y en línea).
-- ============================================================================

-- Crea la base de datos en el servidor SQL si no existe.
USE [master]
GO

CREATE DATABASE [BOLETERIAUNED]
GO
 
-- Cambia al contexto de la base recién creada para definir objetos dbo.
USE [BOLETERIAUNED]
GO

-- Configuración estándar de compatibilidad ANSI para CREATE TABLE.
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================================
-- Tabla: Cliente
-- Almacena usuarios registrados que pueden autenticarse y comprar entradas en línea.
-- PK_Cliente: clave primaria por IdCliente.
-- UQ_Cliente: garantiza identificación (cédula) única por cliente.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[IdCliente] [int] NOT NULL,              -- Identificador numérico interno del cliente
	[Identificacion] [varchar](10) NOT NULL, -- Cédula o documento de identidad (único)
	[Nombre] [varchar](25) NOT NULL,         -- Primer nombre del cliente
	[Apellido] [varchar](25) NOT NULL,       -- Apellido del cliente
	[FechaNacimiento] [datetime] NOT NULL,   -- Fecha de nacimiento para validaciones de edad
	[FechaRegistro] [datetime] NOT NULL,     -- Fecha en que se registró en el sistema
	[Activo] [bit] NOT NULL,                 -- 1=activo (puede comprar); 0=inactivo (rechazado en auth)
 CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED 
(
	[IdCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Cliente] UNIQUE NONCLUSTERED 
(
	[Identificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Tabla: LocalidadPorPartido
-- Inventario de entradas: relaciona un partido con una localidad y cantidad disponible.
-- UQ_Partido_Localidad: evita duplicar la misma localidad en un mismo partido.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocalidadPorPartido](
	[IdLocalidadPartido] [int] NOT NULL,     -- Clave surrogate del registro inventario
	[IdPartido] [int] NOT NULL,              -- FK hacia Partido
	[IdLocalidad] [int] NOT NULL,            -- FK hacia Localidad (sector del estadio)
	[CantidadDisponible] [int] NOT NULL,     -- Stock de entradas restantes para esa combinación
PRIMARY KEY CLUSTERED 
(
	[IdLocalidadPartido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Partido_Localidad] UNIQUE NONCLUSTERED 
(
	[IdPartido] ASC,
	[IdLocalidad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Tabla: Localidad
-- Catálogo de sectores del estadio (Palco, Preferencial, Sol, Sombra) con precio base.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Localidad](
	[IdLocalidad] [int] NOT NULL,            -- Identificador del tipo de localidad
	[NombreLocalidad] [varchar](50) NOT NULL, -- Nombre descriptivo del sector
	[Precio] [decimal](10, 2) NOT NULL,      -- Precio unitario por entrada en colones
PRIMARY KEY CLUSTERED 
(
	[IdLocalidad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Tabla: Partido
-- Eventos deportivos programados; Activo=1 indica que acepta ventas de entradas.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Partido](
	[IdPartido] [int] NOT NULL,              -- Identificador del partido/evento
	[Rival] [varchar](100) NOT NULL,         -- Nombre del equipo visitante
	[Fecha] [datetime] NOT NULL,             -- Fecha del encuentro
	[Hora] [varchar](10) NOT NULL,           -- Hora de inicio (formato texto HH:mm)
	[Activo] [bit] NOT NULL,                 -- 1=visible para venta; 0=partido cancelado o cerrado
PRIMARY KEY CLUSTERED 
(
	[IdPartido] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Tabla: Vendedor
-- Personal de taquilla presencial; referenciado opcionalmente en Venta (NULL en compras en línea).
-- UQ_Vendedor: identificación única por vendedor.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendedor](
	[IdVendedor] [int] NOT NULL,             -- Identificador interno del vendedor
	[Identificacion] [varchar](10) NOT NULL, -- Documento de identidad del vendedor
	[Nombre] [varchar](25) NOT NULL,         -- Nombre del vendedor
	[Apellido] [varchar](25) NOT NULL,       -- Apellido del vendedor
	[FechaNacimiento] [datetime] NOT NULL,   -- Fecha de nacimiento
	[FechaIngreso] [datetime] NOT NULL,      -- Fecha de ingreso a la boletaría
 CONSTRAINT [PK_Vendedor] PRIMARY KEY CLUSTERED 
(
	[IdVendedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Vendedor] UNIQUE NONCLUSTERED 
(
	[Identificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Tabla: Venta
-- Registro de cada transacción (presencial o en línea). IdVenta es IDENTITY autoincremental.
-- TipoVenta distingue canal: 'EnLinea' vs venta presencial con vendedor.
-- ============================================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Venta](
	[IdVenta] [int] IDENTITY(1,1) NOT NULL,  -- Número consecutivo de venta (generado automáticamente)
	[IdCliente] [int] NOT NULL,              -- FK: cliente que realizó la compra
	[IdPartido] [int] NOT NULL,              -- FK: partido para el cual se compraron entradas
	[IdLocalidad] [int] NOT NULL,            -- FK: sector/localidad comprada
	[IdVendedor] [int] NULL,                 -- FK opcional: vendedor presencial; NULL si compra en línea
	[Cantidad] [int] NOT NULL,               -- Número de entradas vendidas en la transacción
	[FechaVenta] [datetime] NOT NULL,        -- Marca de tiempo de la venta
	[MontoTotal] [decimal](10, 2) NOT NULL,  -- Monto total = precio unitario × cantidad
	[TipoVenta] [varchar](20) NOT NULL,      -- Canal de venta ('EnLinea', 'Presencial', etc.)
PRIMARY KEY CLUSTERED 
(
	[IdVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- ============================================================================
-- Claves foráneas: integridad referencial entre tablas del modelo relacional
-- ============================================================================

-- LocalidadPorPartido.IdLocalidad → Localidad.IdLocalidad
ALTER TABLE [dbo].[LocalidadPorPartido]  WITH CHECK ADD  CONSTRAINT [FK_LocalidadPartido_Localidad] FOREIGN KEY([IdLocalidad])
REFERENCES [dbo].[Localidad] ([IdLocalidad])
GO
ALTER TABLE [dbo].[LocalidadPorPartido] CHECK CONSTRAINT [FK_LocalidadPartido_Localidad]
GO

-- LocalidadPorPartido.IdPartido → Partido.IdPartido
ALTER TABLE [dbo].[LocalidadPorPartido]  WITH CHECK ADD  CONSTRAINT [FK_LocalidadPartido_Partido] FOREIGN KEY([IdPartido])
REFERENCES [dbo].[Partido] ([IdPartido])
GO
ALTER TABLE [dbo].[LocalidadPorPartido] CHECK CONSTRAINT [FK_LocalidadPartido_Partido]
GO

-- Venta.IdCliente → Cliente.IdCliente
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Cliente] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Cliente] ([IdCliente])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Cliente]
GO

-- Venta.IdLocalidad → Localidad.IdLocalidad
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Localidad] FOREIGN KEY([IdLocalidad])
REFERENCES [dbo].[Localidad] ([IdLocalidad])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Localidad]
GO

-- Venta.IdPartido → Partido.IdPartido
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Partido] FOREIGN KEY([IdPartido])
REFERENCES [dbo].[Partido] ([IdPartido])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Partido]
GO

-- Venta.IdVendedor → Vendedor.IdVendedor (opcional; NULL permitido para ventas en línea)
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Vendedor] FOREIGN KEY([IdVendedor])
REFERENCES [dbo].[Vendedor] ([IdVendedor])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Vendedor]
GO
