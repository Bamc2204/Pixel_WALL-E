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
            // Verifica que haya exactamente 1 argumento (el radio).
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("DrawCircle", "Se esperaba 1 argumento (radio).", Line);

            // Evalúa el argumento como radio.
            int radius = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            // Obtiene el color y tamaño actual del pincel.
            Color color = executor.BrushColor;
            int size = executor.BrushSize;

            // Dibuja el círculo en el canvas.
            executor.Canvas.DrawCircle(radius, color, size);
        }

        #endregion
    }

    #endregion
}