using System;
using System.Collections.Generic;
using Wall_E;

namespace Wall_E
{
    /// <summary>
    /// Clase encargada de analizar una lista de tokens y convertirlos en comandos ejecutables.
    /// </summary>
    public class Parser
    {
        #region "Campos y Constructor"
        
        // Lista de tokens generados por el lexer
        private readonly List<Token> _tokens;
        // Índice del token actual que se está procesando
        private int _current = 0;

        /// <summary>
        /// Constructor: recibe la lista de tokens a analizar.
        /// </summary>
        /// <param name="tokens">Lista de tokens generados por el lexer.</param>
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        #endregion

        #region "Método principal de análisis"

        /// <summary>
        /// Método principal: recorre todos los tokens y construye una lista de comandos válidos.
        /// </summary>
        /// <returns>Lista de comandos reconocidos.</returns>
        public List<Code> Parse()
        {
            List<Code> codes = new();

            // Mientras no se llegue al final de los tokens
            while (!IsAtEnd())
            {
                // Ignora líneas vacías
                if (Peek().Type == TokenType.NEWLINE)
                {
                    Advance();
                    continue;
                }

                // Intenta analizar un comando
                Code cmd = ParserCode();
                if (cmd != null)
                {
                    codes.Add(cmd); // Agrega el comando reconocido a la lista
                }
                else
                {
                    Advance(); // Evita bucle infinito si no se reconoce el comando
                }
            }

            // Devuelve la lista de comandos reconocidos
            return codes;
        }

        #endregion

        #region "Reconocimiento de comandos"

        /// <summary>
        /// Reconoce el tipo de comando según el token actual.
        /// </summary>
        /// <returns>Comando reconocido o null si no se reconoce.</returns>
        private Code? ParserCode()
        {
            Token token = Peek(); // Obtiene el token actual sin avanzar

            switch (token.Type)
            {
                case TokenType.DRAW_LINE: return ParseDrawLine(); // Si es DrawLine, llama a su parser
                case TokenType.SPAWN: return ParseSpawn();         // Si es Spawn, llama a su parser
                case TokenType.COLOR: return ParseColor();         // Si es Color, llama a su parser
                case TokenType.SIZE: return ParseSize();           // Si es Size, llama a su parser
                case TokenType.LABEL_DEF: return ParseEtiqueta();  // Si es una etiqueta, llama a su parser
                case TokenType.GOTO: return ParseGoto();           // Si es Goto, llama a su parser
                case TokenType.IDENTIFIER: return CheckNext(TokenType.ASSIGN) ? ParseAsignacion() : null; // Asignación
                default:
                    // Si el comando no es reconocido, muestra un mensaje de error
                    Console.WriteLine($"[Línea {token.Line}] Comando no reconocido: {token.Lexeme}");
                    return null;
            }
        }

        #endregion

        #region "ParseEtiqueta"

        /// <summary>
        /// Analiza una etiqueta (LABEL_DEF).
        /// </summary>
        private Code? ParseEtiqueta()
        {
            Token token = Advance();
            return new LabelCommand
            {
                Nombre = token.Lexeme,
                Linea = token.Line
            };
        }

        #endregion

        #region "ParseGoto"

        /// <summary>
        /// Analiza un comando Goto.
        /// </summary>
        private Code? ParseGoto()
        {
            Token gotoToken = Advance(); // consume GOTO
            int linea = gotoToken.Line;

            if (!Match(TokenType.LBRACKET))
                return Error("Falta '[' después de Goto");

            if (!Match(TokenType.IDENTIFIER) && !Match(TokenType.LABEL_DEF))
                return Error("Se esperaba el nombre de la etiqueta entre corchetes");

            string nombreEtiqueta = Previous().Lexeme;

            if (!Match(TokenType.RBRACKET))
                return Error("Falta ']' después del nombre de la etiqueta");

            if (!Match(TokenType.LPAREN))
                return Error("Falta '(' con la condición del Goto");

            // Leer condición como texto crudo (hasta cerrar el paréntesis)
            string condicion = "";

            while (!IsAtEnd() && !Check(TokenType.RPAREN))
            {
                condicion += Advance().Lexeme + " ";
            }

            if (!Match(TokenType.RPAREN))
                return Error("Falta ')' al final de la condición del Goto");

            return new GotoCommand
            {
                EtiquetaDestino = nombreEtiqueta,
                CondicionTexto = condicion.Trim(),
                Linea = linea
            };
        }

