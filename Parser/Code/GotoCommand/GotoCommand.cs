using System;

namespace Wall_E
{
    #region GotoCommandClass

    /// <summary>
    /// Representa un comando de salto condicional hacia una etiqueta.
    /// Sintaxis: Goto[etiqueta](condición)
    /// </summary>
    public class GotoCommand : ICode
    {
        #region Properties

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

        #endregion

        #region JumpLogic

        /// <summary>
        /// Evalúa la condición y determina si se debe hacer el salto.
        /// Si la condición es verdadera (distinta de cero), retorna true.
        /// Si ocurre un error de ejecución, lanza una excepción de comando inválido.
        /// </summary>
        /// <param name="executor">Executor que evalúa la condición.</param>
        /// <returns>True si se debe saltar, false si no.</returns>
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

        #endregion

        #region Execution

        /// <summary>
        /// Ejecuta el comando (en realidad se gestiona desde Executor, así que esto no hace nada).
        /// </summary>
        /// <param name="executor">Executor que gestiona la ejecución.</param>
        public void Execute(Executor executor)
        {
            // La lógica real del salto está en Executor, aquí no se hace nada.
        }

        #endregion

        #region ToStringRepresentation

        /// <summary>
        /// Devuelve una representación en texto del comando Goto, mostrando la etiqueta, condición y línea.
        /// </summary>
        public override string ToString()
        {
            return $"Goto [{TargetLabel}] ({ConditionText}) [línea {Line}]";
        }

        #endregion
    }

    #endregion
}