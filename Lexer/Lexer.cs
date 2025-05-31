using System;
using System.Collections.Generic;
using System.Text;

namespace Wall_E
{
    #region LexerClass

    /// <summary>
    /// Clase encargada de analizar el texto fuente y convertirlo en una lista de tokens.
    /// </summary>
    public class Lexer
    {
        #region FieldsAndConstructor

        // Código fuente a analizar
        private readonly string _source;
        // Índice del carácter actual en el código fuente
        private int _current = 0;
        // Número de línea actual (para mensajes de error y tokens)
        private int _line = 1;
        // Indica si acaba de pasar un salto de línea (para detectar etiquetas)
        private bool _newLineJustPassed = true;

        /// <summary>
        /// Constructor: recibe el código fuente como string.
        /// </summary>
        /// <param name="source">El código fuente a analizar.</param>
        public Lexer(string source)
        {
            _source = source;
        }

        #endregion

        #region MainTokenization

        /// <summary>
        /// Método principal: recorre el código fuente y genera la lista de tokens.
        /// </summary>
        /// <returns>Lista de tokens generados a partir del código fuente.</returns>
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

                // Si el carácter es una letra o un guion bajo, puede ser palabra clave, identificador o etiqueta
                if (char.IsLetter(Peek()) || Peek() == '_')
                {
                    // Lee la palabra completa (letras, dígitos o guion bajo)
                    string word = ReadWhile(ch => char.IsLetterOrDigit(ch) || ch == '_');

                    // Detectar si es palabra clave primero
                    Token tokenAux = KeywordOrIdentifier(word);

                    // Si es una palabra clave, siempre la usamos como tal
                    if (tokenAux.Type != TokenType.IDENTIFIER)
                    {
                        tokens.Add(tokenAux);
                    }
                    // Si está al inicio de línea y no tiene asignación, es una etiqueta
                    else if (_newLineJustPassed && !IsAssignmentJustAfter())
                    {
                        tokens.Add(new Token(TokenType.LABEL_DEF, word, _line));
                    }
                    else
                    {
                        tokens.Add(tokenAux);
                    }

                    _newLineJustPassed = false;
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

        #region NavigationHelpers

        /// <summary>
        /// Verifica si se llegó al final del código fuente.
        /// </summary>
        private bool IsAtEnd() => _current >= _source.Length;

        /// <summary>
        /// Avanza al siguiente carácter y lo devuelve.
        /// </summary>
        private char Advance() => _source[_current++];

        /// <summary>
        /// Devuelve el carácter actual sin avanzar.
        /// </summary>
        private char Peek() => IsAtEnd() ? '\0' : _source[_current];

        /// <summary>
        /// Devuelve el carácter siguiente sin avanzar.
        /// </summary>
        private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];

        /// <summary>
        /// Si el carácter actual es el esperado, avanza y devuelve true; si no, devuelve false.
        /// </summary>
        private bool Match(char expected)
        {
            if (IsAtEnd() || Peek() != expected) return false;
            _current++;
            return true;
        }

        #endregion

        #region ReadWordsNumbersStrings

        /// <summary>
        /// Lee caracteres mientras se cumpla la condición dada.
        /// </summary>
        /// <param name="condition">Función que indica si se debe seguir leyendo.</param>
        /// <returns>Cadena leída.</returns>
        private string ReadWhile(Func<char, bool> condition)
        {
            int start = _current;
            while (!IsAtEnd() && condition(Peek()))
                Advance();
            return _source[start.._current];
        }

        /// <summary>
        /// Lee una cadena de texto entre comillas.
        /// </summary>
        /// <returns>Cadena leída.</returns>
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

        #region KeywordsAndIdentifiers

        // Diccionario que asocia palabras clave con su tipo de token
        private static readonly Dictionary<string, TokenType> Keywords = new()
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

        /// <summary>
        /// Determina si una palabra es palabra clave o identificador.
        /// </summary>
        /// <param name="word">Palabra a analizar.</param>
        /// <returns>Token correspondiente.</returns>
        private Token KeywordOrIdentifier(string word)
        {
            if (Keywords.TryGetValue(word, out var type))
            {
                return new Token(type, word, _line);
            }
            // Si no es palabra clave, es un identificador
            return new Token(TokenType.IDENTIFIER, word, _line);
        }

        #endregion

        #region SymbolAndOperatorRecognition

        /// <summary>
        /// Reconoce operadores y símbolos individuales o compuestos.
        /// </summary>
        /// <param name="c">Carácter a analizar.</param>
        /// <returns>Token correspondiente o null si no reconoce el símbolo.</returns>
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

        #region ContextAnalysisUtilities

        /// <summary>
        /// Verifica si el siguiente símbolo es una asignación pendiente ("<-").
        /// </summary>
        private bool IsAssignmentPending()
        {
            return Peek() == '<' && PeekNext() == '-';
        }

        /// <summary>
        /// Verifica si el siguiente símbolo es el inicio de una función inmediata ("(").
        /// </summary>
        private bool IsImmediateFunction()
        {
            return Peek() == '(';
        }

        /// <summary>
        /// Verifica si justo después de la palabra hay un "<-".
        /// </summary>
        private bool IsAssignmentJustAfter()
        {
            // Guardamos posición temporal
            int temp = _current;

            // Saltamos espacios en blanco
            while (temp < _source.Length && char.IsWhiteSpace(_source[temp]))
                temp++;

            // Verificamos si hay un "<-"
            return temp + 1 < _source.Length &&
                _source[temp] == '<' &&
                _source[temp + 1] == '-';
        }

        #endregion
    }

    #endregion
}