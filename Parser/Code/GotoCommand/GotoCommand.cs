using System;

namespace Wall_E
{
    /// <summary>
    /// Representa un salto condicional a una etiqueta
    /// </summary>
    public class GotoCommand : Comando
    {
        public string EtiquetaDestino { get; set; }
        public string CondicionTexto { get; set; }

        public override string ToString()
        {
            return $"Goto [{EtiquetaDestino}] ({CondicionTexto}) [línea {Linea}]";
        }
    }
}

