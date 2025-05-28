namespace Wall_E
{
    #region DrawLineCommand

    /// <summary>
    /// Clase que representa el comando DrawLine, utilizada para almacenar los parámetros del comando y su línea.
    /// Permite dibujar una línea en el canvas en la dirección y distancia especificadas.
    /// </summary>
    public class DrawLineCommand : Code
    {
        #region Properties

        /// <summary>
        /// Dirección X de la línea a dibujar.
        /// </summary>
        public int DirX { get; set; }

        /// <summary>
        /// Dirección Y de la línea a dibujar.
        /// </summary>
        public int DirY { get; set; }

        /// <summary>
        /// Distancia (longitud) de la línea a dibujar.
        /// </summary>
        public int Distance { get; set; }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una representación en texto del comando DrawLine, mostrando sus parámetros y la línea.
        /// </summary>
        public override string ToString()
        {
            return $"DrawLine({DirX}, {DirY}, {Distance}) [línea {Line}]";
        }

        #endregion
    }

    #endregion
}