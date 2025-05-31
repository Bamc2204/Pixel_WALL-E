namespace Wall_E
{
    #region SizeCommandClass

    /// <summary>
    /// Representa el comando Size, encargado de almacenar los argumentos necesarios para definir un tamaño.
    /// Hereda de la clase base GraphicCommand.
    /// </summary>
    public class SizeCommand : GraphicCommand
    {
        #region Methods

        /// <summary>
        /// Devuelve una representación en texto del comando Size, incluyendo sus argumentos y la línea donde se encuentra.
        /// </summary>
        public override string ToString() =>
            $"Size({string.Join(", ", Arguments)}) [line {Line}]";

        /// <summary>
        /// Ejecuta el comando Size, que define el tamaño del pincel en el canvas.
        /// </summary>
        /// <param name="executor">Contexto de ejecución que contiene el canvas y el estado actual.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que se reciba exactamente un argumento (el tamaño)
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("Size", 1, Arguments.Count, Line);

            // Evalúa el argumento como tamaño y lo asigna al pincel del canvas
            int size = executor.EvaluateExpression(Arguments[0]);
            executor.Canvas.SetBrushSize(size);
        }

        #endregion
    }

    #endregion
}