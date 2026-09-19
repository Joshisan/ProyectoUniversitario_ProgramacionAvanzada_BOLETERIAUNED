/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Punto de entrada de la aplicación cliente Windows Forms.
 *              Inicia el runtime de la interfaz gráfica y muestra el formulario principal.
 */

namespace BOLETERIAUNED.Cliente;

/// <summary>
/// Clase estática que contiene el método <see cref="Main"/>, punto de arranque del ejecutable cliente.
/// WinForms requiere un hilo de interfaz de usuario con apartamento STA (Single Thread Apartment).
/// </summary>
static class Program
{
    /// <summary>
    /// Método principal del proceso cliente. Configura la aplicación WinForms y ejecuta el bucle de mensajes.
    /// </summary>
    /// <remarks>
    /// El atributo STAThread garantiza compatibilidad con controles COM y el modelo de hilos de Windows Forms.
    /// </remarks>
    [STAThread]
    static void Main()
    {
        // Inicializa configuraciones modernas de WinForms (DPI, fuentes, estilos visuales).
        ApplicationConfiguration.Initialize();

        // Arranca el bucle de mensajes de la UI; Form1 es la ventana principal del cliente en línea.
        Application.Run(new Form1());
    }
}
