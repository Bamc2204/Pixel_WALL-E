using System;
using System.Drawing;

namespace Wall_E
{
    public class DrawRectangleCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 2)
                throw new InvalidArgumentError("DrawRectangle", "Se esperaban 2 argumentos (ancho y alto).", Line);

            int width = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int height = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));
            Color color = executor.BrushColor; // Usa directamente el Color, no FromName
            int size = executor.BrushSize;

            executor.Canvas.DrawRectangle(width, height, color, size);
        }
    }
}