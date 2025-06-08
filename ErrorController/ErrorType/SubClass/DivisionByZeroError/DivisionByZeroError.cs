using System;

namespace Wall_E
{
    /// <summary>
    /// Excepción lanzada cuando ocurre una división por cero.
    /// </summary>
    public class DivisionByZeroError : RuntimeError
    {
        public string Symbol { get; }

        public DivisionByZeroError(string symbol, string message, int line)
            : base(message, line)
        {
            Symbol = symbol;
        }
    }
}
