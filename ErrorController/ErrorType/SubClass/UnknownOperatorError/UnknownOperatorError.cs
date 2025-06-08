using System;

namespace Wall_E
{
    /// <summary>
    /// Excepción lanzada cuando se utiliza un operador desconocido.
    /// </summary>
    public class UnknownOperatorError : RuntimeError
    {
        public string Operator { get; }

        public UnknownOperatorError(string op, string message, int line)
            : base(message, line)
        {
            Operator = op;
        }
    }
}
