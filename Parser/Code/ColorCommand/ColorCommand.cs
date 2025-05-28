namespace Wall_E
{
    #region ColorCommand

    /// <summary>
    /// Clase que representa el comando Color, utilizada para almacenar el nombre del color a utilizar.
    /// Hereda de la clase base Code.
    /// </summary>
    public class ColorCommand : Code
    {
        #region Properties

        /// <summary>
        /// Nombre del color que se va a utilizar.
        /// </summary>
        public string ColorName { get; set; }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString() => $"Color(\"{ColorName}\") [línea {Line}]";

        #endregion
    }

    #endregion
}