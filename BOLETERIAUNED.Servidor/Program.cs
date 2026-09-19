/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Punto de entrada de la aplicación servidor.
 */

namespace BOLETERIAUNED.Servidor;

/// <summary>
/// Clase estática que contiene el método <see cref="Main"/> de arranque del servidor WinForms.
/// Es el primer código que ejecuta el runtime al iniciar el ejecutable BOLETERIAUNED.Servidor.
/// </summary>
static class Program
{
    /// <summary>
    /// Punto de entrada principal del proceso servidor.
    /// Configura el entorno WinForms y lanza el formulario de administración <see cref="Form1"/>.
    /// </summary>
    /// <remarks>
    /// El atributo <c>[STAThread]</c> es obligatorio en aplicaciones Windows Forms porque los
    /// controles COM y el bucle de mensajes de la UI requieren un hilo con apartamento STA
    /// (Single-Threaded Apartment).
    /// </remarks>
    [STAThread]
    static void Main()
    {
        // Inicializa DPI, fuentes predeterminadas y configuración visual de WinForms (.NET 6+).
        ApplicationConfiguration.Initialize();

        // Inicia el bucle de mensajes de la aplicación sobre el formulario principal del servidor.
        // Form1 arranca internamente el servidor TCP y la bitácora en tiempo real.
        Application.Run(new Form1());
    }
}
