namespace Wall_E
{
    #region "Comando Color"

    /// <summary>
    /// Clase que representa el comando Color, utilizada para almacenar el nombre del color a utilizar.
    /// Hereda de la clase base Code.
    /// </summary>
    public class ColorCommand : Code
    {
        #region "Propiedades"

        /// <summary>
        /// Nombre del color que se va a utilizar.
        /// </summary>
        public string NombreColor { get; set; }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString() => $"Color(\"{NombreColor}\") [línea {Linea}]";

        #endregion
    }

    #endregion
}