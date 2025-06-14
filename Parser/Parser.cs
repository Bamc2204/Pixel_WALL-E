using System;
using System.Collections.Generic;
#nullable enable

namespace Wall_E
{
    #region ParserClass

    /// <summary>
    /// Clase encargada de convertir una lista de tokens en una lista de comandos ejecutables (ICode).
    /// </summary>
    public class Parser
    {
        #region Fields

        // Lista de tokens a procesar
        private readonly List<Token> _tokens;

        // Índice del token actual
        private int _current = 0;

        // Lista compartida donde se almacenan los errores
        private readonly List<string> _errors;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor del parser que recibe la lista de tokens y la lista de errores compartida.
        /// </summary>
        public Parser(List<Token> tokens, List<string> errors)
        {
            _tokens = tokens;
            _errors = errors;
        }

        #endregion

        #region MainParseMethod

        /// <summary>
        /// Método principal de análisis sintáctico. Procesa todos los tokens y construye los comandos.
        /// </summary>
        public List<ICode> Parse(List<string> errors)
        {
            List<ICode> codes = new();

            while (!IsAtEnd())
            {
                try
                {
                    // Ignora saltos de línea
                    if (Match(TokenType.NEWLINE)) continue;

                    // Si encuentra un token desconocido, lanza error
                    if (Peek().Type == TokenType.UNKNOWN)
                    {
                        throw new InvalidCommandError(
                            Peek().Lexeme,
                            $"Argumento o instrucción no válida: '{Peek().Lexeme}'",
                            Peek().Line
                        );
                    }

                    // Intenta parsear un comando válido
                    ICode? code = ParseCode();
                    if (code != null) codes.Add(code);
                }
                catch (RuntimeError ex)
                {
                    _errors.Add($"[Parser Error] Línea {ex.Line}: {ex.Message}");
                    Advance();
                }
                catch (Exception ex)
                {
                    _errors.Add($"[Parser Error] Línea {Peek().Line}: {ex.Message}");
                    Advance();
                }
            }

            return codes;
        }

        #endregion

        #region CommandParsing

        /// <summary>
        /// Detecta y procesa un tipo de comando específico según el token actual.
        /// </summary>
        private ICode? ParseCode()
        {
            Token token = Peek();

            switch (token.Type)
            {
                case TokenType.SPAWN: return ParseGeneric<SpawnCommand>();
                case TokenType.COLOR: return ParseGeneric<ColorCommand>();
                case TokenType.SIZE: return ParseGeneric<SizeCommand>();
                case TokenType.DRAW_LINE: return ParseGeneric<DrawLineCommand>();
                case TokenType.DRAW_CIRCLE: return ParseGeneric<DrawCircleCommand>();
                case TokenType.DRAW_RECTANGLE: return ParseGeneric<DrawRectangleCommand>();
                case TokenType.FILL: return ParseGeneric<FillCommand>();
                case TokenType.LABEL_DEF:
                    return new LabelCommand { Name = Advance().Lexeme, Line = token.Line };
                case TokenType.GOTO:
                    return ParseGoto();
                case TokenType.IDENTIFIER:
                    if (CheckNext(TokenType.ASSIGN)) return ParseAssignment();
                    throw new InvalidCommandError(token.Lexeme, $"Instrucción no válida: '{token.Lexeme}'", token.Line);
                default:
                    throw new InvalidCommandError(token.Lexeme, $"Instrucción no válida: '{token.Lexeme}'", token.Line);
            }
        }

        /// <summary>
        /// Parsea una instrucción de asignación de variable.
        /// </summary>
        private ICode ParseAssignment()
        {
            Token id = Advance();
            string varName = id.Lexeme;
            int line = id.Line;
            Consume(TokenType.ASSIGN, "Falta '<-' en asignación");
            Expr expr = ParseExpression();
            return new AssignmentCommand { VariableName = varName, Expression = expr, Line = line };
        }

        /// <summary>
        /// Parsea una instrucción Goto con su condición.
        /// </summary>
        private ICode ParseGoto()
        {
            Token token = Advance();
            int line = token.Line;

            if (IsAtEnd()) throw new Exception("Falta '[' después de 'Goto'");
            if (!Match(TokenType.LBRACKET)) throw new Exception("Falta '[' después de 'Goto'");
            if (!Match(TokenType.IDENTIFIER)) throw new Exception("Se esperaba el nombre de la etiqueta");

            string label = Previous().Lexeme;

            if (!Match(TokenType.RBRACKET)) throw new Exception("Falta ']' después del nombre de la etiqueta");
            if (!Match(TokenType.LPAREN)) throw new Exception("Falta '(' para la condición del Goto");

            Expr condition = ParseExpression();

            if (!Match(TokenType.RPAREN)) throw new Exception("Falta ')' al final de la condición");

            return new GotoCommand
            {
                TargetLabel = label,
                Condition = condition,
                Line = line
            };
        }

