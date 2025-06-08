using System;

namespace Wall_E
{
    public class DrawLineCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 3)
                throw new InvalidArgumentError("DrawLine", "Se esperaban 3 argumentos.", Line);

            int dx = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int dy = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));
            int length = Convert.ToInt32(executor.EvaluateExpression(Arguments[2]));

            executor.Canvas.DrawLine(dx, dy, length);
        }
    }
}
