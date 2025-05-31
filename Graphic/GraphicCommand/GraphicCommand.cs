using System.Collections.Generic;

namespace Wall_E
{
    #region GraphicCommandClass

    /// <summary>
    /// Clase base abstracta para comandos gráficos que usan una lista de argumentos.
    /// Hereda de la clase Code.
    /// </summary>
    public abstract class GraphicCommand : ICode
    {
        public int Line { get; set; }
        public List<Expr> Arguments { get; set; } = new();

        public abstract void Execute(Executor executor);
    }


    #endregion
}