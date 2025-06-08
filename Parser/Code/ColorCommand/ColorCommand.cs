using System.Drawing;

namespace Wall_E
{
    public class ColorCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("Color", "Se esperaba 1 argumento.", Line);

            object value = executor.EvaluateExpression(Arguments[0]);
            string colorName = value.ToString() ?? "Transparent";
            executor.SetBrushColor(Color.FromName(colorName));
        }
    }
}