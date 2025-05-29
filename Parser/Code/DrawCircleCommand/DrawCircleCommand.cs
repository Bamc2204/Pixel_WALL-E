using System.Collections.Generic;

namespace Wall_E
{
    #region DrawCircleCommand

    /// <summary>
    /// Comando que representa la instrucción DrawCircle(radio).
    /// Dibuja un círculo con el radio especificado desde la posición actual.
    /// </summary>
    public class DrawCircleCommand : GraphicCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor vacío. Los argumentos se establecerán al parsear.
        /// </summary>
        public DrawCircleCommand()
        {
            Arguments = new List<Expr>();
        }

        #endregion
    }

    #endregion
}