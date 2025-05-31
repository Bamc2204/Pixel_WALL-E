using System.Security.AccessControl;

namespace Wall_E
{
    public class DrawLineCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            // Evaluar los argumentos
            if (Arguments.Count != 3)
                throw new InvalidArgumentError("DrawLine requires 3 arguments", Line);

            int dx = executor.EvaluateExpression(Arguments[0]);
            int dy = executor.EvaluateExpression(Arguments[1]);
            int dist = executor.EvaluateExpression(Arguments[2]);


            executor.Canvas.DrawLine(dx, dy, dist);
        }
    }
}
