namespace Wall_E
{
    #region GotoCommandClass

    /// <summary>
    /// Representa un comando de salto condicional a una etiqueta si la condición se cumple.
    /// Este comando permite alterar el flujo de ejecución del programa.
    /// </summary>
    public class GotoCommand : ICode
    {
        #region Properties

        /// <summary>
        /// Etiqueta de destino del salto.
        /// </summary>
        public string TargetLabel { get; set; } = string.Empty;

        /// <summary>
        /// Condición booleana que se debe cumplir para realizar el salto.
        /// </summary>
        public Expr Condition { get; set; } = null!;

        /// <summary>
        /// Línea del código fuente donde ocurre el comando.
        /// </summary>
        public int Line { get; set; }

        #endregion

        #region Execution

        /// <summary>
        /// Ejecuta el comando de salto si la condición se cumple.
        /// </summary>
        /// <param name="executor">Ejecutor actual que mantiene el contexto de ejecución.</param>
        /// <remarks>
        /// Este método evalúa la condición asociada al salto. El control real del salto
        /// (mover el flujo de ejecución) debe estar implementado en el Executor.
        /// Aquí solo se podría validar la existencia de la etiqueta o evaluar la condición.
        /// </remarks>
        public void Execute(Executor executor)
        {
            // Solo evalúa la condición. El salto real se maneja en Executor.Execute.
        }

        #endregion
    }

    #endregion
}