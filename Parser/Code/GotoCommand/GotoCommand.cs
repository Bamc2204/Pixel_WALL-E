using System;

namespace Wall_E
{
    #region GotoCommand

    /// <summary>
    /// Representa un comando de salto condicional (Goto) en el código.
    /// Permite saltar a una etiqueta específica si se cumple una condición.
    /// </summary>
    public class GotoCommand : Code
    {
        #region Properties

        /// <summary>
        /// Nombre de la etiqueta de destino a la que se realizará el salto.
        /// </summary>
        public string TargetLabel { get; set; } = "";

        /// <summary>
        /// Texto de la condición que debe cumplirse para ejecutar el salto.
        /// </summary>
        public string ConditionText { get; set; } = "";

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una representación en texto del comando Goto, mostrando la etiqueta de destino, la condición y la línea.
        /// </summary>
        public override string ToString() => $"Goto [{TargetLabel}] ({ConditionText}) [line {Line}]";

        #endregion
    }

    #endregion
}