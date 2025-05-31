using System;
using System.Drawing;
using System.Collections.Generic;

namespace Wall_E
{
    #region DrawRectangleCommand

    /// <summary>
    /// Comando que dibuja un rectángulo en el canvas visual.
    /// Sintaxis: DrawRectangle(width, height)
    /// </summary>
    public class DrawRectangleCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            // Verifica la cantidad de argumentos
            if (Arguments.Count != 2)
                throw new InvalidFunctionArityError("DrawRectangle", 2, Arguments.Count, Line);

            // Evalúa los argumentos
            int width = executor.EvaluateExpression(Arguments[0]);
            int height = executor.EvaluateExpression(Arguments[1]);

            // Llama al método de dibujo del canvas
            executor.Canvas.DrawRectangle(width, height, executor.BrushColor, executor.BrushSize);
        }
    }

    #endregion
}
