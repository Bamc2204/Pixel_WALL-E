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

        // Lista de tokens a analizar.
        private readonly List<Token> _tokens;
        // Índice del token actual.
        private int _current = 0;
        // Lista de errores encontrados durante el parseo.
        private readonly List<string> _errors;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el parser con la lista de tokens y la lista de errores.
        /// </summary>
        public Parser(List<Token> tokens, List<string> errors)
        {
            _tokens = tokens;
            _errors = errors;
        }

        #endregion

        #region MainParseMethod

        /// <summary>
        /// Método principal: recorre la lista de tokens y genera la lista de comandos ejecutables.
        /// Maneja errores de sintaxis y tokens desconocidos.
        /// </summary>
        public List<ICode> Parse(List<string> errors)
        {
            List<ICode> codes = new();
            while (!IsAtEnd())
            {
                try
                {
                    // Ignora saltos de línea.
                    if (Match(TokenType.NEWLINE)) continue;

                    // Control de tokens desconocidos.
                    if (Peek().Type == TokenType.UNKNOWN)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DEBUG] Token desconocido: '{Peek().Lexeme}' en línea {Peek().Line}");
                        throw new InvalidCommandError(
                            Peek().Lexeme,
                            $"Argumento o instrucción no válida: '{Peek().Lexeme}'",
                            Peek().Line
                        );
                    }

                    // Intenta parsear un comando.
                    ICode? code = ParseCode();
                    if (code != null) codes.Add(code);
                }
                catch (RuntimeError ex)
                {
                    errors.Add($"[Parser Error] Línea {ex.Line}: {ex.Message}");
                    Advance();
                }
                catch (Exception ex)
                {
                    errors.Add($"[Parser Error] Línea {Peek().Line}: {ex.Message}");
                    Advance();
                }
            }
            return codes;
        }

        #endregion

        #region CommandParsing

        /// <summary>
        /// Determina el tipo de comando a parsear según el token actual.
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
                case TokenType.LABEL_DEF: // Definición de etiqueta
                    return new LabelCommand { Name = Advance().Lexeme, Line = token.Line };
                case TokenType.GOTO: // Comando Goto
                    return ParseGoto();
                case TokenType.IDENTIFIER:
                    // Si el siguiente token es una asignación, parsea como asignación.
                    if (CheckNext(TokenType.ASSIGN)) return ParseAssignment();
                    System.Windows.Forms.MessageBox.Show($"Identificador fuera de contexto: '{token.Lexeme}' en línea {token.Line}");
                    throw new InvalidCommandError(token.Lexeme, $"Instrucción no válida: '{token.Lexeme}'", token.Line);
                default:
                    throw new InvalidCommandError(token.Lexeme, $"Instrucción no válida: '{token.Lexeme}'", token.Line);
            }
        }

        /// <summary>
        /// Parsea un comando de asignación (ejemplo: x <- 5).
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
        /// Parsea un comando Goto con su etiqueta y condición.
        /// </summary>
        private ICode ParseGoto()
        {
            Token token = Advance();
            int line = token.Line;
            Consume(TokenType.LBRACKET, "Falta '[' después de 'Goto'");
            Token label = Consume(TokenType.IDENTIFIER, "Se esperaba el nombre de la etiqueta");
            Consume(TokenType.RBRACKET, "Falta ']' después del nombre de la etiqueta");
            Consume(TokenType.LPAREN, "Falta '(' para la condición");

            // Parsea la condición como una expresión.
            Expr condition = ParseExpression();
            Consume(TokenType.RPAREN, "Falta ')' al final de la condición");

            return new GotoCommand
            {
                TargetLabel = label.Lexeme,
                Condition = condition,
                Line = line
            };
        }

        /// <summary>
        /// Parsea comandos gráficos genéricos usando reflexión.
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
        /// Parsea una expresión completa.
        /// </summary>
        private Expr ParseExpression() => ParseAddSubtract();

        /// <summary>
        /// Parsea sumas y restas.
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
        /// Parsea multiplicaciones, divisiones, módulos y potencias.
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
        /// Parsea expresiones primarias: números, cadenas e identificadores.
        /// </summary>
        private Expr ParsePrimary()
        {
            if (Match(TokenType.NUMBER)) return new LiteralExpr { Value = Previous().Lexeme, Line = Previous().Line };
            if (Match(TokenType.STRING)) return new LiteralExpr { Value = $"\"{Previous().Lexeme}\"", Line = Previous().Line };
            if (Match(TokenType.IDENTIFIER)) return new VariableExpr { Name = Previous().Lexeme, Line = Previous().Line };

            throw new InvalidCommandError(Peek().Lexeme, $"Instrucción no válida: '{Peek().Lexeme}'", Peek().Line);
        }

        #endregion

        #region TokenNavigationHelpers

        /// <summary>
        /// Avanza si el token actual es del tipo esperado.
        /// </summary>
        private bool Match(TokenType type) => Check(type) && Advance() != null;

        /// <summary>
        /// Verifica si el token actual es del tipo esperado.
        /// </summary>
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        /// <summary>
        /// Avanza al siguiente token y lo devuelve.
        /// </summary>
        private Token Advance() => _tokens[_current++];

        /// <summary>
        /// Devuelve el token actual sin avanzar.
        /// </summary>
        private Token Peek() => _tokens[_current];

        /// <summary>
        /// Devuelve el token anterior.
        /// </summary>
        private Token Previous() => _tokens[_current - 1];

        /// <summary>
        /// Verifica si se llegó al final de la lista de tokens.
        /// </summary>
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        /// <summary>
        /// Verifica si el siguiente token es del tipo esperado.
        /// </summary>
        private bool CheckNext(TokenType type) => _current + 1 < _tokens.Count && _tokens[_current + 1].Type == type;

        /// <summary>
        /// Consume el token esperado o lanza una excepción si no es el tipo correcto.
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