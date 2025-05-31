using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wall_E
{
    public class Executor
    {
        #region Fields

        private readonly Dictionary<string, int> _variables = new();
        public PixelCanvas Canvas { get; }
        public Color BrushColor { get; private set; } = Color.Black;
        public int BrushSize { get; private set; } = 1;

        #endregion

        #region Constructor

        public Executor(PixelCanvas canvas)
        {
            Canvas = canvas;
        }

        #endregion

        #region Execute Method

        public void Execute(List<ICode> codes)
        {
            var labelMap = BuildLabelMap(codes);

            for (int i = 0; i < codes.Count; i++)
            {
                ICode code = codes[i];

                try
                {
                    if (code is GotoCommand gotoCmd)
                    {
                        if (gotoCmd.ShouldJump(this))
                        {
                            if (labelMap.TryGetValue(gotoCmd.TargetLabel, out int index))
                            {
                                i = index - 1;
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
                        code.Execute(this);
                    }
                }
                catch (RuntimeError e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected Error: {e.Message}");
                }
            }
        }

        #endregion

        #region Expression Evaluation

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

        private int ParseLiteral(string value, int line)
        {
            if (int.TryParse(value, out int number))
                return number;

            throw new InvalidLiteralError(value, line);
        }

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

        private int EvalIsBrushSize(FunctionCallExpr f, int line)
        {
            if (f.Arguments.Count != 1)
                throw new InvalidFunctionArityError("IsBrushSize", 1, f.Arguments.Count, line);

            int value = EvaluateExpression(f.Arguments[0], line);
            return value == BrushSize ? 1 : 0;
        }

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

        public void AssignVariable(string name, int value, int line)
        {
            _variables[name] = value;
            Console.WriteLine($"[Line {line}] {name} = {value}");
        }

        #endregion

        #region Brush and Canvas Helpers

        public void SetBrushColor(Color color)
        {
            BrushColor = color;
            Canvas.SetBrushColor(color);
        }

        public void SetBrushSize(int size)
        {
            BrushSize = size;
            Canvas.SetBrushSize(size);
        }

        #endregion

        #region Label Map

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

                return EvaluateTextExpression(condition.Trim());
            }
            catch
            {
                throw new InvalidCommandError("Goto", "Invalid condition: " + condition, line);
            }
        }

        private int EvaluateTextExpression(string text)
        {
            if (int.TryParse(text, out int number))
                return number;

            return _variables.TryGetValue(text, out int val) ? val : 0;
        }

    }
}
