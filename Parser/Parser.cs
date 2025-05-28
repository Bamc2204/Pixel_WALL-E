using System;
using System.Collections.Generic;
using Wall_E;

namespace Wall_E
{
    #region Parser Class 
    /// <summary>
    /// Clase encargada de analizar una lista de tokens y convertirlos en comandos ejecutables.
    /// </summary>
    public class Parser
    {
        #region FieldsAndConstructor

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

        #region MainParseMethod

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
                Code cmd = ParseCode();
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

        #region CommandRecognition

        /// <summary>
        /// Reconoce el tipo de comando según el token actual.
        /// </summary>
        /// <returns>Comando reconocido o null si no se reconoce.</returns>
        private Code? ParseCode()
        {
            Token token = Peek(); // Obtiene el token actual sin avanzar

            switch (token.Type)
            {
                case TokenType.DRAW_LINE: return ParseDrawLine(); // Si es DrawLine, llama a su parser
                case TokenType.SPAWN: return ParseSpawn();         // Si es Spawn, llama a su parser
                case TokenType.COLOR: return ParseColor();         // Si es Color, llama a su parser
                case TokenType.SIZE: return ParseSize();           // Si es Size, llama a su parser
                case TokenType.LABEL_DEF: return ParseLabel();     // Si es una etiqueta, llama a su parser
                case TokenType.GOTO: return ParseGoto();           // Si es Goto, llama a su parser
                case TokenType.IDENTIFIER: return CheckNext(TokenType.ASSIGN) ? ParseAssignment() : null; // Asignación
                default:
                    // Si el comando no es reconocido, muestra un mensaje de error
                    Console.WriteLine($"[Line {token.Line}] Unrecognized command: {token.Lexeme}");
                    return null;
            }
        }

        #endregion

        #region ParseLabel

        /// <summary>
        /// Analiza una etiqueta (LABEL_DEF).
        /// </summary>
        private Code? ParseLabel()
        {
            Token token = Advance();
            return new LabelCommand
            {
                Name = token.Lexeme,
                Line = token.Line
            };
        }

        #endregion

        #region ParseGoto

        /// <summary>
        /// Analiza el comando Goto con condición.
        /// </summary>
        private Code? ParseGoto()
        {
            Token token = Advance(); // consume 'Goto'
            int line = token.Line;

            if (!Match(TokenType.LBRACKET))
                return Error("Falta '[' después de 'Goto'");

            if (!Match(TokenType.IDENTIFIER))
                return Error("Se esperaba el nombre de la etiqueta dentro de [ ]");

            string label = Previous().Lexeme;

            if (!Match(TokenType.RBRACKET))
                return Error("Falta ']' después del nombre de la etiqueta");

            if (!Match(TokenType.LPAREN))
                return Error("Falta '(' para la condición del Goto");

            // Captura condicional como string crudo (hasta cerrar el paréntesis)
            string condition = "";
            int depth = 1;

            while (!IsAtEnd() && depth > 0)
            {
                Token t = Advance();
                if (t.Type == TokenType.LPAREN) depth++;
                if (t.Type == TokenType.RPAREN) depth--;

                if (depth > 0)
                    condition += t.Lexeme + " ";
            }

            condition = condition.Trim();

            return new GotoCommand
            {
                TargetLabel = label,
                ConditionText = condition,
                Line = line
            };
        }

        #endregion

        #region ParseAssignment

        /// <summary>
        /// Analiza una asignación de variable (IDENTIFIER <- expresión).
        /// </summary>
        private Code? ParseAssignment()
        {
            Token id = Advance();
            string variable = id.Lexeme;
            int line = id.Line;

            if (!Match(TokenType.ASSIGN))
                return Error("Falta '<-' después del nombre de la variable");

            Expr expr = ParseExpression();
            return new AssignmentCommand
            {
                VariableName = variable,
                Expression = expr,
                Line = line
            };
        }

        #endregion

        #region ParseExpressionWithOperators

        /// <summary>
        /// Analiza una expresión completa (soporta suma, resta, multiplicación, división, potencias, etc.).
        /// </summary>
        private Expr ParseExpression()
        {
            return ParseAddSubtract();
        }

