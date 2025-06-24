using Wall_E;

#region CanvasOutOfBoundsErrorClass

/// <summary>
/// Excepción personalizada lanzada cuando se intenta dibujar fuera del canvas.
/// </summary>
public class CanvasOutOfBoundsError : RuntimeError
{
    #region Fields

    /// <summary>
    /// Coordenada X donde ocurrió el error.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Coordenada Y donde ocurrió el error.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Ancho total del canvas.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Alto total del canvas.
    /// </summary>
    public int Height { get; }

    #endregion

    #region Constructor

    /// <summary>
    /// Inicializa la excepción con coordenadas, tamaño del canvas y línea del error.
    /// </summary>
    /// <param name="x">Coordenada X fuera de los límites.</param>
    /// <param name="y">Coordenada Y fuera de los límites.</param>
    /// <param name="width">Ancho del canvas.</param>
    /// <param name="height">Alto del canvas.</param>
    /// <param name="line">Línea donde ocurrió el error.</param>
    public CanvasOutOfBoundsError(int x, int y, int width, int height, int line)
        :base($"Coordenadas fuera del canvas: ({x}, {y}) no está en el rango [0, {width - 1}] x [0, {height - 1}]", line)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    #endregion
}

#endregion