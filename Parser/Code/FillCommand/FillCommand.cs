using System.Collections.Generic;

namespace Wall_E
{
    #region FillCommand

    /// <summary>
    /// Comando que representa la instrucción Fill().
    /// Rellena la región actual cerrada con el color del pincel.
    /// Este comando no tiene argumentos.
    /// </summary>
    public class FillCommand : GraphicCommand
    {
        #region Constructors

        /// <summary>
        /// Constructor que asegura que la lista de argumentos está vacía.
        /// </summary>
        public FillCommand()
        {
            Arguments = new List<Expr>();
        }

        #endregion
    }

    #endregion
}