        #endregion

        #region "ParseAsignacion"

        /// <summary>
        /// Analiza una asignación de variable (IDENTIFIER <- expresión).
        /// </summary>
        private Code? ParseAsignacion()
        {
            Token id = Advance();
            string variable = id.Lexeme;
            int linea = id.Line;

            if (!Match(TokenType.ASSIGN))
                return Error("Falta '<-' después del nombre de la variable");

            Expr expr = ParseExpresion();
            return new AssignmentCommand
            {
                VariableName = variable,
                Expression = expr,
                Linea = linea
            };
        }

        #endregion

        #region "ParseExpresion (con operadores)"

        /// <summary>
        /// Analiza una expresión completa (soporta suma, resta, multiplicación, división, potencias, etc.).
        /// </summary>
        private Expr ParseExpresion()
        {
            return ParseSumaResta();
        }

        /// <summary>
        /// Analiza sumas y restas (precedencia menor).
        /// </summary>
        private Expr ParseSumaResta()
        {
            Expr expr = ParseMultiplicacion();

            while (Match(TokenType.PLUS) || Match(TokenType.MINUS))
            {
                string operador = Previous().Lexeme;
                Expr derecho = ParseMultiplicacion();
                expr = new BinaryExpr
                {
                    Operator = operador,
                    Left = expr,
                    Right = derecho
                };
            }

            return expr;
        }

        /// <summary>
        /// Analiza multiplicaciones, divisiones, módulo y potencias (precedencia mayor).
        /// </summary>
        private Expr ParseMultiplicacion()
        {
            Expr expr = ParsePrimaria();

            while (Match(TokenType.MULTIPLY) || Match(TokenType.DIVIDE) || Match(TokenType.MOD) || Match(TokenType.POWER))
            {
                string operador = Previous().Lexeme;
                Expr derecho = ParsePrimaria();
                expr = new BinaryExpr
                {
                    Operator = operador,
                    Left = expr,
                    Right = derecho
                };
            }

            return expr;
        }

        /// <summary>
        /// Analiza valores primarios: números, strings, variables, llamadas a funciones.
        /// </summary>
        private Expr ParsePrimaria()
        {
            if (Match(TokenType.NUMBER))
                return new LiteralExpr { Value = Previous().Lexeme };

            if (Match(TokenType.STRING))
                return new LiteralExpr { Value = $"\"{Previous().Lexeme}\"" };

            // Identificadores, funciones y variables
            if (Match(TokenType.IDENTIFIER) || Match(TokenType.IS_BRUSH_COLOR) || Match(TokenType.GET_ACTUAL_X)
                || Match(TokenType.GET_ACTUAL_Y) || Match(TokenType.GET_CANVAS_SIZE) || Match(TokenType.GET_COLOR_COUNT)
                || Match(TokenType.IS_BRUSH_SIZE) || Match(TokenType.IS_CANVAS_COLOR))
            {
                string nombre = Previous().Lexeme;

                // Si es una llamada a función: nombre(...)
                if (Match(TokenType.LPAREN))
                {
                    List<Expr> args = new();
                    if (!Check(TokenType.RPAREN))
                    {
                        do
                        {
                            args.Add(ParseExpresion());
                        } while (Match(TokenType.COMMA));
                    }
                    Consume(TokenType.RPAREN, "Falta ')' en llamada a función");
                    return new FunctionCallExpr { FunctionName = nombre, Arguments = args };
                }

                // Si es solo una variable
                return new VariableExpr { Name = nombre };
            }

            throw new Exception($"[Línea {Peek().Line}] Expresión no válida");
        }

        #endregion

        #region "Parsers específicos para cada comando"

