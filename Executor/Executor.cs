using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wall_E
{
    /// <summary>
    /// Clase responsable de ejecutar instrucciones y gestionar el estado del canvas y variables.
    /// </summary>
    public class Executor
    {
        #region Fields

        // Diccionario para almacenar variables y sus valores
        private readonly Dictionary<string, int> _variables = new();
        // Referencia al canvas donde se dibuja
        public PixelCanvas Canvas { get; }
        // Color actual del pincel
        public Color BrushColor { get; private set; } = Color.Black;
        // Tamaño actual del pincel
        public int BrushSize { get; private set; } = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el executor con el canvas de dibujo.
        /// </summary>
        /// <param name="canvas">Canvas donde se realizarán los dibujos.</param>
        public Executor(PixelCanvas canvas)
        {
            Canvas = canvas;
        }

        #endregion

        #region Execute Method

        /// <summary>
        /// Ejecuta una lista de instrucciones (ICode).
        /// Gestiona saltos, ejecución y errores.
        /// </summary>
        /// <param name="codes">Lista de instrucciones a ejecutar.</param>
        public void Execute(List<ICode> codes)
        {
            var labelMap = BuildLabelMap(codes);

            for (int i = 0; i < codes.Count; i++)
            {
                ICode code = codes[i];

                try
                {
                    // Si es un comando Goto, evalúa si debe saltar
                    if (code is GotoCommand gotoCmd)
                    {
                        if (gotoCmd.ShouldJump(this))
                        {
                            if (labelMap.TryGetValue(gotoCmd.TargetLabel, out int index))
                            {
                                i = index - 1; // Salta a la etiqueta
                                continue;
                            }
                            else
                            {
                                throw new LabelNotFoundError(gotoCmd.TargetLabel, code.Line);
                            }
                        }
                    }
                    else
                    {
                        // Ejecuta el comando normalmente
                        code.Execute(this);
                    }
                }
                catch (RuntimeError e)
                {
                    // Muestra errores personalizados de ejecución
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (Exception e)
                {
                    // Muestra errores inesperados
                    Console.WriteLine($"Unexpected Error: {e.Message}");
                }
            }
        }

        #endregion

        #region Expression Evaluation

        /// <summary>
        /// Evalúa una expresión y retorna su valor entero.
        /// </summary>
        /// <param name="expr">Expresión a evaluar.</param>
        /// <param name="line">Línea de código para contexto de error.</param>
        public int EvaluateExpression(Expr expr, int line = 0)
        {
            return expr switch
            {
                LiteralExpr l => ParseLiteral(l.Value, line),
                VariableExpr v => _variables.TryGetValue(v.Name, out var val)
                    ? val
                    : throw new UndefinedVariableError(v.Name, line),
                BinaryExpr b => EvaluateBinary(b, line),
                FunctionCallExpr f => EvaluateFunctionCall(f, line),
                _ => throw new EmptyExpressionError("Unknown expression type", line)
            };
        }

        /// <summary>
        /// Convierte un literal a entero, lanza error si no es válido.
        /// </summary>
        private int ParseLiteral(string value, int line)
        {
            if (int.TryParse(value, out int number))
                return number;

            throw new InvalidLiteralError(value, line);
        }

        /// <summary>
        /// Evalúa una expresión binaria (+, -, *, /, %, **).
        /// </summary>
        private int EvaluateBinary(BinaryExpr b, int line)
        {
            int left = EvaluateExpression(b.Left, line);
            int right = EvaluateExpression(b.Right, line);

            return b.Operator switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right != 0 ? left / right : throw new DivisionByZeroError(line),
                "%" => left % right,
                "**" => (int)Math.Pow(left, right),
                _ => throw new UnknownOperatorError(b.Operator, line)
            };
        }

        /// <summary>
        /// Evalúa una llamada a función especial (GetActualX, IsBrushColor, etc).
        /// </summary>
        private int EvaluateFunctionCall(FunctionCallExpr f, int line)
        {
            string name = f.FunctionName;

            return name switch
            {
                "GetActualX" => Canvas.Width / 2,
                "GetActualY" => Canvas.Height / 2,
                "GetCanvasSize" => Canvas.Width,
                "GetColorCount" => 10,
                "IsBrushColor" => EvalIsBrushColor(f, line),
                "IsBrushSize" => EvalIsBrushSize(f, line),
                "IsCanvasColor" => EvalIsCanvasColor(f, line),
                _ => throw new FunctionNotImplementedError(name, line)
            };
        }

        /// <summary>
        /// Evalúa si el color del pincel es igual al especificado.
        /// </summary>
        private int EvalIsBrushColor(FunctionCallExpr f, int line)
        {
            if (f.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsBrushColor", 1, f.Arguments.Count, line);

            if (f.Arguments[0] is LiteralExpr literal)
            {
                string color = literal.Value.Trim('"').ToLower();
                return color == BrushColor.Name.ToLower() ? 1 : 0;
            }

            throw new InvalidArgumentError("IsBrushColor requires a string literal", line);
        }

        /// <summary>
        /// Evalúa si el tamaño del pincel es igual al especificado.
        /// </summary>
        private int EvalIsBrushSize(FunctionCallExpr f, int line)
        {
            if (f.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsBrushSize", 1, f.Arguments.Count, line);

            int value = EvaluateExpression(f.Arguments[0], line);
            return value == BrushSize ? 1 : 0;
        }

        /// <summary>
        /// Evalúa si el color del canvas es igual al especificado.
        /// </summary>
        private int EvalIsCanvasColor(FunctionCallExpr f, int line)
        {
            if (f.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsCanvasColor", 1, f.Arguments.Count, line);

            if (f.Arguments[0] is LiteralExpr literal)
            {
                string color = literal.Value.Trim('"').ToLower();
                return color == Canvas.BackColor.Name.ToLower() ? 1 : 0;
            }

            throw new InvalidArgumentError("IsCanvasColor requires a string literal", line);
        }

        #endregion

        #region Variable Helpers

        /// <summary>
        /// Asigna un valor a una variable (o la crea si no existe).
        /// </summary>
        /// <param name="name">Nombre de la variable.</param>
        /// <param name="value">Valor a asignar.</param>
        /// <param name="line">Línea de código para contexto de error.</param>
        public void AssignVariable(string name, int value, int line)
        {
            _variables[name] = value;
            Console.WriteLine($"[Line {line}] {name} = {value}");
        }

        #endregion

        #region Brush and Canvas Helpers

        /// <summary>
        /// Cambia el color del pincel y lo actualiza en el canvas.
        /// </summary>
        public void SetBrushColor(Color color)
        {
            BrushColor = color;
            Canvas.SetBrushColor(color);
        }

        /// <summary>
        /// Cambia el tamaño del pincel y lo actualiza en el canvas.
        /// </summary>
        public void SetBrushSize(int size)
        {
            BrushSize = size;
            Canvas.SetBrushSize(size);
        }

        #endregion

        #region Label Map

        /// <summary>
        /// Construye un mapa de etiquetas a sus posiciones en la lista de instrucciones.
        /// </summary>
        private Dictionary<string, int> BuildLabelMap(List<ICode> codes)
        {
            Dictionary<string, int> map = new();

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i] is LabelCommand lbl)
                    map[lbl.Name] = i;
            }

            return map;
        }

        #endregion

        #region Condition Evaluation

        /// <summary>
        /// Evalúa una condición textual (por ejemplo, "x > 5") y retorna 1 (true) o 0 (false).
        /// </summary>
        /// <param name="condition">Condición en texto.</param>
        /// <param name="line">Línea de código para contexto de error.</param>
        public int EvaluateConditionText(string condition, int line)
        {
            try
            {
                string[] ops = { "==", "!=", ">=", "<=", "<", ">" };
                foreach (string op in ops)
                {
                    if (condition.Contains(op))
                    {
                        string[] parts = condition.Split(op);
                        int left = EvaluateTextExpression(parts[0].Trim());
                        int right = EvaluateTextExpression(parts[1].Trim());

                        return op switch
                        {
                            "==" => left == right ? 1 : 0,
                            "!=" => left != right ? 1 : 0,
                            ">" => left > right ? 1 : 0,
                            "<" => left < right ? 1 : 0,
                            ">=" => left >= right ? 1 : 0,
                            "<=" => left <= right ? 1 : 0,
                            _ => 0
                        };
                    }
                }

                // Si no hay operador, evalúa como expresión simple
                return EvaluateTextExpression(condition.Trim());
            }
            catch
            {
                throw new InvalidCommandError("Goto", "Invalid condition: " + condition, line);
            }
        }

        /// <summary>
        /// Evalúa una expresión textual (puede ser número o variable).
        /// </summary>
        private int EvaluateTextExpression(string text)
        {
            if (int.TryParse(text, out int number))
                return number;

            return _variables.TryGetValue(text, out int val) ? val : 0;
        }

        #endregion

    }
}