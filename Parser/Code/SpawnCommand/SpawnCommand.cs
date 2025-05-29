namespace Wall_E
{
    #region SpawnCommand

    /// <summary>
    /// Clase que representa el comando Spawn, utilizada para almacenar las coordenadas donde se debe crear un objeto.
    /// Hereda de la clase base GraphicCommand.
    /// </summary>
    public class SpawnCommand : GraphicCommand
    {
        #region Properties

        /// <summary>
        /// Coordenada X donde se va a hacer el "spawn".
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Coordenada Y donde se va a hacer el "spawn".
        /// </summary>
        public int Y { get; set; }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString()
        {
            return $"Spawn({X}, {Y}) [línea {Line}]";
        }

        #endregion
    }

    #endregion
}