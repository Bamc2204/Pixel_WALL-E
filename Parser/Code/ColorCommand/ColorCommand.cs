using System.Collections.Generic;

namespace Wall_E
{
    #region ColorCommand

    /// <summary>
    /// Representa el comando Color, encargado de almacenar los argumentos necesarios para definir un color.
    /// Hereda de la clase base GraphicCommand.
    /// </summary>
    public class ColorCommand : GraphicCommand
    {
        #region Properties

        /// <summary>
        /// Lista de argumentos para el comando Color.
        /// Cada argumento es una expresión que representa un valor necesario para definir el color.
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();

        #endregion

        #region Methods

        /// <summary>
        /// Devuelve una representación en texto del comando Color, incluyendo sus argumentos y la línea donde se encuentra.
        /// </summary>
        public override string ToString() =>
            $"Color({string.Join(", ", Arguments)}) [line {Line}]";

        #endregion
    }

    #endregion
}