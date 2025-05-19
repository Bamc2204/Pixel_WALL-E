using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Wall_E;

namespace Wall_E
{
    // Clase encargada de analizar el texto fuente y convertirlo en una lista de tokens
    public class Lexer
    {
        // Código fuente a analizar
        private readonly string _source;
        // Índice del carácter actual en el código fuente
        private int _current = 0;
        // Número de línea actual (para mensajes de error y tokens)
        private int _line = 1;
        // Indica si acaba de pasar un salto de línea (para detectar etiquetas)
        private bool _newLineJustPassed = true;

        // Constructor: recibe el código fuente como string
        public Lexer(string source)
        {
            _source = source;
        }

        // Método principal: recorre el código fuente y genera la lista de tokens
        public List<Token> Tokenize()
        {
            List<Token> tokens = new();

            // Mientras no se llegue al final del código fuente
            while (!IsAtEnd())
            {
                char c = Advance();

                // Ignora espacios en blanco y detecta saltos de línea
                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n')
                    {
                        _line++;
                        _newLineJustPassed = true;
                        tokens.Add(new Token(TokenType.NEWLINE, "\\n", _line));
                    }
                    continue;
                }
                // Identificadores, palabras clave y etiquetas
                if (char.IsLetter(c) || c == '-')
                {
                    // Lee una palabra completa (letras, dígitos o guiones)
                    string word = ReadWhile(ch => char.IsLetterOrDigit(ch) || ch == '-');

                    // Si está al inicio de una línea, lo trata como etiqueta
                    if (_newLineJustPassed && (IsAtEnd() || Peek() == '\n'))
                    {
                        tokens.Add(new Token(TokenType.LABEL_DEF, word, _line));
                        _newLineJustPassed = false;
                        continue;
                    }

                    // Si no es etiqueta, verifica si es palabra clave o identificador
                    tokens.Add(KeywordOrIdentifier(word));
                    _newLineJustPassed = false;
                }
                // Números
                else if (char.IsDigit(c))
                {
                    // Lee todos los dígitos consecutivos
                    string number = ReadWhile(char.IsDigit);
                    tokens.Add(new Token(TokenType.NUMBER, number, _line));
                    _newLineJustPassed = false;
                }
                // Cadenas de texto entre comillas
                else if (c == '"')
                {
                    // Lee hasta el cierre de comillas
                    string str = ReadString();
                    tokens.Add(new Token(TokenType.STRING, str, _line));
                    _newLineJustPassed = false;
                }
                // Operadores y símbolos
                else
                {
                    // Intenta reconocer el símbolo u operador
                    Token token = RecognizeSymbolOrOperator(c);
                    if (token != null)
                    {
                        tokens.Add(token);
                        _newLineJustPassed = false;
                    }
                    else
                    {
                        // Si no reconoce el símbolo, lo marca como desconocido
                        tokens.Add(new Token(TokenType.UNKNOWN, c.ToString(), _line));
                    }
                }
            }

            // Al final, agrega un token de fin de archivo
            tokens.Add(new Token(TokenType.EOF, "", _line));
            return tokens;
        }

        // --------- Métodos auxiliares para el análisis léxico ---------

        // Verifica si se llegó al final del código fuente
        private bool IsAtEnd() => _current >= _source.Length;

        // Avanza al siguiente carácter y lo devuelve
        private char Advance() => _source[_current++];

        // Devuelve el carácter actual sin avanzar
        private char Peek() => IsAtEnd() ? '\0' : _source[_current];

        // Devuelve el carácter siguiente sin avanzar
        private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];

        // Si el carácter actual es el esperado, avanza y devuelve true; si no, devuelve false
        private bool Match(char expected)
        {
            if (IsAtEnd() || Peek() != expected) return false;
            _current++;
            return true;
        }

        // Lee caracteres mientras se cumpla la condición dada
        private string ReadWhile(Func<char, bool> condition)
        {
            int start = _current;
            while (!IsAtEnd() && condition(Peek()))
                Advance();
            return _source[start.._current];
        }

        // Lee una cadena de texto entre comillas
        private string ReadString()
        {
            StringBuilder sb = new StringBuilder();
            while (!IsAtEnd() && Peek() != '"')
            {
                if (Peek() == '\n') _line++;
                sb.Append(Advance());
            }
            if (!IsAtEnd()) Advance(); // Consume el cierre de comillas
            return sb.ToString();
        }

        // Determina si una palabra es palabra clave o identificador
        private Token KeywordOrIdentifier(string word)
        {
            // Intenta convertir la palabra a un valor del enum TokenType
            if (Enum.TryParse(typeof(TokenType), word.ToUpper(CultureInfo.InvariantCulture), out var tokenType))
            {
                return new Token((TokenType)tokenType, word, _line);
            }
            // Si no es palabra clave, es un identificador
            return new Token(TokenType.IDENTIFIER, word, _line);
        }

        // Reconoce operadores y símbolos individuales o compuestos
        private Token RecognizeSymbolOrOperator(char c)
        {
            switch (c)
            {
                case '=':
                    // == o =
                    return Match('=') ? new Token(TokenType.EQUAL, "==", _line) : new Token(TokenType.ASSIGN, "=", _line);
                case '+':
                    return new Token(TokenType.PLUS, "+", _line);
                case '-':
                    return new Token(TokenType.MINUS, "-", _line);
                case '*':
                    // ** o *
                    return Match('*') ? new Token(TokenType.POWER, "**", _line) : new Token(TokenType.MULTIPLY, "*", _line);
                case '/':
                    return new Token(TokenType.DIVIDE, "/", _line);
                case '%':
                    return new Token(TokenType.MOD, "%", _line);
                case '>':
                    // >= o >
                    return Match('=') ? new Token(TokenType.GREATER_EQUAL, ">=", _line) : new Token(TokenType.GREATER, ">", _line);
                case '<':
                    // <=, <- o <
                    return Match('=') ? new Token(TokenType.LESS_EQUAL, "<=", _line) : Match('-') ? new Token(TokenType.LESS_EQUAL, "<-", _line) : new Token(TokenType.LESS, "<", _line);
                case '&':
                    // &&
                    if (Match('&')) return new Token(TokenType.AND, "&&", _line);
                    break;
                case '|':
                    // ||
                    if (Match('|')) return new Token(TokenType.OR, "||", _line);
                    break;
            }
            // Si no reconoce el símbolo, devuelve null
            return null;
        }
    }
}