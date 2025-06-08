using System;

namespace Wall_E
{
    /// <summary>
    /// Excepción lanzada cuando una variable no ha sido definida.
    /// </summary>
    public class UndefinedVariableError : RuntimeError
    {
        public string VariableName { get; }

        public UndefinedVariableError(string variable, string message, int line)
            : base(message, line)
        {
            VariableName = variable;
        }
    }
}