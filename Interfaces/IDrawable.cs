namespace Wall_E
{
    #region InterfaceDefinition

    /// <summary>
    /// Interfaz para comandos que pueden dibujarse en el canvas.
    /// Define el contrato para cualquier objeto que pueda ser dibujado en el PixelCanvas.
    /// </summary>
    public interface IDrawable
    {
        #region Methods

        /// <summary>
        /// Dibuja el objeto en el canvas usando el executor.
        /// </summary>
        /// <param name="canvas">Referencia al canvas donde se dibuja.</param>
        /// <param name="executor">Executor que gestiona la ejecución del comando.</param>
        void Draw(PixelCanvas canvas, Executor executor);

        #endregion
    }

    #endregion
}