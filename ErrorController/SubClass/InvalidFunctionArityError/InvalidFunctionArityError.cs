using Wall_E;

#region InvalidFunctionArityErrorClass

/// <summary>
/// Excepción personalizada que se lanza cuando una función recibe un número incorrecto de argumentos.
/// Hereda de RuntimeError y permite especificar la función, el esperado y el recibido.
/// </summary>
public class InvalidFunctionArityError : RuntimeError
{
    #region Constructor

    /// <summary>
    /// Inicializa la excepción con el nombre de la función, el número esperado y recibido de argumentos, y la línea del error.
    /// </summary>
    /// <param name="function">Nombre de la función.</param>
    /// <param name="expected">Cantidad de argumentos esperados.</param>
    /// <param name="got">Cantidad de argumentos recibidos.</param>
    /// <param name="line">Línea del código donde ocurrió el error.</param>
    public InvalidFunctionArityError(string function, int expected, int got, int line)
        : base($"Function '{function}' expects {expected} argument(s), but got {got}", line) { }

    #endregion
}

#endregion