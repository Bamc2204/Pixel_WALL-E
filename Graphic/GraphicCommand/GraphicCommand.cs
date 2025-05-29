using System.Collections.Generic;

namespace Wall_E
{
    #region GraphicCommandClass

    /// <summary>
    /// Clase base abstracta para comandos gráficos que usan una lista de argumentos.
    /// Hereda de la clase Code.
    /// </summary>
    public abstract class GraphicCommand : Code
    {
        #region Properties

        /// <summary>
        /// Lista de argumentos para el comando gráfico.
        /// Cada argumento es una expresión que representa un valor necesario para el comando.
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();

        #endregion
    }

    #endregion
}