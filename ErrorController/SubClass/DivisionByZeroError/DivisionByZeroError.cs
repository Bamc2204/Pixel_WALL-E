using System;

namespace Wall_E
{
    #region DivisionByZeroError Class

    /// <summary>
    /// Excepción personalizada para indicar un error de división por cero.
    /// Hereda de RuntimeError y permite especificar la línea donde ocurrió el error.
    /// </summary>
    public class DivisionByZeroError : RuntimeError
    {
        #region Constructor
        /// <summary>
        /// Inicializa la excepción con la línea donde ocurrió la división por cero.
        /// </summary>
        /// <param name="line">Número de línea donde ocurrió el error.</param>
        public DivisionByZeroError(int line)
            : base("Division by zero", line)
        {
        }
        #endregion
    }

    #endregion
}