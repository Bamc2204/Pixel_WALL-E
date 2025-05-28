using System;

namespace Wall_E
{
    #region "Comando de Etiqueta"

    /// <summary>
    /// Representa una etiqueta en el código, como 'loop-1'.
    /// Las etiquetas permiten marcar posiciones para saltos (Goto) u organización del flujo.
    /// </summary>
    public class LabelCommand : Code
    {
        #region "Propiedades"

        /// <summary>
        /// Nombre de la etiqueta definida.
        /// </summary>
        public string Nombre { get; set; }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una representación en texto de la etiqueta.
        /// </summary>
        public override string ToString()
        {
            return $"Etiqueta: {Nombre} [línea {Linea}]";
        }

        #endregion
    }

    #endregion
}