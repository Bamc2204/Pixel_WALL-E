namespace Wall_E
{
    #region BaseCommandClass

    /// <summary>
    /// Interfaz base para todos los comandos ejecutables en el sistema.
    /// Define la estructura mínima que debe tener cualquier comando.
    /// </summary>
    public interface ICode
    {
        #region Properties

        /// <summary>
        /// Línea del código fuente donde se encuentra el comando.
        /// </summary>
        int Line { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Método que ejecuta la lógica del comando usando el executor.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y ejecución.</param>
        void Execute(Executor executor);

        #endregion
    }

    #endregion
}