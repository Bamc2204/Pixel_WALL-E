using System;
using System.Collections.Generic;
using Wall_E;

class Program
{
    static void Main()
    {
        Console.Clear();
        // Código de prueba con asignaciones y funciones
        string codigo = @"x <- 5
        y <- x
        z <- x + 3
        is_blue <- IsBrushColor(""Blue"")";

        // Analiza el código fuente
        CodeAnalyzer analizador = new CodeAnalyzer(codigo);
        List<Token> tokens = analizador.ObtenerTokens();

        // Muestra tokens para depuración
        Console.WriteLine("=== TOKENS ===");
        foreach (var t in tokens)
            Console.WriteLine(t);

        // Parseo del código
        Parser parser = new Parser(tokens);
        List<Code> codigos = parser.Parse(); // Usa Code si ya cambiaste el tipo base, o Comando si no

        Console.WriteLine("\n=== CÓDIGOS PARSEADOS ===");
        foreach (var c in codigos)
            Console.WriteLine(c);
    }
}
