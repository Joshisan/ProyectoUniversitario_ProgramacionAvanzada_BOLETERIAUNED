/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Interfaz para validación polimórfica de entidades del sistema.
 */

namespace BOLETERIAUNED.Entidades;

/// <summary>
/// Contrato que define el comportamiento de validación polimórfica para las entidades del dominio.
/// Permite invocar <see cref="Validar"/> de forma uniforme sobre cualquier clase que lo implemente,
/// facilitando la reutilización mediante la clase utilitaria <see cref="ValidacionEntidad"/>.
/// </summary>
public interface IValidable
{
    /// <summary>
    /// Ejecuta las reglas de negocio de la entidad. Debe lanzar <see cref="ArgumentException"/>
    /// cuando algún dato no cumpla las restricciones definidas.
    /// </summary>
    void Validar();
}
