using System.Collections.Generic;

namespace Wall_E
{
    #region ParserExpressions

    #region BaseExpressionClass

    /// <summary>
    /// Clase base abstracta para cualquier tipo de expresión (literal, variable, operación, función...).
    /// Todas las expresiones del parser deben heredar de esta clase.
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// Línea del código fuente donde aparece la expresión (útil para errores).
        /// </summary>
        public int Line { get; set; }
    }

    #endregion

    #region LiteralExpression

    /// <summary>
    /// Representa un valor literal, como un número o una cadena.
    /// Ejemplo: 5, "hola"
    /// </summary>
    public class LiteralExpr : Expr
    {
        /// <summary>
        /// Valor literal de la expresión (como string).
        /// </summary>
        public string Value { get; set; }
    }

    #endregion

    #region VariableExpression

    /// <summary>
    /// Representa el uso de una variable, como x, y, total, etc.
    /// </summary>
    public class VariableExpr : Expr
    {
        /// <summary>
        /// Nombre de la variable.
        /// </summary>
        public string Name { get; set; }
    }

    #endregion

    #region BinaryExpression

    /// <summary>
    /// Representa una operación binaria, como suma, resta, multiplicación, etc.
    /// Ejemplo: (x + 2), (a * b)
    /// </summary>
    public class BinaryExpr : Expr
    {
        /// <summary>
        /// Operador de la expresión ("+", "-", "*", "/", etc.).
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// Expresion izquierdo (numero, booleano, variable, etc.).
        /// </summary>
        public Expr Left { get; set; }
        /// <summary>
        /// Expresión derecho (numero, booleano, variable, etc.).
        /// </summary>
        public Expr Right { get; set; }
    }

    #endregion

    #region UnaryExpression
        
    /// <summary>
    /// Representa una operación unaria, como negación, factorial, etc.
    /// Ejemplo: (-x), (x!)
    /// </summary>
    public class UnaryExpr : Expr
    {
        /// <summary>
        /// Operador de la expresión ("-", "+", "!", etc.).
        /// </summary>
        public Token Operator { get; set; }
        /// <summary>
        /// Expresión a la que se aplica la operación.
        /// </summary>
        public Expr Right { get; set; }
    }


    #endregion

    #region FunctionCallExpression

    /// <summary>
    /// Representa una llamada a función, como GetActualX(), IsBrushColor("Blue"), etc.
    /// </summary>
    public class FunctionCallExpr : Expr
    {
        /// <summary>
        /// Nombre de la función a llamar.
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// Lista de argumentos de la función.
        /// </summary>
        public List<Expr> Arguments { get; set; } = new();
    }

    #endregion

    #endregion
}