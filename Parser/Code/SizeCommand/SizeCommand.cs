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
        #region Methods

        /// <summary>
        /// Devuelve una representación en texto del comando Size, incluyendo sus argumentos y la línea donde se encuentra.
        /// </summary>
        public override string ToString() =>
            $"Size({string.Join(", ", Arguments)}) [line {Line}]";

        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("Size", 1, Arguments.Count, Line);

            int size = executor.EvaluateExpression(Arguments[0]);
            executor.Canvas.SetBrushSize(size);
        }
        
        #endregion
    }

    #endregion
}