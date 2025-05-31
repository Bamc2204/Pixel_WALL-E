using System;

namespace Wall_E
{
    #region UnknownOperatorErrorClass

    /// <summary>
    /// Excepción personalizada que se lanza cuando se encuentra un operador no soportado en una expresión.
    /// Por ejemplo: usar "$" como operador binario.
    /// Hereda de RuntimeError.
    /// </summary>
    public class UnknownOperatorError : RuntimeError
    {
        #region Constructors

        /// <summary>
        /// Constructor que recibe el operador desconocido y la línea donde ocurrió el error.
        /// </summary>
        /// <param name="op">Operador no soportado.</param>
        /// <param name="line">Línea del código donde ocurrió el error.</param>
        public UnknownOperatorError(string op, int line)
            : base($"[Line {line}] Unknown operator: {op}", line)
        {
        }

        #endregion
    }

    #endregion
}