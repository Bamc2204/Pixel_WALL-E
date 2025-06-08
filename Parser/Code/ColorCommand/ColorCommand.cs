using System.Drawing;

namespace Wall_E
{
    #region ColorCommandClass

    /// <summary>
    /// Comando gráfico que cambia el color actual del pincel.
    /// Recibe como argumento el nombre del color a utilizar.
    /// </summary>
    public class ColorCommand : GraphicCommand
    {
        #region Execution

        /// <summary>
        /// Ejecuta el comando Color. Cambia el color del pincel según el argumento recibido.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado y la ejecución.</param>
        public override void Execute(Executor executor)
        {
            // Verifica que haya exactamente 1 argumento.
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("Color", "Se esperaba 1 argumento.", Line);

            // Evalúa el argumento y lo convierte a nombre de color.
            object value = executor.EvaluateExpression(Arguments[0]);
            string colorName = value.ToString() ?? "Transparent";

            // Cambia el color del pincel en el executor.
            executor.SetBrushColor(Color.FromName(colorName));
        }

        #endregion
    }

    #endregion
}