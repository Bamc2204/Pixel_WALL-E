using System.Collections.Generic;

namespace Wall_E
{
    #region DrawLineCommand

    /// <summary>
    /// Representa el comando DrawLine, encargado de almacenar los argumentos necesarios para dibujar una línea.
    /// Hereda de la clase base GraphicCommand.
    /// </summary>
    public class DrawLineCommand : GraphicCommand
    {
        #region Properties

        /// <summary>
        /// Lista de argumentos para el comando DrawLine.
        /// Cada argumento es una expresión que representa un valor necesario para definir la línea a dibujar.
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();

        #endregion

        #region Methods

        /// <summary>
        /// Devuelve una representación en texto del comando DrawLine, incluyendo sus argumentos y la línea donde se encuentra.
        /// </summary>
        public override string ToString() =>
            $"DrawLine({string.Join(", ", Arguments)}) [line {Line}]";

        #endregion
    }

    #endregion
}