        /// <summary>
        /// Parsea comandos gráficos genéricos.
        /// </summary>
        private ICode ParseGeneric<T>() where T : GraphicCommand, new()
        {
            Token t = Advance();
            int line = t.Line;
            Consume(TokenType.LPAREN, $"Falta '(' en {t.Lexeme}");
            List<Expr> args = new();
            if (!Check(TokenType.RPAREN))
            {
                do { args.Add(ParseExpression()); } while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RPAREN, "Falta ')' en lista de argumentos");
            return new T { Line = line, Arguments = args };
        }

        #endregion

        #region ExpressionParsing

        /// <summary>
        /// Parsea una expresión general.
        /// </summary>
        private Expr ParseExpression() => ParseComparison();

        /// <summary>
        /// Parsea expresiones con comparaciones (==, <, >, <=, >=).
        /// </summary>
        private Expr ParseComparison()
        {
            Expr expr = ParseAddSubtract();
            while (Match(TokenType.LESS) || Match(TokenType.LESS_EQUAL) ||
                Match(TokenType.GREATER) || Match(TokenType.GREATER_EQUAL) ||
                Match(TokenType.EQUAL))
            {
                string op = Previous().Lexeme;
                Expr right = ParseAddSubtract();
                expr = new BinaryExpr { Operator = op, Left = expr, Right = right, Line = expr.Line };
            }
            return expr;
        }

        /// <summary>
        /// Parsea operaciones de suma y resta.
        /// </summary>
        private Expr ParseAddSubtract()
        {
            Expr expr = ParseMultiply();
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS))
            {
                string op = Previous().Lexeme;
                Expr right = ParseMultiply();
                expr = new BinaryExpr { Operator = op, Left = expr, Right = right, Line = expr.Line };
            }
            return expr;
        }

        /// <summary>
        /// Parsea multiplicación, división, módulo y potencia.
        /// </summary>
        private Expr ParseMultiply()
        {
            Expr expr = ParsePrimary();
            while (Match(TokenType.MULTIPLY) || Match(TokenType.DIVIDE) || Match(TokenType.MOD) || Match(TokenType.POWER))
            {
                string op = Previous().Lexeme;
                Expr right = ParsePrimary();
                expr = new BinaryExpr { Operator = op, Left = expr, Right = right, Line = expr.Line };
            }
            return expr;
        }

        /// <summary>
        /// Parsea una expresión primaria como números, cadenas o variables.
        /// </summary>
        private Expr ParsePrimary()
        {
            // Soporta números negativos
            if (Match(TokenType.MINUS))
            {
                if (Match(TokenType.NUMBER))
                {
                    string value = "-" + Previous().Lexeme;
                    return new LiteralExpr { Value = value, Line = Previous().Line };
                }
                else
                {
                    throw new InvalidCommandError("-", "Se esperaba un número después de '-'", Peek().Line);
                }
            }

            if (Match(TokenType.NUMBER))
                return new LiteralExpr { Value = Previous().Lexeme, Line = Previous().Line };

            if (Match(TokenType.STRING))
                return new LiteralExpr { Value = $"\"{Previous().Lexeme}\"", Line = Previous().Line };

            if (Match(TokenType.IDENTIFIER))
                return new VariableExpr { Name = Previous().Lexeme, Line = Previous().Line };

            throw new InvalidCommandError(Peek().Lexeme, $"Instrucción no válida: '{Peek().Lexeme}'", Peek().Line);
        }

        #endregion

        #region TokenNavigationHelpers

        /// <summary>
        /// Consume un token si es del tipo especificado.
        /// </summary>
        private bool Match(TokenType type) => Check(type) && Advance() != null;

        /// <summary>
        /// Comprueba si el token actual es del tipo esperado.
        /// </summary>
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        /// <summary>
        /// Avanza al siguiente token y lo retorna.
        /// </summary>
        private Token Advance() => _tokens[_current++];

        /// <summary>
        /// Retorna el token actual.
        /// </summary>
        private Token Peek() => _tokens[_current];

        /// <summary>
        /// Retorna el token anterior.
        /// </summary>
        private Token Previous() => _tokens[_current - 1];

        /// <summary>
        /// Verifica si se ha llegado al final de la lista de tokens.
        /// </summary>
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        /// <summary>
        /// Verifica el tipo del siguiente token (no el actual).
        /// </summary>
        private bool CheckNext(TokenType type) => _current + 1 < _tokens.Count && _tokens[_current + 1].Type == type;

        /// <summary>
        /// Avanza si el token actual es del tipo esperado, si no lanza excepción.
        /// </summary>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw new Exception(message);
        }

        #endregion
    }

    #endregion
}
