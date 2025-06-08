using System;

namespace Wall_E
{
    public class AssignmentCommand : ICode
    {
        public string VariableName { get; set; } = string.Empty;
        public Expr Expression { get; set; } = null!;
        public int Line { get; set; }

        public void Execute(Executor executor)
        {
            // Evalúa la expresión y la convierte a int (ajusta si tu lenguaje soporta otros tipos)
            int value = executor.EvaluateExpression(Expression);
            // Pasa el argumento 'Line' requerido por AssignVariable
            executor.AssignVariable(VariableName, value, Line);
        }
    }
}