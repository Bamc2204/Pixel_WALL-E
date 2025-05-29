using Wall_E;

public class InvalidCommandError : RuntimeError
{
    public InvalidCommandError(string command, string detail, int line)
        : base($"Invalid use of '{command}': {detail}", line) { }
}
