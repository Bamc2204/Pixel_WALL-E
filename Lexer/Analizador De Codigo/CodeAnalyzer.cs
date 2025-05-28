using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region CodeAnalyzer

    /// <summary>
    /// Clase encargada de analizar el código fuente, obtener los tokens y mostrar resultados.
    /// Puede ser utilizada como puente entre el editor, el lexer y el parser.
    /// </summary>
    public class CodeAnalyzer
    {
        #region PrivateFields

        // Variable privada que almacena el código fuente a analizar
        private readonly string _sourceCode;

        #endregion

        #region Constructor

        /// <summary>
        /// El código fuente se pasa como argumento al constructor.
        /// </summary>
        /// <param name="sourceCode">Código fuente a analizar.</param>
        public CodeAnalyzer(string sourceCode)
        {
            _sourceCode = sourceCode;
        }

        #endregion

        #region TokenRetrieval

        /// <summary>
        /// Método que utiliza el Lexer para obtener la lista de tokens del código fuente.
        /// </summary>
        /// <returns>Lista de tokens generados por el lexer.</returns>
        public List<Token> GetTokens()
        {
            // Crea una instancia del Lexer con el código fuente
            Lexer lexer = new Lexer(_sourceCode);
            // Llama al método Tokenize para obtener la lista de tokens
            return lexer.Tokenize();
        }

        #endregion

        #region ExecutionAndVisualization

        /// <summary>
        /// Método que ejecuta el análisis y muestra los tokens generados por consola.
        /// </summary>
        public void RunCode()
        {
            // Obtiene la lista de tokens usando el método anterior
            List<Token> tokens = GetTokens();

            // Muestra un encabezado en la consola
            Console.WriteLine("=== Tokens generados ===");
            // Recorre y muestra cada token en la consola
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            // Comentario: aquí se puede conectar con el parser y el ejecutor en el futuro
            // Parser parser = new Parser(tokens);
            // var commands = parser.Parse();
            // Executor executor = new Executor(commands);
            // executor.Execute();
        }

        #endregion
    }

    #endregion
}