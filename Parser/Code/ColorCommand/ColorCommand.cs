using System;
using System.Drawing;

namespace Wall_E
{
    /// <summary>
    /// Cambia el color del pincel a uno especificado por nombre.
    /// </summary>
    public class ColorCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            // Validamos la cantidad de argumentos esperada
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("Color", 1, Arguments.Count, Line);

            Expr expr = Arguments[0];

            // Verificamos que el argumento sea una cadena literal
            if (expr is not LiteralExpr literal || !literal.Value.StartsWith("\""))
                throw new InvalidArgumentError("El argumento de Color debe ser una cadena entre comillas", Line);

            string colorName = literal.Value.Trim('"');

            try
            {
                // Intentamos convertir el nombre a un color real
                executor.Canvas.SetColor(Color.FromName(colorName));
            }
            catch
            {
                throw new InvalidArgumentError($"Color no válido: {colorName}", Line);
            }
        }
    }
}
