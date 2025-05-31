using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region CanvasEditorCode

    /// <summary>
    /// Clase que simula el editor de texto/código del canvas.
    /// Se encarga de almacenar el código fuente y de analizarlo usando el Lexer.
    /// </summary>
    public class CanvasEditorCode
    {
        #region PrivateFields

        // Variable privada que almacena el código fuente escrito por el usuario
        private string _sourceCode = "";

        #endregion

        #region SourceCodeManipulationMethods

        /// <summary>
        /// Permite establecer el código fuente desde fuera de la clase (por ejemplo, desde una interfaz gráfica).
        /// </summary>
        /// <param name="code">Nuevo código fuente a almacenar.</param>
        public void SetSourceCode(string code)
        {
            _sourceCode = code;
        }

        /// <summary>
        /// Permite obtener el código fuente almacenado actualmente.
        /// </summary>
        /// <returns>El código fuente actual.</returns>
        public string GetSourceCode()
        {
            return _sourceCode;
        }

        #endregion

        #region SourceCodeAnalysis

        /// <summary>
        /// Analiza el código fuente almacenado usando el Lexer.
        /// Devuelve una lista de tokens generados a partir del código fuente.
        /// </summary>
        /// <returns>Lista de tokens generados por el Lexer.</returns>
        public List<Token> AnalyzeSourceCode()
        {
            // Crea una instancia del Lexer, pasándole el código fuente
            Lexer lexer = new Lexer(_sourceCode);
            // Llama al método Tokenize del Lexer para obtener la lista de tokens
            return lexer.Tokenize();
        }

        #endregion
    }

    #endregion
}