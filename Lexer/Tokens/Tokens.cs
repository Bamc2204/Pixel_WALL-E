namespace Wall_E
{
    #region TokenClass

    /// <summary>
    /// Clase que representa un token individual generado por el lexer.
    /// Un token es una unidad mínima del lenguaje (palabra clave, número, símbolo, etc.).
    /// </summary>
    public class Token
    {
        #region Properties

        /// <summary>
        /// Tipo de token (por ejemplo, palabra clave, número, símbolo, etc.).
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// El texto exacto del código fuente que corresponde a este token.
        /// </summary>
        public string Lexeme { get; }

        /// <summary>
        /// Número de línea donde se encontró este token en el código fuente.
        /// </summary>
        public int Line { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el token con su tipo, texto y línea.
        /// </summary>
        /// <param name="type">Tipo de token (TokenType).</param>
        /// <param name="lexeme">Texto exacto del código fuente.</param>
        /// <param name="line">Número de línea donde se encontró el token.</param>
        public Token(TokenType type, string lexeme, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
        }

        #endregion

        #region RepresentationMethods

        /// <summary>
        /// Devuelve una representación legible del token, útil para depuración.
        /// </summary>
        /// <returns>Cadena con el tipo, texto y línea del token.</returns>
        public override string ToString()
        {
            return $"{Type} \"{Lexeme}\" (línea {Line})";
        }

        #endregion
    }

    #endregion
}