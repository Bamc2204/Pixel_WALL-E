using System.Drawing;

namespace Wall_E
{
    /// <summary>
    /// Cambia el color del pincel a uno especificado por nombre.
    /// Sintaxis: Color("Red")
    /// </summary>
    public class ColorCommand : GraphicCommand
    {
        public override void Execute(Executor executor)
        {
            if (Arguments.Count != 1)
                throw new InvalidFunctionArityError("Color", 1, Arguments.Count, Line);

            Expr expr = Arguments[0];

            if (expr is not LiteralExpr literal || !literal.Value.StartsWith("\""))
                throw new InvalidArgumentError("El argumento de Color debe ser una cadena entre comillas", Line);

            string colorName = literal.Value.Trim('"');

            Color color = Color.FromName(colorName);

            if (!color.IsKnownColor)
                throw new InvalidArgumentError($"Color no válido: \"{colorName}\"", Line);

            executor.SetBrushColor(color);
        }
    }
}
