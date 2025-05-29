using System;

namespace Wall_E
{
    public class InvalidArgumentError : RuntimeError
    {
        public InvalidArgumentError(string detail, int line)
            : base($"Invalid argument: {detail}", line)
        {
        }
    }
}
