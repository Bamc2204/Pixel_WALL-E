using Wall_E;

public class CanvasOutOfBoundsError : RuntimeError
{
    public CanvasOutOfBoundsError(int x, int y, int width, int height, int line)
        : base($"Drawing out of bounds: ({x},{y}) exceeds canvas size ({width},{height})", line) { }
}
