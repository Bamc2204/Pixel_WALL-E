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
            // Verifica que haya exactamente 2 argumentos: X e Y
            if (Arguments.Count != 2)
                throw new InvalidFunctionArityError("Spawn", 2, Arguments.Count, Line);

            // Evalúa el primer argumento como coordenada X
            int x = executor.EvaluateExpression(Arguments[0]);
            // Evalúa el segundo argumento como coordenada Y
            int y = executor.EvaluateExpression(Arguments[1]);

            // Llama al método Spawn del canvas para posicionar el origen con el color y tamaño del pincel actuales
            executor.Canvas.Spawn(x, y, executor.BrushColor, executor.BrushSize);
        }
        #endregion
    }
    #endregion
}