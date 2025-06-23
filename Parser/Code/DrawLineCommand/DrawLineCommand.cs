using System;

namespace Wall_E
{
    #region DrawLineCommandClass

    /// <summary>
    /// Comando gráfico que dibuja una línea en el canvas desde la posición actual del cursor.
    /// Recibe como argumentos la dirección (dx, dy) y la longitud de la línea.
    /// </summary>
    public class DrawLineCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando DrawLine. Dibuja una línea en el canvas según los argumentos.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 3 argumentos.
            if (Arguments.Count != 3)
                throw new InvalidArgumentError("DrawLine", "Se esperaban 3 argumentos.", Line);

            // Evalúa los argumentos: dirección x, dirección y y longitud.
            int dx = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int dy = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));
            int length = Convert.ToInt32(executor.EvaluateExpression(Arguments[2]));

            // Dibuja la línea en el canvas usando el color y tamaño actuales del pincel.
            executor.Canvas.DrawLine(dx, dy, length, executor.BrushColor, executor.BrushSize, Line);
        }
        
        #endregion
    }

    #endregion
}