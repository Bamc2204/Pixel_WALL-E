using System;

namespace Wall_E
{
    public class SizeCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("Size", "Se esperaba 1 argumento.", Line);

            int size = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            executor.SetBrushSize(size);
        }
    }
}
