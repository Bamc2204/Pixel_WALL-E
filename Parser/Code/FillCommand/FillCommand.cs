namespace Wall_E
{
    #region FillCommandClass

    /// <summary>
    /// Comando que rellena completamente el canvas con el color del pincel.
    /// Sintaxis: Fill()
    /// </summary>
    public class FillCommand : GraphicCommand
    {
        #region ExecuteMethod
        /// <summary>
        /// Ejecuta el comando de llenado.
        /// </summary>
        /// <param name="executor">Ejecutor que contiene el canvas y estado actual.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que no se pasen argumentos al comando Fill()
            if (Arguments.Count != 0)
                throw new InvalidFunctionArityError("Fill", 0, Arguments.Count, Line);

            // Llama al método del canvas que rellena toda la superficie con el color actual del pincel
            executor.Canvas.Fill(executor.BrushColor);
        }
        #endregion
    }

    #endregion
}