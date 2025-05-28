using System;

namespace Wall_E
{
    #region "Enumeración de Tipos de Tokens"

    /// <summary>
    /// Enumeración que define todos los tipos de tokens que puede reconocer el lexer.
    /// Cada valor representa un tipo de palabra clave, operador, literal, símbolo, etc.
    /// </summary>
    public enum TokenType
    {
        #region "Palabras clave: comandos principales"

        SPAWN,          // Comando para crear un objeto o entidad
        COLOR,          // Comando para cambiar el color
        SIZE,           // Comando para cambiar el tamaño
        DRAW_LINE,      // Comando para dibujar una línea
        DRAW_CIRCLE,    // Comando para dibujar un círculo
        DRAW_RECTANGLE, // Comando para dibujar un rectángulo
        FILL,           // Comando para rellenar una figura

        #endregion

        #region "Funciones: operaciones sobre el canvas o pincel"

        GET_ACTUAL_X,       // Obtener la posición X actual
        GET_ACTUAL_Y,       // Obtener la posición Y actual
        GET_CANVAS_SIZE,    // Obtener el tamaño del canvas
        GET_COLOR_COUNT,    // Obtener la cantidad de colores
        IS_BRUSH_COLOR,     // Verificar si el color del pincel es un color específico
        IS_BRUSH_SIZE,      // Verificar si el tamaño del pincel es un tamaño específico
        IS_CANVAS_COLOR,    // Verificar si el color del canvas es un color específico

        #endregion

        #region "Operadores matemáticos y de asignación"

        ASSIGN,         // <-  Operador de asignación
        PLUS,           // +   Suma
        MINUS,          // -   Resta
        MULTIPLY,       // *   Multiplicación
        DIVIDE,         // /   División
        POWER,          // **  Potencia
        MOD,            // %   Módulo

        #endregion

        #region "Operadores de comparación"

        EQUAL,          // ==  Igualdad
        GREATER_EQUAL,  // >=  Mayor o igual
        LESS_EQUAL,     // <=  Menor o igual
        GREATER,        // >   Mayor que
        LESS,           // <   Menor que

        #endregion

        #region "Operadores booleanos"

        AND,            // &&  Y lógico
        OR,             // ||  O lógico

        #endregion

        #region "Estructuras de control"

        GOTO,           // Salto a una etiqueta
        LABEL_DEF,      // Definición de una etiqueta

        #endregion

        #region "Literales y tipos"

        NUMBER,         // Número (entero o decimal)
        STRING,         // Cadena de texto
        BOOLEAN,        // Valor booleano (true/false)
        IDENTIFIER,     // Identificador (nombre de variable, función, etc.)

        #endregion

        #region "Símbolos de agrupación y puntuación"

        LPAREN,         // (   Paréntesis de apertura
        RPAREN,         // )   Paréntesis de cierre
        LBRACKET,       // [   Corchete de apertura
        RBRACKET,       // ]   Corchete de cierre
        COMMA,          // ,   Coma
        NEWLINE,        //     Salto de línea

        #endregion

        #region "Final y errores"

        EOF,            // Fin de archivo/código
        UNKNOWN         // Token no reconocido (error léxico)

        #endregion
    }

    #endregion
}