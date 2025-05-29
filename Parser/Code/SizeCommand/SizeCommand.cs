using System.Collections.Generic;

namespace Wall_E
{
    #region SizeCommand

    /// <summary>
    /// Representa el comando Size, encargado de almacenar los argumentos necesarios para definir un tamaño.
    /// Hereda de la clase base GraphicCommand.
    /// </summary>
    public class SizeCommand : GraphicCommand
    {
        #region Properties

        /// <summary>
        /// Lista de argumentos para el comando Size.
        /// Cada argumento es una expresión que representa un valor necesario para definir el tamaño.
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();

        #endregion

        #region Methods

        /// <summary>
        /// Devuelve una representación en texto del comando Size, incluyendo sus argumentos y la línea donde se encuentra.
        /// </summary>
        public override string ToString() =>
            $"Size({string.Join(", ", Arguments)}) [line {Line}]";

        #endregion
    }

    #endregion
}