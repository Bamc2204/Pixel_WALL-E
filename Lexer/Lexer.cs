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
        #region "Campos y Constructor"

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

        #endregion

        #region "Tokenización principal"

        // Método principal: recorre el código fuente y genera la lista de tokens
        public List<Token> Tokenize()
        {
            List<Token> tokens = new();

            while (!IsAtEnd())
            {
                // Si el carácter actual es un espacio en blanco
                if (char.IsWhiteSpace(Peek()))
                {
                    // Si es salto de línea, aumenta el contador de línea y agrega un token NEWLINE
                    if (Peek() == '\n')
                    {
                        _line++;
                        _newLineJustPassed = true;
                        tokens.Add(new Token(TokenType.NEWLINE, "\\n", _line));
                    }
                    Advance(); // Avanza al siguiente carácter
                    continue;
                }

                // Si el carácter es una letra o un guion, puede ser palabra clave, identificador o etiqueta
                if (char.IsLetter(Peek()) || Peek() == '-')
                {
                    string word = ReadWhile(ch => char.IsLetterOrDigit(ch) || ch == '-');

                    // Si está al inicio de una línea, lo trata como etiqueta
                    if (_newLineJustPassed && (IsAtEnd() || Peek() == '\n'))
                    {
                        tokens.Add(new Token(TokenType.LABEL_DEF, word, _line));
                        _newLineJustPassed = false;
                    }
                    else
                    {
                        // Si no es etiqueta, verifica si es palabra clave o identificador
                        tokens.Add(KeywordOrIdentifier(word));
                        _newLineJustPassed = false;
                    }

                    continue;
                }

                // Si el carácter es un dígito, lee un número
                if (char.IsDigit(Peek()))
                {
                    string number = ReadWhile(char.IsDigit);
                    tokens.Add(new Token(TokenType.NUMBER, number, _line));
                    _newLineJustPassed = false;
                    continue;
                }

                // Si el carácter es una comilla, lee una cadena de texto
                if (Peek() == '"')
                {
                    Advance(); // consumir la primera comilla
                    string str = ReadString();
                    tokens.Add(new Token(TokenType.STRING, str, _line));
                    _newLineJustPassed = false;
                    continue;
                }

                // Si es cualquier otro carácter, intenta reconocerlo como símbolo u operador
                char c = Advance();
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

            // Al final, agrega un token de fin de archivo
            tokens.Add(new Token(TokenType.EOF, "", _line));
            return tokens;
        }

        #endregion

        #region "Métodos auxiliares de navegación"

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

        #endregion

        #region "Lectura de palabras, números y cadenas"

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

        #endregion

        #region "Palabras clave y identificadores"

        // Diccionario que asocia palabras clave con su tipo de token
        private static readonly Dictionary<string, TokenType> PalabrasClave = new()
        {
            { "Spawn", TokenType.SPAWN },
            { "Color", TokenType.COLOR },
            { "Size", TokenType.SIZE },
            { "DrawLine", TokenType.DRAW_LINE },
            { "DrawCircle", TokenType.DRAW_CIRCLE },
            { "DrawRectangle", TokenType.DRAW_RECTANGLE },
            { "Fill", TokenType.FILL },
            { "Goto", TokenType.GOTO },

            { "GetActualX", TokenType.GET_ACTUAL_X },
            { "GetActualY", TokenType.GET_ACTUAL_Y },
            { "GetCanvasSize", TokenType.GET_CANVAS_SIZE },
            { "GetColorCount", TokenType.GET_COLOR_COUNT },
            { "IsBrushColor", TokenType.IS_BRUSH_COLOR },
            { "IsBrushSize", TokenType.IS_BRUSH_SIZE },
            { "IsCanvasColor", TokenType.IS_CANVAS_COLOR },
        };

        // Determina si una palabra es palabra clave o identificador
        private Token KeywordOrIdentifier(string word)
        {
            if (PalabrasClave.TryGetValue(word, out var type))
            {
                return new Token(type, word, _line);
            }
            // Si no es palabra clave, es un identificador
            return new Token(TokenType.IDENTIFIER, word, _line);
        }

        #endregion

        #region "Reconocimiento de operadores y símbolos"

        // Reconoce operadores y símbolos individuales o compuestos
        private Token RecognizeSymbolOrOperator(char c)
        {
            switch (c)
            {
                case '+': return new Token(TokenType.PLUS, "+", _line);
                case '-': return new Token(TokenType.MINUS, "-", _line);
                case '*':
                    if (Match('*')) return new Token(TokenType.POWER, "**", _line);
                    return new Token(TokenType.MULTIPLY, "*", _line);
                case '/': return new Token(TokenType.DIVIDE, "/", _line);
                case '%': return new Token(TokenType.MOD, "%", _line);
                case '=':
                    if (Match('=')) return new Token(TokenType.EQUAL, "==", _line);
                    break;
                case '<':
                    if (Match('-')) return new Token(TokenType.ASSIGN, "<-", _line);
                    if (Match('=')) return new Token(TokenType.LESS_EQUAL, "<=", _line);
                    return new Token(TokenType.LESS, "<", _line);
                case '>':
                    if (Match('=')) return new Token(TokenType.GREATER_EQUAL, ">=", _line);
                    return new Token(TokenType.GREATER, ">", _line);
                case '&':
                    if (Match('&')) return new Token(TokenType.AND, "&&", _line);
                    break;
                case '|':
                    if (Match('|')) return new Token(TokenType.OR, "||", _line);
                    break;
                // Paréntesis, corchetes y coma
                case '(': return new Token(TokenType.LPAREN, "(", _line);
                case ')': return new Token(TokenType.RPAREN, ")", _line);
                case '[': return new Token(TokenType.LBRACKET, "[", _line);
                case ']': return new Token(TokenType.RBRACKET, "]", _line);
                case ',': return new Token(TokenType.COMMA, ",", _line);
            }

            // Si no reconoce el símbolo, devuelve null
            return null;
        }

        #endregion
    }
}