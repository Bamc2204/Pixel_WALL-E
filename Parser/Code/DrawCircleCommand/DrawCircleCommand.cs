using System;
using System.Drawing;

namespace Wall_E
{
    public class DrawCircleCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("DrawCircle", "Se esperaba 1 argumento (radio).", Line);

            int radius = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            Color color = executor.BrushColor; // Usa directamente el Color, no FromName
            int size = executor.BrushSize;

            executor.Canvas.DrawCircle(radius, color, size);
        }
    }
}