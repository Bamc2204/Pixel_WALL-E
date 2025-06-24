namespace Wall_E
{
    #region InvalidLiteralErrorClass

    /// <summary>
    /// Excepción personalizada que se lanza cuando un literal no puede ser convertido al tipo esperado.
    /// Por ejemplo: intentar convertir "Blue" a número.
    /// Hereda de RuntimeError.
    /// </summary>
    public class InvalidLiteralError : RuntimeError
    {
        #region Constructors

        /// <summary>
        /// Constructor que recibe el mensaje del literal inválido y la línea donde ocurrió el error.
        /// </summary>
        /// <param name="message">Mensaje o valor del literal inválido.</param>
        /// <param name="line">Línea del código donde ocurrió el error.</param>
        public InvalidLiteralError(string message, int line)
            : base($"[Linea {line}] Literal no válido: {message}", line){ }

        #endregion
    }

    #endregion
}