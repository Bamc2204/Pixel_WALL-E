using System;

namespace Wall_E
{
    #region RuntimeError Class

    /// <summary>
    /// Excepción personalizada para errores en tiempo de ejecución.
    /// Permite asociar un mensaje de error y la línea donde ocurrió.
    /// </summary>
    public class RuntimeError : Exception
    {
        #region Properties
        /// <summary>
        /// Línea del código donde ocurrió el error.
        /// </summary>
        public int Line { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa la excepción con un mensaje y la línea del error.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del error.</param>
        /// <param name="line">Número de línea donde ocurrió el error.</param>
        public RuntimeError(string message, int line)
            : base($"[Line {line}] {message}")
        {
            Line = line;
        }
        #endregion
    }

    #endregion
}