/*
 * UNED - Programación Avanzada con C# (00830) - II Cuatrimestre 2026
 * Proyecto #2 Boletaría UNED | Estudiante: Jossue Sanabria | Fecha: 04/07/2026
 * Descripción: Validaciones comunes reutilizables del sistema.
 */

using System.Globalization;
using System.Text.RegularExpressions;

namespace BOLETERIAUNED.LogicaNegocio;

/// <summary>
/// Clase estática con validaciones de negocio reutilizables en toda la capa de lógica.
/// Centraliza reglas comunes para identificación, fechas, horas, precios y cantidades.
/// </summary>
public static class ValidadorComun
{
    /// <summary>
    /// Valida que la identificación no esté vacía y no exceda 10 caracteres.
    /// </summary>
    /// <param name="identificacion">Número de identificación del cliente o vendedor.</param>
    /// <exception cref="ArgumentException">
    /// Se lanza si la identificación está vacía o supera la longitud máxima permitida.
    /// </exception>
    public static void ValidarIdentificacion(string identificacion)
    {
        ValidarTextoNoVacio(identificacion, "Identificación");
        // Regla de negocio: máximo 10 caracteres según especificación del proyecto
        if (identificacion.Trim().Length > 10)
        {
            throw new ArgumentException("La Identificación no debe exceder 10 caracteres.");
        }
    }

    /// <summary>
    /// Valida que un campo de texto no sea nulo, vacío ni contenga solo espacios en blanco.
    /// </summary>
    /// <param name="valor">Cadena a validar.</param>
    /// <param name="nombreCampo">Nombre descriptivo del campo para el mensaje de error.</param>
    /// <exception cref="ArgumentException">Se lanza cuando el valor es nulo o está en blanco.</exception>
    public static void ValidarTextoNoVacio(string valor, string nombreCampo)
    {
        // IsNullOrWhiteSpace cubre null, "" y cadenas con solo espacios/tabs
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException($"El campo {nombreCampo} no debe quedar vacío.");
        }
    }

    /// <summary>
    /// Valida que una fecha no sea posterior al día actual del sistema.
    /// </summary>
    /// <param name="fecha">Fecha a validar (se compara solo la parte de fecha, sin hora).</param>
    /// <param name="nombreCampo">Nombre del campo para personalizar el mensaje de error.</param>
    /// <exception cref="ArgumentException">Se lanza si la fecha es futura respecto a hoy.</exception>
    public static void ValidarFechaNoFutura(DateTime fecha, string nombreCampo)
    {
        // .Date elimina la hora para comparar únicamente día/mes/año
        if (fecha.Date > DateTime.Today)
        {
            throw new ArgumentException($"El campo {nombreCampo} no debe ser mayor al día actual.");
        }
    }

    /// <summary>
    /// Valida que la fecha de nacimiento sea estrictamente anterior al día actual.
    /// </summary>
    /// <param name="fechaNacimiento">Fecha de nacimiento del cliente o vendedor.</param>
    /// <exception cref="ArgumentException">
    /// Se lanza si la fecha de nacimiento es hoy o una fecha futura.
    /// </exception>
    public static void ValidarFechaNacimiento(DateTime fechaNacimiento)
    {
        // Regla de negocio: una persona debe haber nacido antes del día de hoy
        if (fechaNacimiento.Date >= DateTime.Today)
        {
            throw new ArgumentException("La FechaNacimiento no debe ser mayor o igual al día actual.");
        }
    }

    /// <summary>
    /// Valida que la hora esté en formato HH:mm de 24 horas (00:00 a 23:59).
    /// </summary>
    /// <param name="hora">Cadena con la hora del partido (ejemplo: "19:30").</param>
    /// <exception cref="ArgumentException">
    /// Se lanza si la hora está vacía o no cumple el patrón HH:mm.
    /// </exception>
    public static void ValidarHora(string hora)
    {
        ValidarTextoNoVacio(hora, "Hora");
        // Expresión regular: horas 00-23, minutos 00-59, formato exacto HH:mm
        if (!Regex.IsMatch(hora.Trim(), @"^([01]\d|2[0-3]):[0-5]\d$"))
        {
            throw new ArgumentException("La Hora debe registrarse en formato válido HH:mm.");
        }
    }

    /// <summary>
    /// Valida que el precio de una localidad sea un valor positivo mayor a cero.
    /// </summary>
    /// <param name="precio">Precio unitario de la entrada en la localidad.</param>
    /// <exception cref="ArgumentException">Se lanza si el precio es cero o negativo.</exception>
    public static void ValidarPrecio(decimal precio)
    {
        // Regla de negocio: no se permiten precios gratuitos ni negativos
        if (precio <= 0)
        {
            throw new ArgumentException("El Precio debe ser mayor a cero.");
        }
    }

    /// <summary>
    /// Valida que la cantidad de entradas a vender sea un entero positivo mayor a cero.
    /// </summary>
    /// <param name="cantidad">Número de entradas solicitadas en la venta.</param>
    /// <exception cref="ArgumentException">Se lanza si la cantidad es cero o negativa.</exception>
    public static void ValidarCantidad(int cantidad)
    {
        // Regla de negocio: mínimo 1 entrada por transacción de venta
        if (cantidad <= 0)
        {
            throw new ArgumentException("La Cantidad debe ser mayor a cero.");
        }
    }
}
