@echo off
REM Configura la base de datos BOLETERIAUNED para el Proyecto #2
REM Ejecutar UNA VEZ antes de usar la aplicacion (o cuando quiera resetear datos de prueba)

chcp 65001 >nul
echo ============================================
echo  Configuracion BD - BOLETERIA UNED
echo ============================================
echo.

set SQLSERVER=.\SQLEXPRESS
set SCRIPT_DIR=%~dp0

echo Usando instancia SQL: %SQLSERVER%
echo.

echo [1/2] Creando base de datos y tablas...
sqlcmd -S %SQLSERVER% -E -C -f 65001 -i "%SCRIPT_DIR%Script Base Datos BOLETERIAUNED proyecto 2.sql"
if errorlevel 1 (
    echo ERROR: No se pudo ejecutar el script de creacion.
    echo Verifique que SQL Server este instalado y en ejecucion.
    echo Si usa otra instancia, edite SQLSERVER en este archivo y App.config del Servidor.
    pause
    exit /b 1
)

echo.
echo [2/2] Cargando datos de prueba...
sqlcmd -S %SQLSERVER% -E -C -f 65001 -i "%SCRIPT_DIR%DatosPrueba.sql"
if errorlevel 1 (
    echo ERROR al cargar datos de prueba.
    pause
    exit /b 1
)

echo.
echo Verificando nombres con tildes...
sqlcmd -S %SQLSERVER% -E -C -W -Q "SET NOCOUNT ON; SELECT 'Cliente' AS Tipo, Nombre, Apellido FROM BOLETERIAUNED.dbo.Cliente WHERE IdCliente=1 UNION ALL SELECT 'Vendedor', Nombre, Apellido FROM BOLETERIAUNED.dbo.Vendedor WHERE IdVendedor=1"
echo.
echo ============================================
echo  Base de datos lista.
echo  Ahora abra Visual Studio y ejecute:
echo    1. BOLETERIAUNED.Servidor
echo    2. BOLETERIAUNED.Cliente
echo ============================================
pause
