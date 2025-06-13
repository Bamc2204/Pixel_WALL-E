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

        // Código fuente a analizar.
        private readonly string _source;
        // Índice del carácter actual en el código fuente.
        private int _current = 0;
        // Número de línea actual (para mensajes de error y tokens).
        private int _line = 1;
        // Indica si acaba de pasar un salto de línea (para detectar etiquetas).
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
                // Manejo de espacios y saltos de línea.
                if (char.IsWhiteSpace(Peek()))
                {
                    if (Peek() == '\n')
                    {
                        _line++;
                        _newLineJustPassed = true;
                        tokens.Add(new Token(TokenType.NEWLINE, "\\n", _line));
                    }
                    Advance();
                    continue;
                }

                // --- ETIQUETAS, PALABRAS CLAVE E IDENTIFICADORES ---
                if (char.IsLetter(Peek()) || Peek() == '_')
                {
                    // Lee una palabra formada por letras, dígitos y guion bajo.
                    string word = ReadWhile(ch => char.IsLetterOrDigit(ch) || ch == '_');

                    Token tokenAux = KeywordOrIdentifier(word);

                    // Si cumple las reglas de etiqueta y está sola en la línea, es LABEL_DEF
                    if (IsValidLabel(word) && _newLineJustPassed && IsLabelAloneOnLine())
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

                // Números
                if (char.IsDigit(Peek()))
                {
                    string number = ReadWhile(char.IsDigit);
                    tokens.Add(new Token(TokenType.NUMBER, number, _line));
                    _newLineJustPassed = false;
                    continue;
                }

                // Cadenas de texto
                if (Peek() == '"')
                {
                    Advance();
                    string str = ReadString();
                    tokens.Add(new Token(TokenType.STRING, str, _line));
                    _newLineJustPassed = false;
                    continue;
                }

                // Comentarios (líneas que empiezan con #)
                if (Peek() == '#')
                {
                    while (!IsAtEnd() && Peek() != '\n')
                        Advance();
                    continue;
                }

                // Símbolos y operadores
                char c = Advance();
                Token token = RecognizeSymbolOrOperator(c);
                tokens.Add(token);
                _newLineJustPassed = false;
            }

            // Token de fin de archivo
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
            if (!IsAtEnd()) Advance();
            return sb.ToString();
        }

        #endregion

        #region KeywordsAndIdentifiers

        // Diccionario de palabras clave del lenguaje y su tipo de token.
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
        /// Determina si una palabra es palabra clave, identificador o desconocido.
        /// </summary>
        /// <param name="word">Palabra a analizar.</param>
        /// <returns>Token correspondiente.</returns>
        private Token KeywordOrIdentifier(string word)
        {
            if (Keywords.TryGetValue(word, out var type))
            {
                return new Token(type, word, _line);
            }
            // Solo acepta identificadores válidos: empiezan por letra o '_', y el resto son letras, dígitos o '_'
            if (char.IsLetter(word[0]) || word[0] == '_')
            {
                foreach (char c in word)
                {
                    if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                        return new Token(TokenType.UNKNOWN, word, _line);
                }
                return new Token(TokenType.IDENTIFIER, word, _line);
            }
            return new Token(TokenType.UNKNOWN, word, _line);
        }

        #endregion

        #region SymbolAndOperatorRecognition

        /// <summary>
        /// Reconoce operadores y símbolos individuales o compuestos.
        /// </summary>
        /// <param name="c">Carácter a analizar.</param>
        /// <returns>Token correspondiente o UNKNOWN si no reconoce el símbolo.</returns>
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
                case '(': return new Token(TokenType.LPAREN, "(", _line);
                case ')': return new Token(TokenType.RPAREN, ")", _line);
                case '[': return new Token(TokenType.LBRACKET, "[", _line);
                case ']': return new Token(TokenType.RBRACKET, "]", _line);
                case ',': return new Token(TokenType.COMMA, ",", _line);
            }
            return new Token(TokenType.UNKNOWN, c.ToString(), _line);
        }

        #endregion

        #region LabelRecognitionHelpers

        /// <summary>
        /// Valida si una palabra cumple las reglas para ser etiqueta:
        /// - No puede empezar por número ni por '_'
        /// - Debe tener al menos una letra y al menos un guion bajo
        /// - Solo puede contener letras, dígitos y guion bajo
        /// </summary>
        private bool IsValidLabel(string word)
        {
            if (word.Length == 0 || char.IsDigit(word[0]) || word[0] == '_')
                return false;

            bool hasLetter = false;
            bool hasUnderscore = false;

            foreach (char c in word)
            {
                if (char.IsLetter(c))
                    hasLetter = true;
                else if (c == '_')
                    hasUnderscore = true;
                else if (!char.IsDigit(c))
                    return false; // Solo letras, dígitos y guion bajo permitidos
            }

            // Debe tener al menos una letra
            return hasLetter || hasUnderscore;
        }

        /// <summary>
        /// Verifica si la palabra está sola en la línea (solo espacios después).
        /// </summary>
        private bool IsLabelAloneOnLine()
        {
            int temp = _current;
            // Salta espacios en blanco
            while (temp < _source.Length && char.IsWhiteSpace(_source[temp]) && _source[temp] != '\n')
                temp++;
            // Debe ser fin de línea o fin de archivo
            return temp >= _source.Length || _source[temp] == '\n';
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
            int temp = _current;
            while (temp < _source.Length && char.IsWhiteSpace(_source[temp]))
                temp++;
            return temp + 1 < _source.Length &&
                _source[temp] == '<' &&
                _source[temp + 1] == '-';
        }

        #endregion
    }

    #endregion
}