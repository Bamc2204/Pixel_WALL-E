using System;
using Wall_E; // Importa el enum TokenType

namespace Wall_E
{
    #region "Clase Token"

    /// <summary>
    /// Clase que representa un token individual generado por el lexer.
    /// Un token es una unidad mínima del lenguaje (palabra clave, número, símbolo, etc.).
    /// </summary>
    public class Token
    {
        #region "Propiedades"

        /// <summary>
        /// Tipo de token (por ejemplo, palabra clave, número, símbolo, etc.).
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// El texto exacto del código fuente que corresponde a este token.
        /// </summary>
        public string Lexeme { get; }

        /// <summary>
        /// Número de línea donde se encontró este token en el código fuente.
        /// </summary>
        public int Line { get; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// Inicializa el token con su tipo, texto y línea.
        /// </summary>
        public Token(TokenType type, string lexeme, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
        }

        #endregion

        #region "Métodos de Representación"

        /// <summary>
        /// Devuelve una representación legible del token, útil para depuración.
        /// </summary>
        public override string ToString()
        {
            return $"{Type} \"{Lexeme}\" (línea {Line})";
        }

        #endregion
    }

    #endregion
}