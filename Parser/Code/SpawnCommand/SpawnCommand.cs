using System;

namespace Wall_E
{
    #region SpawnCommandClass

    /// <summary>
    /// Comando gráfico que posiciona el cursor de Wall-E en el canvas.
    /// </summary>
    public class SpawnCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando Spawn. Posiciona el cursor en las coordenadas (x, y) dadas.
        /// Lanza errores si los argumentos son incorrectos o si la posición está fuera del canvas.
        /// </summary>
        /// <param name="executor">Instancia del ejecutor que mantiene el estado del programa.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 2 argumentos.
            if (Arguments.Count != 2)
                throw new InvalidArgumentError("Spawn", "Se esperaban 2 argumentos.", Line);

            // Evalúa los argumentos como enteros.
            int x = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            int y = Convert.ToInt32(executor.EvaluateExpression(Arguments[1]));

            // Verifica que las coordenadas estén dentro de los límites del canvas.
            if (x < 0 || x >= executor.Canvas.Width || y < 0 || y >= executor.Canvas.Height)
                throw new CanvasOutOfBoundsError(x, y, executor.Canvas.Width, executor.Canvas.Height, Line);

            // Llama directamente al método del canvas para posicionar el cursor.
            executor.Canvas.SetCursorPosition(x, y);
        }

        #endregion
    }

    #endregion
}