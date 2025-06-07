using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wall_E
{
    public class Executor
    {
        #region PrivateFields

        // Diccionario para almacenar variables y sus valores.
        private readonly Dictionary<string, int> _variables = new();

        // Referencia al canvas donde se dibuja.
        private readonly PixelCanvas _canvas;

        #endregion

        #region PublicProperties

        /// <summary>
        /// Permite acceder al canvas actual.
        /// </summary>
        public PixelCanvas Canvas => _canvas;

        /// <summary>
        /// Permite obtener el color actual del pincel.
        /// </summary>
        public Color BrushColor => _canvas.GetCurrentColor();

        /// <summary>
        /// Permite obtener el tamaño actual del pincel.
        /// </summary>
        public int BrushSize => _canvas.GetBrushSize();

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el ejecutor con un canvas dado.
        /// </summary>
        /// <param name="canvas">Canvas sobre el que se dibuja.</param>
        public Executor(PixelCanvas canvas)
        {
            _canvas = canvas;
        }

        #endregion

        #region MainExecutionMethod

        /// <summary>
        /// Método principal que ejecuta una lista de instrucciones.
        /// </summary>
        /// <param name="codes">Lista de instrucciones a ejecutar.</param>
        public void Execute(List<ICode> codes)
        {
            // Mapa de etiquetas para saltos.
            Dictionary<string, int> labels = BuildLabelMap(codes);

            for (int i = 0; i < codes.Count; i++)
            {
                ICode code = codes[i];

                try
                {
                    // Si es un comando de salto (goto)
                    if (code is GotoCommand gotoCmd)
                    {
                        int cond = EvaluateConditionText(gotoCmd.ConditionText, code.Line);
                        if (cond != 0)
                        {
                            if (!labels.TryGetValue(gotoCmd.TargetLabel, out int dest))
                                throw new LabelNotFoundError(gotoCmd.TargetLabel, code.Line);

                            i = dest - 1; // Saltar a la etiqueta
                            continue;
                        }
                    }
                    else
                    {
                        // Ejecutar instrucción normal
                        code.Execute(this);
                    }
                }
                catch (RuntimeError ex)
                {
                    // Captura y muestra errores de ejecución
                    Console.WriteLine($"[Runtime Error - Línea {ex.Line}] {ex.Message}");
                }
            }
        }

        #endregion

        #region ExpressionEvaluation

        /// <summary>
        /// Evalúa una expresión y devuelve su valor entero.
        /// </summary>
        /// <param name="expr">Expresión a evaluar.</param>
        /// <returns>Valor entero de la expresión.</returns>
        public int EvaluateExpression(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr lit:
                    if (int.TryParse(lit.Value, out int val))
                        return val;
                    throw new InvalidLiteralError(lit.Value, expr.Line);

                case VariableExpr var:
                    if (_variables.TryGetValue(var.Name, out int value))
                        return value;
                    throw new UndefinedVariableError(var.Name, expr.Line);

                case BinaryExpr bin:
                    int left = EvaluateExpression(bin.Left);
                    int right = EvaluateExpression(bin.Right);
                    return bin.Operator switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right == 0 ? throw new DivisionByZeroError(expr.Line) : left / right,
                        "%" => left % right,
                        "**" => (int)Math.Pow(left, right),
                        _ => throw new UnknownOperatorError(bin.Operator, expr.Line)
                    };

                case FunctionCallExpr fn:
                    return fn.FunctionName switch
                    {
                        "GetActualX" => _canvas.GetCursorX(),
                        "GetActualY" => _canvas.GetCursorY(),
                        "GetCanvasSize" => _canvas.Cols * _canvas.Rows,
                        "GetColorCount" => Enum.GetNames(typeof(KnownColor)).Length,
                        "IsBrushColor" => IsBrushColor(fn, expr.Line),
                        "IsBrushSize" => IsBrushSize(fn, expr.Line),
                        "IsCanvasColor" => IsCanvasColor(fn, expr.Line),
                        _ => throw new FunctionNotImplementedError(fn.FunctionName, expr.Line)
                    };

                default:
                    throw new EmptyExpressionError("expresión", expr.Line);
            }
        }

        #endregion

        #region PublicHelperMethods

        /// <summary>
        /// Evalúa una condición en formato texto y devuelve 1 (true) o 0 (false).
        /// </summary>
        /// <param name="text">Condición en texto.</param>
        /// <param name="line">Línea de código.</param>
        /// <returns>1 si la condición es verdadera, 0 si es falsa.</returns>
        public int EvaluateConditionText(string text, int line)
        {
            try
            {
                string[] ops = { "==", "!=", ">=", "<=", "<", ">" };
                foreach (string op in ops)
                {
                    if (text.Contains(op))
                    {
                        string[] parts = text.Split(op);
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

                return EvaluateTextExpression(text);
            }
            catch
            {
                throw new EmptyExpressionError("condición", line);
            }
        }

        /// <summary>
        /// Asigna un valor a una variable.
        /// </summary>
        /// <param name="name">Nombre de la variable.</param>
        /// <param name="value">Valor a asignar.</param>
        public void AssignVariable(string name, int value) => _variables[name] = value;

        /// <summary>
        /// Cambia el color actual del pincel.
        /// </summary>
        /// <param name="color">Color a establecer.</param>
        public void SetBrushColor(Color color) => _canvas.SetColor(color);

        #endregion

        #region PrivateHelperMethods

        /// <summary>
        /// Evalúa una expresión en texto (literal o variable).
        /// </summary>
        /// <param name="txt">Texto a evaluar.</param>
        /// <returns>Valor entero correspondiente.</returns>
        private int EvaluateTextExpression(string txt)
        {
            if (int.TryParse(txt, out int val))
                return val;

            if (_variables.TryGetValue(txt, out int result))
                return result;

            return 0;
        }

        /// <summary>
        /// Verifica si el color actual del pincel coincide con el argumento.
        /// </summary>
        private int IsBrushColor(FunctionCallExpr fn, int line)
        {
            if (fn.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsBrushColor", 1, fn.Arguments.Count, line);

            Expr arg = fn.Arguments[0];
            if (arg is not LiteralExpr lit || !lit.Value.StartsWith("\""))
                throw new InvalidArgumentError("El argumento debe ser una cadena entre comillas", line);

            string name = lit.Value.Trim('"');
            return string.Equals(name, _canvas.GetCurrentColor().Name, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        }

        /// <summary>
        /// Verifica si el tamaño del pincel coincide con el argumento.
        /// </summary>
        private int IsBrushSize(FunctionCallExpr fn, int line)
        {
            if (fn.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsBrushSize", 1, fn.Arguments.Count, line);

            Expr arg = fn.Arguments[0];
            if (arg is not LiteralExpr lit || !int.TryParse(lit.Value, out int size))
                throw new InvalidArgumentError("El argumento debe ser un número entero", line);

            return size == _canvas.GetBrushSize() ? 1 : 0;
        }

        /// <summary>
        /// Verifica si el color del canvas coincide con el argumento.
        /// </summary>
        private int IsCanvasColor(FunctionCallExpr fn, int line)
        {
            if (fn.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsCanvasColor", 1, fn.Arguments.Count, line);

            Expr arg = fn.Arguments[0];
            if (arg is not LiteralExpr lit || !lit.Value.StartsWith("\""))
                throw new InvalidArgumentError("El argumento debe ser una cadena entre comillas", line);

            string colorName = lit.Value.Trim('"');
            return _canvas.GetPixelColor(0, 0, line).Name.Equals(colorName, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        }

        /// <summary>
        /// Construye un mapa de etiquetas para saltos rápidos en el código.
        /// </summary>
        /// <param name="codes">Lista de instrucciones.</param>
        /// <returns>Diccionario de etiquetas y su posición.</returns>
        private Dictionary<string, int> BuildLabelMap(List<ICode> codes)
        {
            Dictionary<string, int> map = new();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i] is LabelCommand lbl)
                {
                    map[lbl.Name] = i;
                }
            }
            return map;
        }

        #endregion
    }
}