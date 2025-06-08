using System;
using System.Drawing;

namespace Wall_E
{
    #region DrawRectangleCommandClass

    /// <summary>
    /// Comando gráfico que dibuja un rectángulo en el canvas desde la posición actual del cursor.
    /// Recibe como argumentos el ancho y el alto del rectángulo.
    /// </summary>
    public class DrawRectangleCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando DrawRectangle. Dibuja un rectángulo en el canvas según los argumentos.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 2 argumentos (ancho y alto).
            if (Arguments.Count != 2)
                throw new InvalidArgumentError("DrawRectangle", "Se esperaban 2 argumentos (ancho y alto).", Line);

            // Evalúa los argumentos: ancho y alto.
            int width = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int height = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));
            // Obtiene el color y tamaño actual del pincel.
            Color color = executor.BrushColor;
            int size = executor.BrushSize;

            // Dibuja el rectángulo en el canvas.
            executor.Canvas.DrawRectangle(width, height, color, size);
        }

        #endregion
    }

    #endregion
}