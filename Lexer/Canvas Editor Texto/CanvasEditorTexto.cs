using System;
using System.Collections.Generic;

namespace Wall_E
{
    // Clase que simula el editor de texto/código del canvas.
    // Se encarga de almacenar el código fuente y de analizarlo usando el Lexer.
    public class CanvasEditorCodigo
    {
        // Variable privada que almacena el código fuente escrito por el usuario
        private string _codigoFuente = "";

        // Permite establecer el código fuente desde fuera de la clase (por ejemplo, desde una interfaz gráfica)
        public void SetCodigo(string codigo)
        {
            _codigoFuente = codigo;
        }

        // Permite obtener el código fuente almacenado actualmente
        public string GetCodigo()
        {
            return _codigoFuente;
        }

        // Analiza el código fuente almacenado usando el Lexer
        // Devuelve una lista de tokens generados a partir del código fuente
        public List<Token> AnalizarCodigo()
        {
            // Crea una instancia del Lexer, pasándole el código fuente
            Lexer lexer = new Lexer(_codigoFuente);
            // Llama al método Tokenize del Lexer para obtener la lista de tokens
            return lexer.Tokenize();
        }
    }
}