        /// <summary>
        /// Analiza sumas y restas (precedencia menor).
        /// </summary>
        private Expr ParseAddSubtract()
        {
            Expr expr = ParseMultiply();

            while (Match(TokenType.PLUS) || Match(TokenType.MINUS))
            {
                string op = Previous().Lexeme;
                Expr right = ParseMultiply();
                expr = new BinaryExpr
                {
                    Operator = op,
                    Left = expr,
                    Right = right
                };
            }

            return expr;
        }

        /// <summary>
        /// Analiza multiplicaciones, divisiones, módulo y potencias (precedencia mayor).
        /// </summary>
        private Expr ParseMultiply()
        {
            Expr expr = ParsePrimary();

            while (Match(TokenType.MULTIPLY) || Match(TokenType.DIVIDE) || Match(TokenType.MOD) || Match(TokenType.POWER))
            {
                string op = Previous().Lexeme;
                Expr right = ParsePrimary();
                expr = new BinaryExpr
                {
                    Operator = op,
                    Left = expr,
                    Right = right
                };
            }

            return expr;
        }

        /// <summary>
        /// Analiza valores primarios: números, strings, variables, llamadas a funciones.
        /// </summary>
        private Expr ParsePrimary()
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
                string name = Previous().Lexeme;

                // Si es una llamada a función: nombre(...)
                if (Match(TokenType.LPAREN))
                {
                    List<Expr> args = new();
                    if (!Check(TokenType.RPAREN))
                    {
                        do
                        {
                            args.Add(ParseExpression());
                        } while (Match(TokenType.COMMA));
                    }
                    Consume(TokenType.RPAREN, "Falta ')' en llamada a función");
                    return new FunctionCallExpr { FunctionName = name, Arguments = args };
                }

                // Si es solo una variable
                return new VariableExpr { Name = name };
            }

            throw new Exception($"[Line {Peek().Line}] Invalid expression");
        }

        #endregion

        #region SpecificCommandParsers

        // ---------------------
        // Comando: DrawLine(x, y, d)
        // ---------------------
        /// <summary>
        /// Analiza el comando DrawLine(x, y, d).
        /// </summary>
        private Code? ParseDrawLine()
        {
            Token token = Advance(); // Consume el token DRAW_LINE
            int line = token.Line;   // Guarda el número de línea para mensajes de error

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de DrawLine");

            int? dirX = ParseNumber(); // Primer parámetro: dirección X
            if (dirX == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? dirY = ParseNumber(); // Segundo parámetro: dirección Y
            if (dirY == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de segundo número");

            int? dist = ParseNumber(); // Tercer parámetro: distancia
            if (dist == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de DrawLine");

            // Si todo fue bien, crea y devuelve el comando DrawLineCommand
            return new DrawLineCommand
            {
                DirX = dirX.Value,
                DirY = dirY.Value,
                Distance = dist.Value,
                Line = line
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
            int line = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Spawn");

            int? x = ParseNumber(); // Primer parámetro: X
            if (x == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? y = ParseNumber(); // Segundo parámetro: Y
            if (y == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Spawn");

            // Si todo fue bien, crea y devuelve el comando SpawnCommand
            return new SpawnCommand
            {
                X = x.Value,
                Y = y.Value,
                Line = line
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
            int line = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Color");

            // Espera un string como parámetro
            if (!Match(TokenType.STRING))
            {
                Console.WriteLine($"[Line {Peek().Line}] Se esperaba un string con el nombre del color");
                return null;
            }

            string color = Previous().Lexeme; // Obtiene el nombre del color

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Color");

            // Si todo fue bien, crea y devuelve el comando ColorCommand
            return new ColorCommand
            {
                ColorName = color,
                Line = line
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
            int line = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Size");

            int? value = ParseNumber(); // Espera un número como parámetro
            if (value == null) return null;

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Size");

            // Si todo fue bien, crea y devuelve el comando SizeCommand
            return new SizeCommand
            {
                Value = value.Value,
                Line = line
            };
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Intenta analizar un número, si el token actual es un número lo consume y lo devuelve.
        /// </summary>
        private int? ParseNumber()
        {
            if (Match(TokenType.NUMBER))
            {
                return int.Parse(Previous().Lexeme);
            }
            Console.WriteLine($"[Line {Peek().Line}] Se esperaba un número");
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
        private Code? Error(string message)
        {
            Console.WriteLine($"[Line {Peek().Line}] Syntax error: {message}");
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
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new Exception($"[Line {Peek().Line}] {message}");
        }

        #endregion
    }
    #endregion
}