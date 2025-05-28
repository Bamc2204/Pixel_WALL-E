#region Using
using System;
using System.Collections.Generic;
#endregion


namespace Wall_E
{
    #region Executor Class

    /// <summary>
    /// Clase principal encargada de ejecutar una lista de comandos de código.
    /// Gestiona variables, saltos, etiquetas y evaluación de expresiones.
    /// </summary>
    public class Executor
    {
        #region Fields

        // Diccionario para almacenar el valor de las variables durante la ejecución
        private Dictionary<string, int> _variables = new();

        #endregion

        #region Execute Method

        /// <summary>
        /// Ejecuta una lista de comandos de código.
        /// Controla el flujo, maneja saltos (goto), evalúa condiciones y ejecuta asignaciones.
        /// </summary>
        /// <param name="codes">Lista de comandos a ejecutar.</param>
        public void Execute(List<Code> codes)
        {
            // Mapea las etiquetas a sus posiciones en la lista de comandos
            Dictionary<string, int> labelMap = BuildLabelMap(codes);

            for (int i = 0; i < codes.Count; i++)
            {
                Code code = codes[i];

                // Si el comando es un salto (goto), evalúa la condición y salta si corresponde
                if (code is GotoCommand jump)
                {
                    int condition = EvaluateConditionText(jump.ConditionText);
                    if (condition != 0)
                    {
                        if (labelMap.TryGetValue(jump.TargetLabel, out int destination))
                        {
                            i = destination - 1; // Salta a la etiqueta correspondiente
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"[Line {code.Line}] Label '{jump.TargetLabel}' not found.");
                        }
                    }
                }
                else
                {
                    // Ejecuta cualquier otro tipo de comando
                    ExecuteCode(code);
                }
            }
        }

        #endregion

        #region Execute Code

        /// <summary>
        /// Ejecuta un solo comando de código según su tipo.
        /// </summary>
        /// <param name="code">Comando a ejecutar.</param>
        private void ExecuteCode(Code code)
        {
            switch (code)
            {
                case AssignmentCommand assignment:
                    // Evalúa la expresión y asigna el valor a la variable
                    int value = EvaluateExpression(assignment.Expression);
                    _variables[assignment.VariableName] = value;
                    Console.WriteLine($"[Line {code.Line}] {assignment.VariableName} = {value}");
                    break;

                case LabelCommand:
                    // Las etiquetas no se ejecutan, solo marcan posición
                    break;

                case GotoCommand:
                    // Los saltos se manejan en el método Execute
                    break;

                default:
                    Console.WriteLine($"[Line {code.Line}] Command not implemented.");
                    break;
            }
        }

        #endregion

        #region Evaluate Expression

        /// <summary>
        /// Evalúa una expresión y devuelve su valor entero.
        /// Soporta literales, variables, operaciones binarias y llamadas a funciones.
        /// </summary>
        /// <param name="expr">Expresión a evaluar.</param>
        /// <returns>Valor entero de la expresión.</returns>
        private int EvaluateExpression(Expr expr)
        {
            switch (expr)
            {
                case LiteralExpr l:
                    return int.Parse(l.Value);

                case VariableExpr v:
                    if (_variables.TryGetValue(v.Name, out int val))
                        return val;
                    throw new Exception($"Undefined variable: {v.Name}");

                case BinaryExpr b:
                    int left = EvaluateExpression(b.Left);
                    int right = EvaluateExpression(b.Right);
                    return b.Operator switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => right != 0 ? left / right : throw new DivideByZeroException(),
                        "%" => left % right,
                        "**" => (int)Math.Pow(left, right),
                        _ => throw new Exception($"Unknown operator: {b.Operator}")
                    };

                case FunctionCallExpr f:
                    // Llama a funciones internas simuladas
                    return f.FunctionName switch
                    {
                        "GetActualX" => 10,
                        "GetActualY" => 20,
                        "GetCanvasSize" => 100,
                        "GetColorCount" => 5,
                        "IsBrushColor" => EvaluateIsBrushColor(f),
                        "IsBrushSize" => EvaluateIsBrushSize(f),
                        "IsCanvasColor" => EvaluateIsCanvasColor(f),
                        _ => throw new Exception($"Function not implemented: {f.FunctionName}")
                    };

                default:
                    throw new Exception("Unknown expression");
            }
        }

        #endregion

        #region Evaluate Built-in Functions

        /// <summary>
        /// Evalúa la función IsBrushColor, retorna 1 si el color es "blue", 0 en otro caso.
        /// </summary>
        private int EvaluateIsBrushColor(FunctionCallExpr f)
        {
            if (f.Arguments.Count != 1)
                throw new Exception("IsBrushColor requires 1 argument");

            Expr arg = f.Arguments[0];
            if (arg is LiteralExpr literal && literal.Value.StartsWith("\""))
            {
                string color = literal.Value.Trim('"').ToLower();
                return color == "blue" ? 1 : 0;
            }

            throw new Exception("Invalid argument in IsBrushColor");
        }

        /// <summary>
        /// Evalúa la función IsBrushSize, retorna 1 si el tamaño es 3, 0 en otro caso.
        /// </summary>
        private int EvaluateIsBrushSize(FunctionCallExpr f)
        {
            if (f.Arguments.Count != 1)
                throw new Exception("IsBrushSize requires 1 argument");

            Expr arg = f.Arguments[0];
            if (arg is LiteralExpr literal && int.TryParse(literal.Value, out int size))
            {
                return size == 3 ? 1 : 0;
            }

            throw new Exception("Invalid argument in IsBrushSize");
        }

        /// <summary>
        /// Evalúa la función IsCanvasColor, retorna 1 si el color es "white", 0 en otro caso.
        /// </summary>
        private int EvaluateIsCanvasColor(FunctionCallExpr f)
        {
            if (f.Arguments.Count != 1)
                throw new Exception("IsCanvasColor requires 1 argument");

            Expr arg = f.Arguments[0];
            if (arg is LiteralExpr literal && literal.Value.StartsWith("\""))
            {
                string color = literal.Value.Trim('"').ToLower();
                return color == "white" ? 1 : 0;
            }

            throw new Exception("Invalid argument in IsCanvasColor");
        }

        #endregion

        #region Evaluate Goto Conditions

        /// <summary>
        /// Evalúa una condición textual para un salto (goto).
        /// Soporta operadores relacionales y evalúa expresiones simples.
        /// </summary>
        /// <param name="condition">Condición en texto.</param>
        /// <returns>1 si la condición es verdadera, 0 si es falsa.</returns>
        private int EvaluateConditionText(string condition)
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
                return 0;
            }
        }

        /// <summary>
        /// Evalúa una expresión textual simple (número o variable).
        /// </summary>
        private int EvaluateTextExpression(string text)
        {
            if (int.TryParse(text, out int number))
                return number;

            if (_variables.TryGetValue(text, out int val))
                return val;

            return 0;
        }

        #endregion

        #region Build Label Map

        /// <summary>
        /// Construye un diccionario que mapea nombres de etiquetas a sus posiciones en la lista de comandos.
        /// </summary>
        /// <param name="codes">Lista de comandos.</param>
        /// <returns>Diccionario de etiquetas y posiciones.</returns>
        private Dictionary<string, int> BuildLabelMap(List<Code> codes)
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
    #endregion
}
