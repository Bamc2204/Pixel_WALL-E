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
            if (Arguments.Count != 1)
                throw new InvalidArgumentError("Color", "Se esperaba 1 argumento.", Line);

            object value = executor.EvaluateExpression(Arguments[0]);
            string colorName = value.ToString()?.Trim() ?? "Transparent";

            // Normaliza el nombre: quita comillas si quedaron, y pone mayúscula inicial
            colorName = colorName.Trim('"').Trim();
            if (colorName.Length > 1)
                colorName = char.ToUpper(colorName[0]) + colorName.Substring(1).ToLower();

            // Para depuración: imprime el nombre real del color
            System.Diagnostics.Debug.WriteLine($"[ColorCommand] colorName: '{colorName}'");

            var color = Color.FromName(colorName);

            if (!color.IsKnownColor)
                throw new InvalidArgumentError($"Color desconocido: {colorName}", "El color no es válido", Line);

            executor.SetBrushColor(color);
        }

        #endregion
    }

    #endregion
}