using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region "Editor de Código para Canvas"

    /// <summary>
    /// Clase que simula el editor de texto/código del canvas.
    /// Se encarga de almacenar el código fuente y de analizarlo usando el Lexer.
    /// </summary>
    public class CanvasEditorCodigo
    {
        #region "Campos privados"

        // Variable privada que almacena el código fuente escrito por el usuario
        private string _codigoFuente = "";

        #endregion

        #region "Métodos para manipular el código fuente"

        /// <summary>
        /// Permite establecer el código fuente desde fuera de la clase (por ejemplo, desde una interfaz gráfica).
        /// </summary>
        public void SetCodigo(string codigo)
        {
            _codigoFuente = codigo;
        }

        /// <summary>
        /// Permite obtener el código fuente almacenado actualmente.
        /// </summary>
        public string GetCodigo()
        {
            return _codigoFuente;
        }

        #endregion

        #region "Análisis de código fuente"

        /// <summary>
        /// Analiza el código fuente almacenado usando el Lexer.
        /// Devuelve una lista de tokens generados a partir del código fuente.
        /// </summary>
        public List<Token> AnalizarCodigo()
        {
            // Crea una instancia del Lexer, pasándole el código fuente
            Lexer lexer = new Lexer(_codigoFuente);
            // Llama al método Tokenize del Lexer para obtener la lista de tokens
            return lexer.Tokenize();
        }

        #endregion
    }

    #endregion
}