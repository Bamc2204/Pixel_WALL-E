using System;
using System.Collections.Generic;
using Wall_E;

class Program
{
    static void Main()
    {
        string codigo = @"DrawLine(1, 0, 5)";

        AnalizadorDeCodigo analizador = new AnalizadorDeCodigo(codigo);
        List<Token> tokens = analizador.ObtenerTokens();

        Parser parser = new Parser(tokens);
        List<Comando> comandos = parser.Parse();

        Console.WriteLine("=== Comandos reconocidos ===");
        foreach (var cmd in comandos)
        {
            Console.WriteLine(cmd);
        }
    }
}
