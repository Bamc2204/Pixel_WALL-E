using System;
using System.Drawing;

namespace Wall_E
{
    #region DrawCircleCommandClass

    /// <summary>
    /// Comando gráfico que dibuja un círculo en el canvas desde la posición actual del cursor.
    /// Recibe como argumento el radio del círculo.
    /// </summary>
    public class DrawCircleCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando DrawCircle. Dibuja un círculo en el canvas según el radio indicado.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 3)
                throw new InvalidFunctionArityError("DrawCircle", 3, Arguments.Count, Line);

            int dirX = (int)executor.EvaluateExpression(Arguments[0]);
            int dirY = (int)executor.EvaluateExpression(Arguments[1]);
            int radius = (int)executor.EvaluateExpression(Arguments[2]);

            executor.Canvas.DrawCircle(dirX, dirY, radius, executor.BrushColor, executor.BrushSize, Line);
        }

        #endregion
    }

    #endregion
}