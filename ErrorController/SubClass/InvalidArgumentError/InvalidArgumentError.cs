namespace Wall_E
{
    #region InvalidArgumentErrorClass

    /// <summary>
    /// Excepción personalizada para indicar que un argumento es inválido en una función o comando.
    /// Hereda de RuntimeError y permite especificar detalles y línea del error.
    /// </summary>
    public class InvalidArgumentError : RuntimeError
    {
        #region Constructors

        /// <summary>
        /// Constructor que recibe el detalle del argumento inválido y la línea donde ocurrió el error.
        /// </summary>
        /// <param name="detail">Detalle del argumento inválido.</param>
        /// <param name="line">Línea del código donde ocurrió el error.</param>
        public InvalidArgumentError(string detail, int line)
            : base($"Invalid argument: {detail}", line)
        {
        }

        /// <summary>
        /// Constructor que recibe el nombre de la función, un mensaje personalizado y la línea del error.
        /// </summary>
        /// <param name="functionName">Nombre de la función donde ocurrió el error.</param>
        /// <param name="message">Mensaje personalizado del error.</param>
        /// <param name="line">Línea del código donde ocurrió el error.</param>
        public InvalidArgumentError(string functionName, string message, int line)
            : base($"Función '{functionName}': {message}", line)
        {
        }

        #endregion
    }

    #endregion
}