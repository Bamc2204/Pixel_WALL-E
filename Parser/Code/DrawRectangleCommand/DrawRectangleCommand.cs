namespace Wall_E
{
    #region DrawRectangleCommandClass

    /// <summary>
    /// Comando que dibuja un rectángulo en el canvas visual.
    /// Sintaxis: DrawRectangle(width, height)
    /// </summary>
    public class DrawRectangleCommand : GraphicCommand
    {
        #region ExecuteMethod
        /// <summary>
        /// Ejecuta el comando para dibujar un rectángulo.
        /// </summary>
        /// <param name="executor">Contexto de ejecución que contiene el canvas y el estado actual.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que se reciban exactamente dos argumentos: ancho y alto
            if (Arguments.Count != 2)
                throw new InvalidFunctionArityError("DrawRectangle", 2, Arguments.Count, Line);

            // Evalúa el primer argumento como ancho del rectángulo
            int width = executor.EvaluateExpression(Arguments[0]);
            // Evalúa el segundo argumento como alto del rectángulo
            int height = executor.EvaluateExpression(Arguments[1]);

            // Llama al método de dibujo del canvas para crear el rectángulo con el color y tamaño actuales del pincel
            executor.Canvas.DrawRectangle(width, height, executor.BrushColor, executor.BrushSize);
        }
        #endregion
    }

    #endregion
}