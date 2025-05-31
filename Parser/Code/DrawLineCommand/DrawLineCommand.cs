using System.Security.AccessControl;

namespace Wall_E
{
    #region DrawLineCommandClass

    /// <summary>
    /// Comando gráfico que representa la instrucción para dibujar una línea en el canvas.
    /// Hereda de GraphicCommand y utiliza tres argumentos: dx, dy y distancia.
    /// </summary>
    public class DrawLineCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando de dibujar línea usando el executor.
        /// Evalúa los argumentos y llama al método DrawLine del canvas.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que la cantidad de argumentos sea exactamente 3
            if (Arguments.Count != 3)
                throw new InvalidArgumentError("DrawLine requires 3 arguments", Line);

            // Evalúa los argumentos: desplazamiento en X, desplazamiento en Y y distancia
            int dx = executor.EvaluateExpression(Arguments[0]);
            int dy = executor.EvaluateExpression(Arguments[1]);
            int dist = executor.EvaluateExpression(Arguments[2]);

            // Llama al método del canvas para dibujar la línea
            executor.Canvas.DrawLine(dx, dy, dist);
        }

        #endregion
    }

    #endregion
}