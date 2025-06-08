namespace Wall_E
{
    #region LabelCommandClass

    /// <summary>
    /// Representa una etiqueta en el código (como un marcador de posición para saltos).
    /// Las etiquetas no ejecutan ninguna acción, solo sirven como destino de instrucciones Goto.
    /// </summary>
    public class LabelCommand : ICode
    {
        #region Properties

        /// <summary>
        /// Nombre de la etiqueta.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Línea en la que aparece esta etiqueta.
        /// </summary>
        public int Line { get; set; }

        #endregion

        #region Execution

        /// <summary>
        /// Ejecuta la etiqueta. No hace nada, ya que las etiquetas son utilizadas como puntos de salto.
        /// </summary>
        /// <param name="executor">Contexto de ejecución.</param>
        public void Execute(Executor executor)
        {
            // Las etiquetas no hacen nada durante la ejecución,
            // simplemente marcan una posición en el código para saltos Goto.
        }

        #endregion
    }

    #endregion
}