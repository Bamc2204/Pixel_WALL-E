using Wall_E;

public class LabelNotFoundError : RuntimeError
{
    public LabelNotFoundError(string label, int line)
        : base($"Label '{label}' not found", line) { }
}
