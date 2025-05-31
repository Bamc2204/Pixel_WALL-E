using System;

namespace Wall_E
{
    #region UndefinedVariableError Class

    /// <summary>
    /// Excepción personalizada para indicar que una variable no está definida.
    /// Hereda de RuntimeError y permite especificar el nombre de la variable y la línea del error.
    /// </summary>
    public class UndefinedVariableError : RuntimeError
    {
        #region Constructor
        /// <summary>
        /// Inicializa la excepción con el nombre de la variable y la línea donde ocurrió el error.
        /// </summary>
        /// <param name="variable">Nombre de la variable no definida.</param>
        /// <param name="line">Número de línea donde ocurrió el error.</param>
        public UndefinedVariableError(string variable, int line)
            : base($"Undefined variable '{variable}'", line)
        {
        }
        #endregion
    }

    #endregion
}