namespace Wall_E
{
    /// <summary>
    /// Interfaz para comandos que pueden dibujarse en el canvas.
    /// </summary>
    public interface IDrawable
    {
        void Draw(PixelCanvas canvas, Executor executor);
    }
}
