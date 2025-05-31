using Wall_E;

#region CanvasOutOfBoundsErrorClass

/// <summary>
/// Excepción personalizada que se lanza cuando se intenta dibujar fuera de los límites del canvas.
/// Hereda de RuntimeError y permite especificar la posición, el tamaño del canvas y la línea del error.
/// </summary>
public class CanvasOutOfBoundsError : RuntimeError
{
    #region Constructor

    /// <summary>
    /// Inicializa la excepción con la posición fuera de límites, el tamaño del canvas y la línea donde ocurrió el error.
    /// </summary>
    /// <param name="x">Coordenada X fuera de los límites.</param>
    /// <param name="y">Coordenada Y fuera de los límites.</param>
    /// <param name="width">Ancho del canvas.</param>
    /// <param name="height">Alto del canvas.</param>
    /// <param name="line">Línea del código donde ocurrió el error.</param>
    public CanvasOutOfBoundsError(int x, int y, int width, int height, int line)
        : base($"Drawing out of bounds: ({x},{y}) exceeds canvas size ({width},{height})", line) { }

    #endregion
}

#endregion