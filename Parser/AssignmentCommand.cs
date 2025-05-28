using Wall_E;

/// <summary>
/// Representa un comando de asignación de variable en el código fuente.
/// Ejemplo: x <- 5 + y
/// </summary>
public class AssignmentCommand : Code
{
    #region "Propiedades"

    /// <summary>
    /// Nombre de la variable a la que se asigna el valor.
    /// </summary>
    public string VariableName { get; set; }

    /// <summary>
    /// Expresión cuyo resultado se asigna a la variable.
    /// </summary>
    public Expr Expression { get; set; }

    #endregion

    #region "Métodos de Formato y Representación"

    /// <summary>
    /// Devuelve una representación en texto del comando de asignación.
    /// </summary>
    public override string ToString()
    {
        return $"{VariableName} <- {Format(Expression)} [línea {Linea}]";
    }

    /// <summary>
    /// Formatea recursivamente la expresión para mostrarla como string.
    /// </summary>
    private string Format(Expr expr)
    {
        return expr switch
        {
            LiteralExpr l => l.Value,
            VariableExpr v => v.Name,
            BinaryExpr b => $"({Format(b.Left)} {b.Operator} {Format(b.Right)})",
            FunctionCallExpr f => $"{f.FunctionName}({string.Join(", ", f.Arguments.ConvertAll(Format))})",
            _ => "?"
        };
    }

    #endregion
}

