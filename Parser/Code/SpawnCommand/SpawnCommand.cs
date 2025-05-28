namespace Wall_E
{
    #region "Comando Spawn"

    /// <summary>
    /// Clase que representa el comando Spawn, utilizada para almacenar las coordenadas donde se debe crear un objeto.
    /// Hereda de la clase base Code.
    /// </summary>
    public class SpawnCommand : Code
    {
        #region "Propiedades"

        /// <summary>
        /// Coordenada X donde se va a hacer el "spawn".
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Coordenada Y donde se va a hacer el "spawn".
        /// </summary>
        public int Y { get; set; }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una cadena con el formato del comando y la línea donde se encontró.
        /// </summary>
        public override string ToString()
        {
            return $"Spawn({X}, {Y}) [línea {Linea}]";
        }

        #endregion
    }

    #endregion
}