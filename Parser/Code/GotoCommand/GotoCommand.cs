using System;

namespace Wall_E
{
    #region "Comando Goto"

    /// <summary>
    /// Representa un salto condicional a una etiqueta.
    /// Permite cambiar el flujo del programa según una condición.
    /// Ejemplo: Goto [etiqueta] (condición)
    /// </summary>
    public class GotoCommand : Code
    {
        #region "Propiedades"

        /// <summary>
        /// Nombre de la etiqueta de destino a la que se realiza el salto.
        /// </summary>
        public string EtiquetaDestino { get; set; }

        /// <summary>
        /// Condición en texto que debe cumplirse para realizar el salto.
        /// </summary>
        public string CondicionTexto { get; set; }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una representación en texto del comando Goto.
        /// </summary>
        public override string ToString()
        {
            return $"Goto [{EtiquetaDestino}] ({CondicionTexto}) [línea {Linea}]";
        }

        #endregion
    }

    #endregion
}