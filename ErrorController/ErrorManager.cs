using System;
using System.Collections.Generic;

namespace Wall_E
{
    /// <summary>
    /// Clase centralizada para almacenar errores en la aplicación.
    /// </summary>
    public static class ErrorManager
    {
        private static readonly List<string> _errors = new();

        /// <summary>
        /// Agrega un error a la lista.
        /// </summary>
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
        /// Devuelve todos los errores almacenados como un solo string.
        /// </summary>
        public static string GetAll()
        {
            return string.Join(Environment.NewLine, _errors);
        }

        /// <summary>
        /// Devuelve la lista de errores.
        /// </summary>
        public static List<string> GetList()
        {
            return new List<string>(_errors);
        }
    }
}