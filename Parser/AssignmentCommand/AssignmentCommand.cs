using System;

namespace Wall_E
{
    #region AssignmentCommandClass

    /// <summary>
    /// Representa un comando de asignación de valor a una variable.
    /// Evalúa una expresión y almacena el resultado en la variable indicada.
    /// </summary>
    public class AssignmentCommand : ICode
    {
        #region Properties

        /// <summary>
        /// Nombre de la variable a la que se asigna el valor.
        /// </summary>
        public string VariableName { get; set; } = string.Empty;

        /// <summary>
        /// Expresión cuyo valor será asignado a la variable.
        /// </summary>
        public Expr Expression { get; set; } = null!;

        /// <summary>
        /// Línea del código fuente donde ocurre la asignación.
        /// </summary>
        public int Line { get; set; }

        #endregion

        #region Execution

        /// <summary>
        /// Ejecuta la asignación: evalúa la expresión y asigna el resultado a la variable.
        /// </summary>
        /// <param name="executor">Executor que gestiona el estado del programa.</param>
        public void Execute(Executor executor)
        {
            // Pasa el argumento 'Line' requerido por AssignVariable
            executor.AssignVariable(VariableName, executor.EvaluateExpression(Expression), Line);
        }

        #endregion
    }

    #endregion
}