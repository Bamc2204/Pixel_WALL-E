namespace Wall_E
{
    public class CanvasOutOfBoundsError : RuntimeError
    {
        public CanvasOutOfBoundsError(int x, int y, int width, int height, int line)
            : base($"[{line}] Position ({x}, {y}) is out of bounds (Canvas: {width}x{height})", line) { }
    }
}
