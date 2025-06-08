using System;

namespace Wall_E
{
    public class SpawnCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 2)
                throw new InvalidArgumentError("Spawn", "Se esperaban 2 argumentos.", Line);

            int x = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int y = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));

            if (x < 0 || x >= executor.Canvas.Width || y < 0 || y >= executor.Canvas.Height)
                throw new CanvasOutOfBoundsError(x, y, executor.Canvas.Width, executor.Canvas.Height, Line);

            // Llama directamente al método del canvas para posicionar el cursor
            executor.Canvas.SetCursorPosition(x, y);
        }
    }
}