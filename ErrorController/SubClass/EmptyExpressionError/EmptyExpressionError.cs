using Wall_E;

public class EmptyExpressionError : RuntimeError
{
    public EmptyExpressionError(string context, int line)
        : base($"Missing expression in {context}", line) { }
}
