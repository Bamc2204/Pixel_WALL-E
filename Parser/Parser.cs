using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region ParserClass

    /// <summary>
    /// Clase encargada de analizar una lista de tokens y convertirlos en una lista de comandos (AST).
    /// </summary>
    public class Parser
    {
        #region FieldsAndConstructor

        // Lista de tokens a analizar
        private readonly List<Token> _tokens;
        // Índice actual en la lista de tokens
        private int _current = 0;

        /// <summary>
        /// Constructor que recibe la lista de tokens a procesar.
        /// </summary>
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        #endregion

        #region MainParseMethod

        /// <summary>
        /// Método principal que recorre todos los tokens y genera la lista de comandos.
        /// </summary>
        public List<Code> Parse()
        {
            List<Code> codes = new();

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
                    codes.Add(cmd);
                }
                else
                {
                    // Si no reconoce el comando, avanza al siguiente token
                    Advance();
                }
            }

            return codes;
        }

        #endregion

        #region CommandRecognition

        /// <summary>
        /// Reconoce el tipo de comando según el token actual y llama al método de análisis correspondiente.
        /// </summary>
        private Code? ParseCode()
        {
            Token token = Peek();

            switch (token.Type)
            {
                case TokenType.DRAW_LINE: return ParseGenericFunctionCall<DrawLineCommand>();
                case TokenType.DRAW_CIRCLE: return ParseGenericFunctionCall<DrawCircleCommand>();
                case TokenType.DRAW_RECTANGLE: return ParseGenericFunctionCall<DrawRectangleCommand>();
                case TokenType.FILL: return ParseGenericFunctionCall<FillCommand>();

                case TokenType.SPAWN: return ParseGenericFunctionCall<SpawnCommand>();
                case TokenType.COLOR: return ParseGenericFunctionCall<ColorCommand>();
                case TokenType.SIZE: return ParseGenericFunctionCall<SizeCommand>();

                case TokenType.LABEL_DEF: return ParseLabel();
                case TokenType.GOTO: return ParseGoto();
                case TokenType.IDENTIFIER: return CheckNext(TokenType.ASSIGN) ? ParseAssignment() : null;

                default:
                    Console.WriteLine($"[Line {token.Line}] Unrecognized command: {token.Lexeme}");
                    return null;
            }


        }

        #endregion

        #region ParseLabel

        /// <summary>
        /// Analiza una etiqueta y la convierte en un LabelCommand.
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
        /// Analiza un comando Goto, extrayendo la etiqueta de destino y la condición.
        /// </summary>
        private Code? ParseGoto()
        {
            Token token = Advance();
            int line = token.Line;

            if (!Match(TokenType.LBRACKET)) return Error("Falta '[' después de 'Goto'");
            if (!Match(TokenType.IDENTIFIER)) return Error("Se esperaba el nombre de la etiqueta dentro de [ ]");

            string label = Previous().Lexeme;

            if (!Match(TokenType.RBRACKET)) return Error("Falta ']' después del nombre de la etiqueta");
            if (!Match(TokenType.LPAREN)) return Error("Falta '(' para la condición del Goto");

            string condition = "";
            int depth = 1;

            // Lee la condición entre paréntesis, permitiendo anidamiento
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
        /// Analiza una asignación de variable.
        /// </summary>
        private Code? ParseAssignment()
        {
            Token id = Advance();
            string variable = id.Lexeme;
            int line = id.Line;

            if (!Match(TokenType.ASSIGN))
                throw new EmptyExpressionError("assignment", line);

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
        /// Analiza una expresión, soportando operadores aritméticos.
        /// </summary>
        private Expr ParseExpression() => ParseAddSubtract();

        /// <summary>
        /// Analiza sumas y restas.
        /// </summary>
        private Expr ParseAddSubtract()
        {
            Expr expr = ParseMultiply();

            while (Match(TokenType.PLUS) || Match(TokenType.MINUS))
            {
                string op = Previous().Lexeme;
                Expr right = ParseMultiply();
                expr = new BinaryExpr { Operator = op, Left = expr, Right = right };
            }

            return expr;
        }

        /// <summary>
        /// Analiza multiplicaciones, divisiones, módulo y potencia.
        /// </summary>
        private Expr ParseMultiply()
        {
            Expr expr = ParsePrimary();

            while (Match(TokenType.MULTIPLY) || Match(TokenType.DIVIDE) || Match(TokenType.MOD) || Match(TokenType.POWER))
            {
                string op = Previous().Lexeme;
                Expr right = ParsePrimary();
                expr = new BinaryExpr { Operator = op, Left = expr, Right = right };
            }

            return expr;
        }

        /// <summary>
        /// Analiza expresiones primarias: literales, variables y llamadas a funciones.
        /// </summary>
        private Expr ParsePrimary()
        {
            if (Match(TokenType.NUMBER)) return new LiteralExpr { Value = Previous().Lexeme };
            if (Match(TokenType.STRING)) return new LiteralExpr { Value = $"\"{Previous().Lexeme}\"" };

            if (Match(TokenType.IDENTIFIER) || Match(TokenType.IS_BRUSH_COLOR) || Match(TokenType.GET_ACTUAL_X)
                || Match(TokenType.GET_ACTUAL_Y) || Match(TokenType.GET_CANVAS_SIZE) || Match(TokenType.GET_COLOR_COUNT)
                || Match(TokenType.IS_BRUSH_SIZE) || Match(TokenType.IS_CANVAS_COLOR))
            {
                string name = Previous().Lexeme;
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
                return new VariableExpr { Name = name };
            }

            throw new Exception($"[Line {Peek().Line}] Invalid expression");
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Avanza si el siguiente token es del tipo esperado.
        /// </summary>
        private bool Match(TokenType type)
        {
            if (Check(type)) { Advance(); return true; }
            return false;
        }

        /// <summary>
        /// Verifica si el token actual es del tipo esperado.
        /// </summary>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        /// <summary>
        /// Avanza al siguiente token y retorna el anterior.
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
        /// Retorna el token actual.
        /// </summary>
        private Token Peek() => _tokens[_current];

        /// <summary>
        /// Retorna el token anterior.
        /// </summary>
        private Token Previous() => _tokens[_current - 1];

        /// <summary>
        /// Verifica si el siguiente token es del tipo esperado.
        /// </summary>
        private bool CheckNext(TokenType type) => _current + 1 < _tokens.Count && _tokens[_current + 1].Type == type;

        /// <summary>
        /// Muestra un error de sintaxis y retorna null.
        /// </summary>
        private Code? Error(string message)
        {
            Console.WriteLine($"[Line {Peek().Line}] Syntax error: {message}");
            return null;
        }

        /// <summary>
        /// Consume el token si es del tipo esperado, si no lanza excepción.
        /// </summary>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new Exception($"[Line {Peek().Line}] {message}");
        }

        #endregion

        #region GenericFunctionCallParser

        /// <summary>
        /// Analiza comandos gráficos genéricos que reciben argumentos entre paréntesis.
        /// </summary>
        private Code ParseGenericFunctionCall<T>() where T : GraphicCommand, new()
        {
            Token t = Advance();
            int line = t.Line;

            if (!Match(TokenType.LPAREN))
                throw new Exception($"[Line {line}] Expected '(' after {t.Lexeme}");

            List<Expr> args = new();

            if (!Check(TokenType.RPAREN))
            {
                do
                {
                    args.Add(ParseExpression());
                } while (Match(TokenType.COMMA));
            }

            Consume(TokenType.RPAREN, "Expected ')' after arguments");

            return new T { Line = line, Arguments = args };
        }

        #endregion
    }
    #endregion
}