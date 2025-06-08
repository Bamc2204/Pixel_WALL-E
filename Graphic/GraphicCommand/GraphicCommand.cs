using System.Collections.Generic;

namespace Wall_E
{
    #region GraphicCommandClass

    /// <summary>
    /// Clase base abstracta para comandos gráficos que usan una lista de argumentos.
    /// Hereda de la interfaz ICode.
    /// Proporciona propiedades comunes y define el método de ejecución que deben implementar los comandos gráficos concretos.
    /// </summary>
    public abstract class GraphicCommand : ICode
    {
        #region Properties

        /// <summary>
        /// Línea del código donde se encuentra el comando gráfico.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Lista de argumentos de la instrucción gráfica (pueden ser expresiones).
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();

        #endregion

        #region AbstractMethod

        /// <summary>
        /// Método abstracto que ejecuta el comando gráfico usando el executor.
        /// Debe ser implementado por cada comando gráfico concreto.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y ejecución.</param>
        public abstract void Execute(Executor executor);

        #endregion
    }

    #endregion
}