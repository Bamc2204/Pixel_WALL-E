using System;

namespace Wall_E
{
    /// <summary>
    /// Comando que representa Spawn(x, y), posiciona el origen en el canvas.
    /// </summary>
    #region SpawnCommandClass
    public class SpawnCommand : GraphicCommand, ICode
    {
        #region Properties
        // Número de línea donde se declaró el comando (propiedad en inglés)
        public new int Line { get; set; }
        
        #endregion

        #region ExecuteMethod
        /// <summary>
        /// Ejecuta el comando Spawn sobre el canvas visual.
        /// </summary>
        /// <param name="executor">Contexto de ejecución que contiene el canvas y estado.</param>
        public override void Execute(Executor executor)
        {
            int x = executor.EvaluateExpression(Arguments[0]);
            int y = executor.EvaluateExpression(Arguments[1]);

            if (!executor.Canvas.IsInBounds(x, y))
                throw new CanvasOutOfBoundsError(x, y, executor.Canvas.Cols, executor.Canvas.Rows, Line);

            executor.Canvas.Spawn(x, y);
        }
        #endregion
    }
    #endregion
}