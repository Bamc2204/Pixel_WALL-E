using System;
using System.Collections.Generic;

namespace Wall_E
{
    // Clase encargada de analizar una lista de tokens y convertirlos en comandos ejecutables
    public class Parser
    {
        #region "Campos y Constructor"

        // Lista de tokens generados por el lexer
        private readonly List<Token> _tokens;
        // Índice del token actual que se está procesando
        private int _current = 0;

        // Constructor: recibe la lista de tokens a analizar
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        #endregion

        #region "Método principal de análisis"

        // Método principal: recorre todos los tokens y construye una lista de comandos válidos
        public List<Comando> Parse()
        {
            List<Comando> comandos = new();

            // Mientras no se llegue al final de los tokens
            while (!IsAtEnd())
            {
                if (Peek().Type == TokenType.NEWLINE)
                {
                    Advance(); // Ignora líneas vacías
                    continue;
                }

                Comando cmd = ParseComando();
                if (cmd != null)
                {
                    comandos.Add(cmd);
                }
                else
                {
                    Advance(); // Evita bucle infinito
                }
            }

            // Devuelve la lista de comandos reconocidos
            return comandos;
        }

        #endregion

        #region "Reconocimiento de comandos"

        // Reconoce el tipo de comando según el token actual
        private Comando? ParseComando()
        {
            Token token = Peek(); // Obtiene el token actual sin avanzar

            switch (token.Type)
            {
                case TokenType.DRAW_LINE: return ParseDrawLine(); // Si es DrawLine, llama a su parser
                case TokenType.SPAWN: return ParseSpawn();         // Si es Spawn, llama a su parser
                case TokenType.COLOR: return ParseColor();         // Si es Color, llama a su parser
                case TokenType.SIZE: return ParseSize();           // Si es Size, llama a su parser
                case TokenType.LABEL_DEF: return ParseEtiqueta();
                case TokenType.GOTO: return ParseGoto();
                default:
                    // Si el comando no es reconocido, muestra un mensaje de error
                    Console.WriteLine($"[Línea {token.Line}] Comando no reconocido: {token.Lexeme}");
                    return null;
            }
        }

        #endregion

        #region ParseEtiqueta

        private Comando? ParseEtiqueta()
        {
            Token token = Advance();
            return new LabelCommand
            {
                Nombre = token.Lexeme,
                Linea = token.Line
            };
        }

        #endregion

        #region ParseGoto

        private Comando? ParseGoto()
        {
            Token gotoToken = Advance(); // consume GOTO
            int linea = gotoToken.Line;

            if (!Match(TokenType.LBRACKET))
                return Error("Falta '[' después de Goto");

            if (!Match(TokenType.IDENTIFIER) && !Match(TokenType.LABEL_DEF))
                return Error("Se esperaba el nombre de la etiqueta entre corchetes");

            string nombreEtiqueta = Previous().Lexeme;

            if (!Match(TokenType.RBRACKET))
                return Error("Falta ']' después del nombre de la etiqueta");

            if (!Match(TokenType.LPAREN))
                return Error("Falta '(' con la condición del Goto");

            // Leer condición como texto crudo (hasta cerrar el paréntesis)
            string condicion = "";

            while (!IsAtEnd() && !Check(TokenType.RPAREN))
            {
                condicion += Advance().Lexeme + " ";
            }

            if (!Match(TokenType.RPAREN))
                return Error("Falta ')' al final de la condición del Goto");

            return new GotoCommand
            {
                EtiquetaDestino = nombreEtiqueta,
                CondicionTexto = condicion.Trim(),
                Linea = linea
            };
        }

        #endregion

        #region "Parsers específicos para cada comando"

        // ---------------------
        // Comando: DrawLine(x, y, d)
        // ---------------------
        private Comando? ParseDrawLine()
        {
            Token token = Advance(); // Consume el token DRAW_LINE
            int linea = token.Line;  // Guarda el número de línea para mensajes de error

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de DrawLine");

            int? dirX = ParseNumero(); // Primer parámetro: dirección X
            if (dirX == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? dirY = ParseNumero(); // Segundo parámetro: dirección Y
            if (dirY == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de segundo número");

            int? dist = ParseNumero(); // Tercer parámetro: distancia
            if (dist == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de DrawLine");

            // Si todo fue bien, crea y devuelve el comando DrawLineCommand
            return new DrawLineCommand
            {
                DirX = dirX.Value,
                DirY = dirY.Value,
                Distance = dist.Value,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Spawn(x, y)
        // ---------------------
        private Comando? ParseSpawn()
        {
            Token token = Advance(); // Consume el token SPAWN
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Spawn");

            int? x = ParseNumero(); // Primer parámetro: X
            if (x == null) return null;
            if (!Match(TokenType.COMMA)) return Error("Falta ',' después de primer número");

            int? y = ParseNumero(); // Segundo parámetro: Y
            if (y == null) return null;
            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Spawn");

            // Si todo fue bien, crea y devuelve el comando SpawnCommand
            return new SpawnCommand
            {
                X = x.Value,
                Y = y.Value,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Color("nombre")
        // ---------------------
        private Comando? ParseColor()
        {
            Token token = Advance(); // Consume el token COLOR
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Color");

            // Espera un string como parámetro
            if (!Match(TokenType.STRING))
            {
                Console.WriteLine($"[Línea {Peek().Line}] Se esperaba un string con el nombre del color");
                return null;
            }

            string color = Previous().Lexeme; // Obtiene el nombre del color

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Color");

            // Si todo fue bien, crea y devuelve el comando ColorCommand
            return new ColorCommand
            {
                NombreColor = color,
                Linea = linea
            };
        }

        // ---------------------
        // Comando: Size(n)
        // ---------------------
        private Comando? ParseSize()
        {
            Token token = Advance(); // Consume el token SIZE
            int linea = token.Line;

            if (!Match(TokenType.LPAREN)) return Error("Falta '(' después de Size");

            int? valor = ParseNumero(); // Espera un número como parámetro
            if (valor == null) return null;

            if (!Match(TokenType.RPAREN)) return Error("Falta ')' al final de Size");

            // Si todo fue bien, crea y devuelve el comando SizeCommand
            return new SizeCommand
            {
                Valor = valor.Value,
                Linea = linea
            };
        }

        #endregion

        #region "Utilidades y métodos auxiliares"

        // Intenta analizar un número, si el token actual es un número lo consume y lo devuelve
        private int? ParseNumero()
        {
            if (Match(TokenType.NUMBER))
            {
                return int.Parse(Previous().Lexeme);
            }
            Console.WriteLine($"[Línea {Peek().Line}] Se esperaba un número");
            return null;
        }

        // Si el token actual es del tipo esperado, lo consume y devuelve true; si no, devuelve false
        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        // Verifica si el token actual es del tipo esperado sin consumirlo
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        // Avanza al siguiente token y devuelve el token anterior
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        // Verifica si se llegó al final de la lista de tokens
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        // Devuelve el token actual sin avanzar
        private Token Peek() => _tokens[_current];

        // Devuelve el token anterior al actual
        private Token Previous() => _tokens[_current - 1];

        // Muestra un mensaje de error y devuelve null
        private Comando? Error(string mensaje)
        {
            Console.WriteLine($"[Línea {Peek().Line}] Error de sintaxis: {mensaje}");
            return null;
        }

        #endregion
    }
}