using Wall_E;
public class InvalidFunctionArityError : RuntimeError
{
    public InvalidFunctionArityError(string function, int expected, int got, int line)
        : base($"Function '{function}' expects {expected} argument(s), but got {got}", line) { }
}
