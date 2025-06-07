using System;

namespace Wall_E
{
    #region DrawLineCommandClass

    /// <summary>
    /// Comando que dibuja una línea desde la posición actual del cursor.
    /// Sintaxis: DrawLine(dx, dy, distance)
    /// </summary>
    public class DrawLineCommand : GraphicCommand
    {
        #region ExecuteMethod

        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 3)
                throw new InvalidFunctionArityError("DrawLine", 3, Arguments.Count, Line);

            int dx = executor.EvaluateExpression(Arguments[0]);
            int dy = executor.EvaluateExpression(Arguments[1]);
            int dist = executor.EvaluateExpression(Arguments[2]);

            // Posición inicial del cursor
            int x = executor.Canvas.CursorX;
            int y = executor.Canvas.CursorY;

            // Validación: verificar que no se salga del canvas
            int finalX = x + dx * dist;
            int finalY = y + dy * dist;

            if (!executor.Canvas.IsInBounds(x, y) || !executor.Canvas.IsInBounds(finalX, finalY))
            {
                int cols = executor.Canvas.Cols;
                int rows = executor.Canvas.Rows;
                throw new CanvasOutOfBoundsError(finalX, finalY, cols, rows, Line);
            }

            // Ejecutar el dibujo
            executor.Canvas.DrawLine(dx, dy, dist);
        }

        #endregion
    }

    #endregion
}
