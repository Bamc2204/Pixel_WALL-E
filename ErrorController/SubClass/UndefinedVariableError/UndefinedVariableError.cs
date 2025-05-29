using System;

namespace Wall_E
{
    public class UndefinedVariableError : RuntimeError
    {
        public UndefinedVariableError(string variable, int line)
            : base($"Undefined variable '{variable}'", line)
        {
        }
    }
}
