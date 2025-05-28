using System;

namespace Wall_E
{
    #region LabelCommand

    /// <summary>
    /// Representa una etiqueta en el código, como 'loop-1'.
    /// Las etiquetas permiten marcar posiciones para saltos (Goto) u organización del flujo.
    /// </summary>
    public class LabelCommand : Code
    {
        #region Properties

        /// <summary>
        /// Nombre de la etiqueta definida.
        /// Por ejemplo: 'inicio', 'loop-1', etc.
        /// Esta propiedad almacena el identificador de la etiqueta que se usará para referencias en saltos o control de flujo.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una representación en texto de la etiqueta.
        /// Incluye el nombre de la etiqueta y la línea donde está definida.
        /// </summary>
        /// <returns>Cadena descriptiva de la etiqueta.</returns>
        public override string ToString()
        {
            return $"Etiqueta: {Name} [línea {Line}]";
        }

        #endregion
    }

    #endregion
}