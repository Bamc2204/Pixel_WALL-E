using System;

namespace Wall_E
{
    /// <summary>
    /// Representa una etiqueta en el código (como un marcador de posición).
    /// </summary>
    public class LabelCommand : ICode
    {
        // Nombre de la etiqueta
        public string Name { get; set; } = string.Empty;

        // Línea en la que aparece esta etiqueta
        public int Line { get; set; }

        /// <summary>
        /// Ejecuta la etiqueta. No hace nada, ya que las etiquetas son utilizadas como puntos de salto.
        /// </summary>
        /// <param name="executor">Contexto de ejecución.</param>
        public void Execute(Executor executor)
        {
            // Las etiquetas no hacen nada durante la ejecución,
            // simplemente marcan una posición en el código para saltos Goto.
        }
    }
}
