namespace Wall_E
{
    #region "Comando Size"

    /// <summary>
    /// Clase que representa el comando Size, utilizada para almacenar el valor del tamaño a establecer.
    /// Hereda de la clase base Code.
    /// </summary>
    public class SizeCommand : Code
    {
        #region "Propiedades"

        /// <summary>
        /// Valor del tamaño que se va a establecer.
        /// </summary>
        public int Valor { get; set; }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString() => $"Size({Valor}) [línea {Linea}]";

        #endregion
    }

    #endregion
}