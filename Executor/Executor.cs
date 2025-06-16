using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wall_E
{
    /// <summary>
    /// Clase encargada de ejecutar la lista de comandos (ICode) y mantener el estado de las variables y el canvas.
    /// Gestiona la ejecución secuencial, saltos condicionales, evaluación de expresiones y el estado del pincel.
    /// </summary>
    public class Executor
    {
        #region PublicProperties

        /// <summary>
        /// Canvas de píxeles donde se dibuja.
        /// </summary>
        public PixelCanvas Canvas { get; }

        /// <summary>
        /// Color actual del pincel.
        /// </summary>
        public Color BrushColor { get; private set; } = Color.Transparent;

        /// <summary>
        /// Tamaño actual del pincel.
        /// </summary>
        public int BrushSize { get; private set; } = 1;

        #endregion

        #region PrivateFields

        // Diccionario de variables del programa.
        private readonly Dictionary<string, int> _variables = new();
        // Lista de errores de ejecución.
        private readonly List<string> _runtimeErrors;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el ejecutor con el canvas y la lista de errores.
        /// </summary>
        public Executor(PixelCanvas canvas, List<string> runtimeErrors)
        {
            Canvas = canvas;
            _runtimeErrors = runtimeErrors;
        }

        #endregion

        #region ExecuteEntryPoint

        /// <summary>
        /// Ejecuta la lista de comandos recibida.
        /// Controla saltos de etiquetas y errores de ejecución.
        /// </summary>
        /// <param name="codes">Lista de comandos a ejecutar.</param>
        public void Execute(List<ICode> codes)
        {
            // Mapa de etiquetas para saltos rápidos.
            Dictionary<string, int> labelMap = BuildLabelMap(codes);

            for (int i = 0; i < codes.Count; i++)
            {
                ICode code = codes[i];
                try
                {
                    // Si es un comando Goto, evalúa la condición y salta si corresponde.
                    if (code is GotoCommand jump)
                    {
                        bool condition = EvaluateCondition(jump.Condition);
                        if (condition)
                        {
                            if (labelMap.TryGetValue(jump.TargetLabel, out int destination))
                            {
                                i = destination - 1; // Saltar a la etiqueta (ajustar por el for)
                                continue;
                            }
                            else
                            {
                                throw new LabelNotFoundError(jump.TargetLabel, $"Etiqueta no encontrada: {jump.TargetLabel}", code.Line);
                            }
                        }
                    }
                    else
                    {
                        // Ejecuta el comando normalmente.
                        code.Execute(this);
                    }
                }
                catch (RuntimeError ex)
                {
                    _runtimeErrors.Add($"[Runtime Error] Línea {ex.Line}: {ex.Message}");
                }
            }
        }

        #endregion

        #region LabelMapping

        /// <summary>
        /// Construye un diccionario con las etiquetas y su posición en la lista de comandos.
        /// Permite saltos eficientes a etiquetas durante la ejecución.
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

        #region PublicAPI_Evaluation

        /// <summary>
        /// Evalúa una expresión y devuelve su valor entero.
        /// Soporta literales, variables, operaciones binarias y llamadas a funciones especiales.
        /// </summary>
        public object EvaluateExpression(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr l:
                    // Si es un número, retorna el número
                    if (int.TryParse(l.Value, out int value))
                        return value;
                    // Si empieza y termina con comillas, es un string: quita las comillas
                    if (l.Value.StartsWith("\"") && l.Value.EndsWith("\""))
                        return l.Value.Substring(1, l.Value.Length - 2);
                    throw new InvalidArgumentError($"Literal inválido: {l.Value}", "No se pudo convertir a número o string", expr.Line);

                case VariableExpr v:
                    if (_variables.TryGetValue(v.Name, out int val))
                        return val;
                    throw new UndefinedVariableError($"Variable no definida: {v.Name}", "Debe declarar la variable antes de usarla", expr.Line);

                case BinaryExpr b:
                    int left = Convert.ToInt32(EvaluateExpression(b.Left));
                    int right = Convert.ToInt32(EvaluateExpression(b.Right));

                    return b.Operator switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right != 0 ? left / right : throw new DivisionByZeroError(
                                    "División por cero", "El divisor es cero", expr.Line),
                        "%" => left % right,
                        "**" => (int)Math.Pow(left, right),
                        "==" => left == right ? 1 : 0,
                        "!=" => left != right ? 1 : 0,
                        "<" => left < right ? 1 : 0,
                        "<=" => left <= right ? 1 : 0,
                        ">" => left > right ? 1 : 0,
                        ">=" => left >= right ? 1 : 0,

                        _ => throw new InvalidArgumentError(
                            $"Operador desconocido: {b.Operator}", "No se reconoce el operador", expr.Line)
                    };


                case FunctionCallExpr f:
                    return EvaluateFunction(f);

                default:
                    throw new EmptyExpressionError("Expresión inválida", expr.Line);
            }
        }

        /// <summary>
        /// Evalúa una función especial del lenguaje.
        /// </summary>
        private int EvaluateFunction(FunctionCallExpr f)
        {
            switch (f.FunctionName)
            {
                case "GetActualX":
                    return Canvas.GetCursorX();

                case "GetActualY":
                    return Canvas.GetCursorY();

                case "GetCanvasSize":
                    return Canvas.Cols * Canvas.Rows;

                case "GetColorCount":
                    return Enum.GetNames(typeof(KnownColor)).Length;

                case "IsBrushColor":
                    return f.Arguments.Count == 1 && f.Arguments[0] is LiteralExpr litColor &&
                           BrushColor.Name.Equals(litColor.Value.Trim('"'), StringComparison.OrdinalIgnoreCase)
                           ? 1 : 0;

                case "IsBrushSize":
                    return f.Arguments.Count == 1 && f.Arguments[0] is LiteralExpr litSize &&
                           int.TryParse(litSize.Value, out int s) && s == BrushSize ? 1 : 0;

                case "IsCanvasColor":
                    return f.Arguments.Count == 1 && f.Arguments[0] is LiteralExpr lit &&
                           Canvas.GetPixelColor(Canvas.GetCursorX(), Canvas.GetCursorY(), f.Line).Name.Equals(
                               lit.Value.Trim('"'), StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                default:
                    throw new FunctionNotImplementedError(f.FunctionName, f.Line);
            }
        }

        /// <summary>
        /// Evalúa una condición booleana a partir de una expresión.
        /// Considera verdadera si el resultado es distinto de cero.
        /// </summary>
        public bool EvaluateCondition(Expr expr)
        {
             return Convert.ToInt32(EvaluateExpression(expr)) != 0;
        }

        #endregion

        #region PublicAPI_State

        /// <summary>
        /// Asigna un valor a una variable.
        /// </summary>
        public void AssignVariable(string name, int value, int line)
        {
            _variables[name] = value;
        }

        /// <summary>
        /// Cambia el color actual del pincel.
        /// </summary>
        public void SetBrushColor(Color color)
        {
            BrushColor = color;
        }

        /// <summary>
        /// Cambia el tamaño actual del pincel.
        /// </summary>
        public void SetBrushSize(int size)
        {
            BrushSize = size;
        }

        #endregion
    }
}