        // ---------------------
        // Comando: DrawLine(x, y, d)
        // ---------------------
        /// <summary>
        /// Analiza el comando DrawLine(x, y, d).
        /// </summary>
        private Code? ParseDrawLine()
        {
            Token token = Advance(); // Consume el token DRAW_LINE
            int linea = token.Line;  // Guarda el número de línea para mensajes de error

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de DrawLine");

            int? dirX = ParseNumero(); // Primer parámetro: dirección X
            if (dirX == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? dirY = ParseNumero(); // Segundo parámetro: dirección Y
            if (dirY == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de segundo número");

            int? dist = ParseNumero(); // Tercer parámetro: distancia
            if (dist == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de DrawLine");

            // Si todo fue bien, crea y devuelve el comando DrawLineCommand
            return new DrawLineCommand
            {
                DirX = dirX.Value,
                DirY = dirY.Value,
                Distance = dist.Value,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Spawn(x, y)
        // ---------------------
        /// <summary>
        /// Analiza el comando Spawn(x, y).
        /// </summary>
        private Code? ParseSpawn()
        {
            Token token = Advance(); // Consume el token SPAWN
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Spawn");

            int? x = ParseNumero(); // Primer parámetro: X
            if (x == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? y = ParseNumero(); // Segundo parámetro: Y
            if (y == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Spawn");

            // Si todo fue bien, crea y devuelve el comando SpawnCommand
            return new SpawnCommand
            {
                X = x.Value,
                Y = y.Value,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Color("nombre")
        // ---------------------
        /// <summary>
        /// Analiza el comando Color("nombre").
        /// </summary>
        private Code? ParseColor()
        {
            Token token = Advance(); // Consume el token COLOR
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Color");

            // Espera un string como parámetro
            if (!Match(TokenType.STRING))
            {
                Console.WriteLine($"[Línea {Peek().Line}] Se esperaba un string con el nombre del color");
                return null;
            }

            string color = Previous().Lexeme; // Obtiene el nombre del color

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Color");

            // Si todo fue bien, crea y devuelve el comando ColorCommand
            return new ColorCommand
            {
                NombreColor = color,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Size(n)
        // ---------------------
        /// <summary>
        /// Analiza el comando Size(n).
        /// </summary>
        private Code? ParseSize()
        {
            Token token = Advance(); // Consume el token SIZE
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Size");

            int? valor = ParseNumero(); // Espera un número como parámetro
            if (valor == null) return null;

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Size");

            // Si todo fue bien, crea y devuelve el comando SizeCommand
            return new SizeCommand
            {
                Valor = valor.Value,
                Linea = linea
            };
        }

        #endregion

        #region "Utilidades y métodos auxiliares"

        /// <summary>
        /// Intenta analizar un número, si el token actual es un número lo consume y lo devuelve.
        /// </summary>
        private int? ParseNumero()
        {
            if (Match(TokenType.NUMBER))
            {
                return int.Parse(Previous().Lexeme);
            }
            Console.WriteLine($"[Línea {Peek().Line}] Se esperaba un número");
            return null;
        }

        /// <summary>
        /// Si el token actual es del tipo esperado, lo consume y devuelve true; si no, devuelve false.
        /// </summary>
        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica si el token actual es del tipo esperado sin consumirlo.
        /// </summary>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        /// <summary>
        /// Avanza al siguiente token y devuelve el token anterior.
        /// </summary>
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        /// <summary>
        /// Verifica si se llegó al final de la lista de tokens.
        /// </summary>
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        /// <summary>
        /// Devuelve el token actual sin avanzar.
        /// </summary>
        private Token Peek() => _tokens[_current];

        /// <summary>
        /// Devuelve el token anterior al actual.
        /// </summary>
        private Token Previous() => _tokens[_current - 1];

        /// <summary>
        /// Muestra un mensaje de error y devuelve null.
        /// </summary>
        private Code? Error(string mensaje)
        {
            Console.WriteLine($"[Línea {Peek().Line}] Error de sintaxis: {mensaje}");
            return null;
        }

        /// <summary>
        /// Verifica si el siguiente token es del tipo esperado.
        /// </summary>
        private bool CheckNext(TokenType type)
        {
            if (_current + 1 >= _tokens.Count) return false;
            return _tokens[_current + 1].Type == type;
        }

        /// <summary>
        /// Consume un token del tipo esperado o lanza excepción si no lo encuentra.
        /// </summary>
        private Token Consume(TokenType type, string mensaje)
        {
            if (Check(type)) return Advance();
            throw new Exception($"[Línea {Peek().Line}] {mensaje}");
        }

        #endregion
    }
}