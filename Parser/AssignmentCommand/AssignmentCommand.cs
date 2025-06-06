using System;
using Wall_E;

#region AssignmentCommand

/// <summary>
/// Representa un comando de asignación de variable en el código fuente.
/// Ejemplo: x <- 5 + y
/// </summary>
public class AssignmentCommand : ICode
{
    #region Properties

    /// <summary>
    /// Nombre de la variable a la que se asigna el valor.
    /// Por ejemplo, en 'x <- 5', el nombre sería 'x'.
    /// </summary>
    public string VariableName { get; set; }

    /// <summary>
    /// Expresión cuyo resultado se asigna a la variable.
    /// Puede ser un valor literal, una operación, una llamada a función, etc.
    /// </summary>
    public Expr Expression { get; set; }

    /// <summary>
    /// Línea del código donde se encuentra la asignación.
    /// </summary>
    public int Line { get; set; }

    #endregion

    #region FormattingAndRepresentationMethods

    /// <summary>
    /// Devuelve una representación en texto del comando de asignación.
    /// Incluye el nombre de la variable, la expresión formateada y la línea donde está definida.
    /// </summary>
    public override string ToString()
    {
        return $"{VariableName} <- {Format(Expression)} [línea {Line}]";
    }

    /// <summary>
    /// Formatea recursivamente la expresión para mostrarla como string.
    /// Dependiendo del tipo de expresión, la convierte a una representación legible.
    /// </summary>
    /// <param name="expr">Expresión a formatear.</param>
    /// <returns>Cadena representando la expresión.</returns>
    private string Format(Expr expr)
    {
        // Utiliza pattern matching para identificar el tipo de expresión y formatearla adecuadamente.
        return expr switch
        {
            LiteralExpr l => l.Value, // Valor literal, por ejemplo '5'
            VariableExpr v => v.Name, // Nombre de variable, por ejemplo 'y'
            BinaryExpr b => $"({Format(b.Left)} {b.Operator} {Format(b.Right)})", // Operación binaria, por ejemplo '(5 + y)'
            FunctionCallExpr f => $"{f.FunctionName}({string.Join(", ", f.Arguments.ConvertAll(Format))})", // Llamada a función
            _ => "?" // Caso desconocido
        };
    }

    #endregion

    #region Execution

    /// <summary>
    /// Ejecuta el comando de asignación usando el executor.
    /// Evalúa la expresión y asigna el resultado a la variable correspondiente.
    /// </summary>
    /// <param name="executor">Executor que gestiona el estado y ejecución.</param>
    public void Execute(Executor executor)
    {
        // Evalúa la expresión y asigna el resultado a la variable.
        int value = executor.EvaluateExpression(Expression);
        executor.AssignVariable(VariableName, value);
        // Muestra en consola la asignación realizada.
        Console.WriteLine($"[Line {Line}] {VariableName} = {value}");
    }

    #endregion
}

#endregion