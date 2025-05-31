using Wall_E;

#region LabelNotFoundErrorClass

/// <summary>
/// Excepción personalizada que se lanza cuando no se encuentra una etiqueta (label) en el código.
/// Hereda de RuntimeError y permite especificar el nombre de la etiqueta y la línea donde ocurrió el error.
/// </summary>
public class LabelNotFoundError : RuntimeError
{
    #region Constructor

    /// <summary>
    /// Inicializa la excepción con el nombre de la etiqueta y la línea donde ocurrió el error.
    /// </summary>
    /// <param name="label">Nombre de la etiqueta no encontrada.</param>
    /// <param name="line">Línea del código donde ocurrió el error.</param>
    public LabelNotFoundError(string label, int line)
        : base($"Label '{label}' not found", line) { }

    #endregion
}

#endregion