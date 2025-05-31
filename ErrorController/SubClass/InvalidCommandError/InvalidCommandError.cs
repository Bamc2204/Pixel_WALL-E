using Wall_E;

#region InvalidCommandErrorClass

/// <summary>
/// Excepción personalizada que se lanza cuando un comando es usado de forma incorrecta.
/// Hereda de RuntimeError y permite especificar el comando, el detalle y la línea del error.
/// </summary>
public class InvalidCommandError : RuntimeError
{
    #region Constructor

    /// <summary>
    /// Inicializa la excepción con el nombre del comando, el detalle del error y la línea donde ocurrió.
    /// </summary>
    /// <param name="command">Nombre del comando inválido.</param>
    /// <param name="detail">Detalle del error.</param>
    /// <param name="line">Línea del código donde ocurrió el error.</param>
    public InvalidCommandError(string command, string detail, int line)
        : base($"Invalid use of '{command}': {detail}", line) { }

    #endregion
}

#endregion