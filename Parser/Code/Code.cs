namespace Wall_E
{
    #region BaseCommandClass

    /// <summary>
    /// Clase abstracta base para todos los comandos que se pueden reconocer en el parser.
    /// Todas las instrucciones del lenguaje deben heredar de esta clase.
    /// </summary>
    public abstract class Code
    {
        #region Properties

        /// <summary>
        /// Propiedad que almacena el número de línea del código fuente donde se encontró el comando.
        /// Útil para mensajes de error y depuración.
        /// </summary>
        public int Line { get; set; }

        #endregion
    }

    #endregion
}