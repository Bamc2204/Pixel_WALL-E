using System;
using System.Collections.Generic;
using Wall_E;

class Program
{
    static void Main()
    {
        string codigo = @"Spawn(0, 0)
        Color(""Blue"")
        Size(3)
        DrawLine(1, 0, 5)";

        // Paso 1: Analizar código → tokens
        AnalizadorDeCodigo analizador = new AnalizadorDeCodigo(codigo);
        List<Token> tokens = analizador.ObtenerTokens();

        // 🔍 Paso 2: Mostrar todos los tokens generados
        Console.WriteLine("=== TOKENS GENERADOS POR EL LEXER ===");
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        // Paso 3: Analizar sintácticamente → comandos
        Parser parser = new Parser(tokens);
        List<Comando> comandos = parser.Parse();

        Console.WriteLine("\n=== COMANDOS RECONOCIDOS POR EL PARSER ===");
        foreach (var cmd in comandos)
        {
            Console.WriteLine(cmd);
        }
    }
}
