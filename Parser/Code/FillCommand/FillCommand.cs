using System.Drawing;

namespace Wall_E
{
    public class FillCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            executor.Canvas.Fill(executor.BrushColor);
        }
    }
}