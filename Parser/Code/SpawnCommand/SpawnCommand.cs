using System;

namespace Wall_E
{
    /// <summary>
    /// Comando que representa Spawn(x, y), posiciona el origen en el canvas.
    /// </summary>
    public class SpawnCommand : GraphicCommand, ICode
    {
        // Número de línea donde se declaró el comando
        public new int Line { get; set; }

        /// <summary>
        /// Ejecuta el comando Spawn sobre el canvas visual.
        /// </summary>
        /// <param name="executor">Contexto de ejecución que contiene el canvas y estado.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 2 argumentos: X e Y
            if (Arguments.Count != 2)
                throw new InvalidFunctionArityError("Spawn", 2, Arguments.Count, Line);

            int x = executor.EvaluateExpression(Arguments[0]);
            int y = executor.EvaluateExpression(Arguments[1]);

            executor.Canvas.Spawn(x, y, executor.BrushColor, executor.BrushSize);

        }
    }
}
