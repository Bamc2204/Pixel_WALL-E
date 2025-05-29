using System;

namespace Wall_E
{
    public class FunctionNotImplementedError : RuntimeError
    {
        public FunctionNotImplementedError(string function, int line)
            : base($"Function not implemented: {function}", line)
        {
        }
    }
}
