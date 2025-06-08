namespace Wall_E
{
    #region FillCommandClass

    /// <summary>
    /// Comando gráfico que rellena todo el canvas con el color actual del pincel.
    /// </summary>
    public class FillCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando Fill. Rellena el canvas con el color del pincel.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Llama al método Fill del canvas usando el color actual del pincel.
            executor.Canvas.Fill(executor.BrushColor);
        }

        #endregion
    }

    #endregion
}