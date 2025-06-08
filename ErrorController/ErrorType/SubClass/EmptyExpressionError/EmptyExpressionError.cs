using Wall_E;

#region EmptyExpressionErrorClass

/// <summary>
/// Excepción personalizada que se lanza cuando falta una expresión en el contexto esperado.
/// Hereda de RuntimeError y permite especificar el contexto y la línea del error.
/// </summary>
public class EmptyExpressionError : RuntimeError
{
    #region Constructor

    /// <summary>
    /// Inicializa la excepción con el contexto donde falta la expresión y la línea donde ocurrió el error.
    /// </summary>
    /// <param name="context">Contexto donde se esperaba la expresión.</param>
    /// <param name="line">Línea del código donde ocurrió el error.</param>
    public EmptyExpressionError(string context, int line)
        : base($"Missing expression in {context}", line) { }

    #endregion
}

#endregion