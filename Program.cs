using System;
using System.Collections.Generic;
using Wall_E;

class Program
{
    static void Main()
    {
        Console.Clear();
        // Código de prueba con asignaciones y funciones
        string codigo = @"i <- 0
        loop_1
        Goto [end] (i >= 3)
        i <- i + 1
        Goto [loop_1] (1 == 1)
        end
        ";

        // Analiza el código fuente
        CodeAnalyzer analizador = new CodeAnalyzer(codigo);
        List<Token> tokens = analizador.GetTokens();

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

        // Ejecutar
        Console.WriteLine("\n=== EJECUCIÓN ===");
        Executor executor = new Executor();
        executor.Execute(codigos);
        
    }
}
