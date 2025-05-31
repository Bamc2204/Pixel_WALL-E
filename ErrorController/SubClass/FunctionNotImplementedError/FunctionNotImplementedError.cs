using System;

namespace Wall_E
{
    #region FunctionNotImplementedError Class

    /// <summary>
    /// Excepción personalizada para indicar que una función no está implementada.
    /// Hereda de RuntimeError y permite especificar el nombre de la función y la línea del error.
    /// </summary>
    public class FunctionNotImplementedError : RuntimeError
    {
        #region Constructor
        /// <summary>
        /// Inicializa la excepción con el nombre de la función y la línea donde ocurrió el error.
        /// </summary>
        /// <param name="function">Nombre de la función no implementada.</param>
        /// <param name="line">Número de línea donde ocurrió el error.</param>
        public FunctionNotImplementedError(string function, int line)
            : base($"Function not implemented: {function}", line)
        {
        }
        #endregion
    }

    #endregion
}