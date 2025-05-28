using System;
using System.Collections.Generic;

namespace Wall_E
{
    #region "Analizador de Código Fuente"

    /// <summary>
    /// Clase encargada de analizar el código fuente, obtener los tokens y mostrar resultados.
    /// Puede ser utilizada como puente entre el editor, el lexer y el parser.
    /// </summary>
    public class CodeAnalyzer
    {
        #region "Campos privados"

        // Variable privada que almacena el código fuente a analizar
        private readonly string _codigo;

        #endregion

        #region "Constructor"

        /// <summary>
        /// El código fuente se pasa como argumento al constructor.
        /// </summary>
        /// <param name="codigo">Código fuente a analizar.</param>
        public CodeAnalyzer(string codigo)
        {
            _codigo = codigo;
        }

        #endregion

        #region "Obtención de Tokens"

        /// <summary>
        /// Método que utiliza el Lexer para obtener la lista de tokens del código fuente.
        /// </summary>
        /// <returns>Lista de tokens generados por el lexer.</returns>
        public List<Token> ObtenerTokens()
        {
            // Crea una instancia del Lexer con el código fuente
            Lexer lexer = new Lexer(_codigo);
            // Llama al método Tokenize para obtener la lista de tokens
            return lexer.Tokenize();
        }

        #endregion

        #region "Ejecución y Visualización"

        /// <summary>
        /// Método que ejecuta el análisis y muestra los tokens generados por consola.
        /// </summary>
        public void EjecutarCodigo()
        {
            // Obtiene la lista de tokens usando el método anterior
            List<Token> tokens = ObtenerTokens();

            // Muestra un encabezado en la consola
            Console.WriteLine("=== Tokens generados ===");
            // Recorre y muestra cada token en la consola
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }

            // Comentario: aquí se puede conectar con el parser y el ejecutor en el futuro
            // Parser parser = new Parser(tokens);
            // var comandos = parser.Parse();
            // Executor executor = new Executor(comandos);
            // executor.Ejecutar();
        }

        #endregion
    }

    #endregion
}