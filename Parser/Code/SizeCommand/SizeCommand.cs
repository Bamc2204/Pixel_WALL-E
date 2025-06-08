using System;

namespace Wall_E
{
    #region SizeCommandClass

    /// <summary>
    /// Comando gráfico que cambia el tamaño actual del pincel.
    /// Recibe como argumento el nuevo tamaño a establecer.
    /// </summary>
    public class SizeCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando Size. Cambia el tamaño del pincel según el argumento recibido.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 1 argumento.
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("Size", "Se esperaba 1 argumento.", Line);

            // Evalúa el argumento y lo convierte a entero.
            int size = Convert.ToInt32(executor.EvaluateExpression(Arguments[0]));
            // Cambia el tamaño del pincel en el executor.
            executor.SetBrushSize(size);
        }

        #endregion
    }

    #endregion
}