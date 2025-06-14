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
            if (Arguments.Count != 5)
                throw new InvalidFunctionArityError("DrawRectangler", 5, Arguments.Count, Line);


            int dirX = (int)executor.EvaluateExpression(Arguments[0]);
            int dirY = (int)executor.EvaluateExpression(Arguments[1]);
            int distance = (int)executor.EvaluateExpression(Arguments[2]);
            int width = (int)executor.EvaluateExpression(Arguments[3]);
            int height = (int)executor.EvaluateExpression(Arguments[4]);

            executor.Canvas.DrawRectangle(dirX, dirY, distance, width, height, executor.BrushColor, executor.BrushSize, Line);
        }

        #endregion
    }

    #endregion
}