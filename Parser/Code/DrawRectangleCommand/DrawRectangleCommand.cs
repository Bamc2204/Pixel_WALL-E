using System.Collections.Generic;

namespace Wall_E
{
    #region DrawRectangleCommand

    /// <summary>
    /// Comando que representa la instrucción DrawRectangle(ancho, alto).
    /// Dibuja un rectángulo desde la posición actual con el ancho y alto indicados.
    /// </summary>
    public class DrawRectangleCommand : GraphicCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor que asegura que los argumentos se inicialicen correctamente.
        /// </summary>
        public DrawRectangleCommand()
        {
            Arguments = new List<Expr>();
        }

        #endregion
    }

    #endregion
}