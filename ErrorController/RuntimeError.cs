using System;

namespace Wall_E
{
    public class RuntimeError : Exception
    {
        public int Line { get; }

        public RuntimeError(string message, int line)
            : base($"[Line {line}] {message}")
        {
            Line = line;
        }
    }
}
