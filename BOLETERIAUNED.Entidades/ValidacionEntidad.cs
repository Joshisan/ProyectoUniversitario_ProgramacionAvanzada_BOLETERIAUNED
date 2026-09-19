/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Utilidad polimórfica para validar entidades IValidable.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Clase estática utilitaria que centraliza la validación polimórfica de entidades
/// que implementan <see cref="IValidable"/>. Evita repetir llamadas directas a
/// <see cref="IValidable.Validar"/> en múltiples capas de la aplicación.
/// </summary>
public static class ValidacionEntidad
{
    /// <summary>
    /// Valida una única entidad delegando la lógica al método <see cref="IValidable.Validar"/> de la misma.
    /// </summary>
    /// <param name="entidad">Instancia de entidad que implementa <see cref="IValidable"/>.</param>
    public static void Validar(IValidable entidad)
    {
        // Delegación polimórfica: cada entidad ejecuta sus propias reglas de negocio
        entidad.Validar();
    }

    /// <summary>
    /// Valida secuencialmente un conjunto variable de entidades, deteniéndose en la primera excepción lanzada.
    /// </summary>
    /// <param name="entidades">Arreglo de entidades a validar, una tras otra.</param>
    public static void ValidarVarios(params IValidable[] entidades)
    {
        // Recorre cada entidad del arreglo y valida individualmente
        foreach (var entidad in entidades)
        {
            entidad.Validar();
        }
    }
}
