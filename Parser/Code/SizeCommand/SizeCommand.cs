namespace Wall_E
{
    #region SizeCommand

    /// <summary>
    /// Clase que representa el comando Size, utilizada para almacenar el valor del tamaño a establecer.
    /// Hereda de la clase base Code.
    /// </summary>
    public class SizeCommand : Code
    {
        #region Properties

        /// <summary>
        /// Valor del tamaño que se va a establecer.
        /// </summary>
        public int Value { get; set; }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString() => $"Size({Value}) [línea {Line}]";

        #endregion
    }

    #endregion
}