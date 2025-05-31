using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region DrawCircleCommand

    /// <summary>
    /// Comando gráfico que dibuja un círculo con un radio dado usando el pincel actual.
    /// Sintaxis: DrawCircle(radius)
    /// </summary>
    public class DrawCircleCommand : GraphicCommand
    {
        /// <summary>
        /// Ejecuta el comando gráfico de dibujo de círculo.
        /// </summary>
        /// <param name="executor">Entorno de ejecución con acceso a canvas y variables.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que se haya pasado exactamente un argumento
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("DrawCircle", 1, Arguments.Count, Line);

            // Evalúa el radio del círculo
            int radius = executor.EvaluateExpression(Arguments[0]);

            // Dibuja el círculo en el canvas visual
            executor.Canvas.DrawCircle(radius, executor.BrushColor, executor.BrushSize);
        }
    }

    #endregion
}
