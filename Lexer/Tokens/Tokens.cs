using System;
using Wall_E; // Importa el enum TokenType

namespace Wall_E
{
    // Clase que representa un token individual generado por el lexer
    public class Token
    {
        // Tipo de token (por ejemplo, palabra clave, número, símbolo, etc.)
        public TokenType Type { get; }
        // El texto exacto del código fuente que corresponde a este token
        public string Lexeme { get; }
        // Número de línea donde se encontró este token en el código fuente
        public int Line { get; }

        // Constructor: inicializa el token con su tipo, texto y línea
        public Token(TokenType type, string lexeme, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
        }

        // Devuelve una representación legible del token, útil para depuración
        public override string ToString()
        {
            return $"{Type} \"{Lexeme}\" (línea {Line})";
        }
    }
}