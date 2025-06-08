using System;

namespace Wall_E
{
    /// <summary>
    /// Excepción lanzada cuando una etiqueta no puede encontrarse en el código.
    /// </summary>
    public class LabelNotFoundError : RuntimeError
    {
        /// <summary>
        /// Nombre de la etiqueta que no fue encontrada.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Constructor para la excepción LabelNotFoundError.
        /// </summary>
        /// <param name="label">Nombre de la etiqueta.</param>
        /// <param name="message">Mensaje descriptivo del error.</param>
        /// <param name="line">Número de línea donde ocurrió el error.</param>
        public LabelNotFoundError(string label, string message, int line)
            : base(message, line)
        {
            Label = label;
        }
    }
}
