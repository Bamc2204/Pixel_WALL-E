using System;
using System.Collections.Generic;

namespace Wall_E
{
    /// <summary>
    /// Clase centralizada para almacenar y gestionar errores en la aplicación.
    /// Permite agregar, limpiar y recuperar errores de forma global.
    /// </summary>
    public static class ErrorManager
    {
        #region Fields

        // Lista interna de errores.
        private static readonly List<string> _errors = new();

        #endregion

        #region PublicAPI

        /// <summary>
        /// Agrega un error a la lista.
        /// </summary>
        /// <param name="message">Mensaje de error a agregar.</param>
        public static void Add(string message)
        {
            _errors.Add(message);
        }

        /// <summary>
        /// Limpia todos los errores almacenados.
        /// </summary>
        public static void Clear()
        {
            _errors.Clear();
        }

        /// <summary>
        /// Devuelve todos los errores almacenados como un solo string, separados por saltos de línea.
        /// </summary>
        /// <returns>Cadena con todos los errores.</returns>
        public static string GetAll()
        {
            return string.Join(Environment.NewLine, _errors);
        }

        /// <summary>
        /// Devuelve una copia de la lista de errores.
        /// </summary>
        /// <returns>Lista de errores.</returns>
        public static List<string> GetList()
        {
            return new List<string>(_errors);
        }

        #endregion
    }
}