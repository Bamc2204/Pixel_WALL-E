using System;

namespace Wall_E
{
    public class DivisionByZeroError : RuntimeError
    {
        public DivisionByZeroError(int line)
            : base("Division by zero", line)
        {
        }
    }
}
