using System;

namespace Wall_E
{
    /// <summary>
    /// Representa una etiqueta en el código, como 'loop-1'
    /// </summary>
    public class LabelCommand : Comando
    {
        public string Nombre { get; set; }

        public override string ToString()
        {
            return $"Etiqueta: {Nombre} [línea {Linea}]";
        }
    }
}

