using System.Drawing;

namespace Wall_E
{
    /// <summary>
    /// Cambia el color del pincel a uno especificado por nombre.
    /// </summary>
    #region ColorCommandClass
    public class ColorCommand : GraphicCommand
    {
        #region ExecuteMethod
        /// <summary>
        /// Ejecuta el comando para cambiar el color del pincel.
        /// </summary>
        /// <param name="executor">El ejecutor que contiene el contexto de ejecución y el canvas.</param>
        public override void Execute(Executor executor)
        {
            // Validamos que solo se reciba un argumento
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("Color", 1, Arguments.Count, Line);

            // Obtenemos el primer argumento
            Expr expr = Arguments[0];

            // Verificamos que el argumento sea una cadena literal (debe empezar con comillas)
            if (expr is not LiteralExpr literal || !literal.Value.StartsWith("\""))
                throw new InvalidArgumentError("El argumento de Color debe ser una cadena entre comillas", Line);

            // Quitamos las comillas del nombre del color
            string colorName = literal.Value.Trim('"');

            try
            {
                // Intentamos convertir el nombre a un color real y lo aplicamos al canvas
                executor.Canvas.SetColor(Color.FromName(colorName));
            }
            catch
            {
                // Si el color no es válido, lanzamos un error
                throw new InvalidArgumentError($"Color no válido: {colorName}", Line);
            }
        }
        #endregion
    }
    #endregion
}