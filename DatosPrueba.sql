-- ============================================================================
-- Autor: Jossue Sanabria
-- Proyecto: BOLETERIAUNED - Proyecto 2 (00830 Programación Avanzada con C#)
-- Descripción: Script de datos de prueba para poblar la base de datos.
--              Usa CHAR() para acentos y evitar corrupción por codificación del archivo o sqlcmd.
-- ============================================================================

-- Selecciona la base de datos del proyecto de boletaría UNED.
USE [BOLETERIAUNED]
GO

-- ---------------------------------------------------------------------------
-- Limpieza de tablas (orden inverso a dependencias por claves foráneas)
-- ---------------------------------------------------------------------------
DELETE FROM Venta;                  -- Ventas dependen de Cliente, Partido, Localidad y Vendedor
DELETE FROM LocalidadPorPartido;    -- Inventario por partido depende de Partido y Localidad
DELETE FROM Cliente;                -- Clientes registrados para compras en línea
DELETE FROM Vendedor;               -- Vendedores del módulo presencial (referenciados en Venta)
DELETE FROM Partido;                -- Partidos/eventos deportivos disponibles
DELETE FROM Localidad;              -- Tipos de localidad (Palco, Preferencial, etc.) y precios base
GO

-- ---------------------------------------------------------------------------
-- Tabla: Localidad — catálogo de sectores del estadio y precio unitario por entrada
-- ---------------------------------------------------------------------------
INSERT INTO Localidad (IdLocalidad, NombreLocalidad, Precio) VALUES
(1, 'Palco', 15000.00),           -- Sector premium; precio más alto
(2, 'Preferencial', 10000.00),    -- Sector intermedio-alto
(3, 'Sol', 5000.00),              -- Sector económico expuesto al sol
(4, 'Sombra', 7000.00);           -- Sector con sombra; precio intermedio
GO

-- ---------------------------------------------------------------------------
-- Tabla: Partido — eventos activos contra equipos rivales (fecha y hora del encuentro)
-- ---------------------------------------------------------------------------
INSERT INTO Partido (IdPartido, Rival, Fecha, Hora, Activo) VALUES
(1, 'UCR', '2026-08-15', '15:00', 1),              -- Partido activo vs Universidad de Costa Rica
(2, 'TEC', '2026-07-26', '17:00', 1),              -- Partido activo vs Tecnológico
(3, 'LD Alajuelense', '2026-09-10', '19:00', 1);   -- Partido activo vs Liga Deportiva Alajuelense
GO

-- ---------------------------------------------------------------------------
-- Tabla: Vendedor — personal de taquilla presencial (puede quedar NULL en ventas en línea)
-- ---------------------------------------------------------------------------
INSERT INTO Vendedor (IdVendedor, Identificacion, Nombre, Apellido, FechaNacimiento, FechaIngreso) VALUES
(1, '105560789', 'Juan', 'P' + CHAR(233) + 'rez', '1990-05-15', '2020-01-10'),   -- Apellido: Pérez
(2, '204450123', 'Mar' + CHAR(237) + 'a', 'Gonz' + CHAR(225) + 'lez', '1988-08-22', '2019-06-01'); -- María González
GO

-- ---------------------------------------------------------------------------
-- Tabla: Cliente — usuarios que compran en línea; Activo=1 permite autenticación y compra
-- ---------------------------------------------------------------------------
INSERT INTO Cliente (IdCliente, Identificacion, Nombre, Apellido, FechaNacimiento, FechaRegistro, Activo) VALUES
(1, '108870456', 'Johan', 'Ram' + CHAR(237) + 'rez', '1995-03-10', '2025-01-15', 1),  -- Cliente activo (Ramírez)
(2, '109990321', 'Ana', 'Mora', '1998-11-25', '2025-02-20', 1),                        -- Cliente activo
(3, '107770654', 'Carlos', 'Vega', '1992-07-08', '2024-12-01', 0);                     -- Cliente inactivo (prueba de rechazo)
GO

-- ---------------------------------------------------------------------------
-- Tabla: LocalidadPorPartido — inventario: cantidad disponible por localidad en cada partido
-- Constraint UQ_Partido_Localidad: un partido no puede tener la misma localidad duplicada
-- ---------------------------------------------------------------------------
INSERT INTO LocalidadPorPartido (IdLocalidadPartido, IdPartido, IdLocalidad, CantidadDisponible) VALUES
(1, 1, 1, 20),   -- Partido 1 vs UCR: 20 entradas Palco
(2, 1, 2, 50),   -- Partido 1 vs UCR: 50 entradas Preferencial
(3, 1, 3, 100),  -- Partido 1 vs UCR: 100 entradas Sol
(4, 2, 1, 15),   -- Partido 2 vs TEC: 15 entradas Palco
(5, 2, 4, 40),   -- Partido 2 vs TEC: 40 entradas Sombra
(6, 3, 2, 60);   -- Partido 3 vs LD Alajuelense: 60 entradas Preferencial
GO
