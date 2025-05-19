using System;
using System.Collections.Generic;

namespace Wall_E
{
    // Clase principal encargada de analizar una lista de tokens y convertirlos en comandos ejecutables
    public class Parser
    {
        // Lista de tokens generados por el lexer
        private readonly List<Token> _tokens;
        // Índice del token actual que se está procesando
        private int _current = 0;

        // Constructor: recibe la lista de tokens a analizar
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        // Método principal: recorre todos los tokens y construye una lista de comandos válidos
        public List<Comando> Parse()
        {
            List<Comando> comandos = new();

            // Mientras no se llegue al final de los tokens
            while (!IsAtEnd())
            {
                // Intenta analizar un comando
                Comando cmd = ParseComando();
                if (cmd != null)
                {
                    // Si el comando es válido, lo agrega a la lista
                    comandos.Add(cmd);
                }
                else
                {
                    // Si no es válido, avanza al siguiente token para evitar bucles infinitos
                    Advance();
                }
            }

            // Devuelve la lista de comandos reconocidos
            return comandos;
        }

        // ------- Método principal para reconocer un comando -------
        // Analiza el token actual y decide qué tipo de comando es
        private Comando? ParseComando()
        {
            Token token = Peek(); // Obtiene el token actual sin avanzar

            switch (token.Type)
            {
                case TokenType.DRAW_LINE:
                    // Si es un comando DrawLine, llama al método específico para analizarlo
                    return ParseDrawLine();

                // Aquí se pueden agregar otros comandos como SPAWN, COLOR, etc.

                default:
                    // Si el comando no es reconocido, muestra un mensaje de error
                    Console.WriteLine($"[Línea {token.Line}] Comando no reconocido: {token.Lexeme}");
                    return null;
            }
        }

        // ------- Método para analizar DrawLine(x, y, d) -------
        // Analiza la sintaxis específica del comando DrawLine
        private Comando? ParseDrawLine()
        {
            Token drawToken = Advance(); // Consume el token DRAW_LINE
            int linea = drawToken.Line;  // Guarda el número de línea para mensajes de error

            // Verifica que el siguiente token sea un paréntesis de apertura '('
            if (!Match(TokenType.LPAREN))
                return Error("Falta '(' después de DrawLine");

            // Primer parámetro: dirX (debe ser un número)
            int? dirX = ParseNumero();
            if (dirX == null) return null;

            // Verifica que haya una coma después del primer número
            if (!Match(TokenType.COMMA))
                return Error("Falta ',' después de primer número");

            // Segundo parámetro: dirY (debe ser un número)
            int? dirY = ParseNumero();
            if (dirY == null) return null;

            // Verifica que haya una coma después del segundo número
            if (!Match(TokenType.COMMA))
                return Error("Falta ',' después de segundo número");

            // Tercer parámetro: distancia (debe ser un número)
            int? dist = ParseNumero();
            if (dist == null) return null;

            // Verifica que el comando termine con un paréntesis de cierre ')'
            if (!Match(TokenType.RPAREN))
                return Error("Falta ')' al final de DrawLine");

            // Si todo fue bien, crea y devuelve el comando DrawLineCommand
            return new DrawLineCommand
            {
                DirX = dirX.Value,
                DirY = dirY.Value,
                Distance = dist.Value,
                Linea = linea
            };
        }

        // ------- Métodos auxiliares -------

        // Intenta analizar un número, si el token actual es un número lo consume y lo devuelve
        private int? ParseNumero()
        {
            if (Match(TokenType.NUMBER))
            {
                Token num = Previous();
                return int.Parse(num.Lexeme);
            }
            // Si no es un número, muestra un mensaje de error
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
    }
}