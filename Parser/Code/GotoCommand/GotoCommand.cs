using System;

namespace Wall_E
{
    /// <summary>
    /// Representa un comando de salto condicional hacia una etiqueta.
    /// Sintaxis: Goto[etiqueta](condición)
    /// </summary>
    public class GotoCommand : ICode
    {
        /// <summary>
        /// Nombre de la etiqueta de destino.
        /// </summary>
        public string TargetLabel { get; set; }

        /// <summary>
        /// Condición como string (por ejemplo: "x >= 3").
        /// </summary>
        public string ConditionText { get; set; }

        /// <summary>
        /// Línea en la que se encuentra el comando en el código fuente.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Evalúa la condición y determina si se debe hacer el salto.
        /// </summary>
        public bool ShouldJump(Executor executor)
        {
            try
            {
                return executor.EvaluateConditionText(ConditionText, Line) != 0;
            }
            catch (RuntimeError e)
            {
                throw new InvalidCommandError("Goto", e.Message, Line);
            }
        }

        /// <summary>
        /// Ejecuta el comando (en realidad se gestiona desde Executor, así que esto no hace nada).
        /// </summary>
        public void Execute(Executor executor)
        {
            // La lógica real del salto está en Executor, aquí no se hace nada.
        }

        public override string ToString()
        {
            return $"Goto [{TargetLabel}] ({ConditionText}) [línea {Line}]";
        }
    }
}
