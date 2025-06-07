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
        public int Line { get; }

        public RuntimeError(string message, int line) : base(message)
        {
            Line = line;
        }
    }

    #endregion
}