using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
#nullable enable

namespace Wall_E
{
    /// <summary>
    /// Clase auxiliar para proveer autocompletado y sugerencias inteligentes en un editor de código.
    /// </summary>
    public class SmartEditorHelper
    {
        #region CamposPrivados

        // Referencia al control RichTextBox que actúa como editor de código.
        private readonly RichTextBox _editor;
        // ListBox que muestra las sugerencias de autocompletado.
        private readonly ListBox _suggestionBox;
        // Lista de palabras clave del lenguaje.
        private readonly List<string> _keywords;
        // Lista de nombres de colores soportados.
        private readonly List<string> _colors;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el helper de autocompletado con los controles y listas de sugerencias.
        /// </summary>
        /// <param name="editor">RichTextBox del editor de código.</param>
        /// <param name="suggestionBox">ListBox para mostrar sugerencias.</param>
        /// <param name="keywords">Palabras clave del lenguaje.</param>
        /// <param name="colors">Colores soportados.</param>
        public SmartEditorHelper(RichTextBox editor, ListBox suggestionBox, IEnumerable<string> keywords, IEnumerable<string> colors)
        {
            _editor = editor;
            _suggestionBox = suggestionBox;
            _keywords = keywords.ToList();
            _colors = colors.ToList();

            // Suscribe los eventos necesarios para autocompletado y selección de sugerencias.
            _editor.KeyUp += Editor_KeyUp;
            _suggestionBox.DoubleClick += SuggestionBox_DoubleClick;
            _suggestionBox.KeyDown += SuggestionBox_KeyDown;
        }

        #endregion

        #region MétodosAutocompletado

        /// <summary>
        /// Evento que se dispara al soltar una tecla en el editor. Muestra sugerencias si corresponde.
        /// </summary>
        private void Editor_KeyUp(object? sender, KeyEventArgs e)
        {
            // Solo sugerir si se está escribiendo texto
            if (char.IsLetter((char)e.KeyCode) || e.KeyCode == Keys.Back)
            {
                int pos = _editor.SelectionStart;
                string text = _editor.Text.Substring(0, pos);
                string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
                string lastWord = parts.Length > 0 ? parts[^1] : "";

                var suggestions = _keywords.Concat(_colors)
                    .Where(k => k.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (suggestions.Count > 0 && lastWord.Length > 0)
                {
                    _suggestionBox.DataSource = suggestions;
                    _suggestionBox.Visible = true;
                }
                else
                {
                    _suggestionBox.Visible = false;
                }
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                _suggestionBox.Visible = false;
            }
        }

        /// <summary>
        /// Evento que se dispara al hacer doble clic en una sugerencia.
        /// </summary>
        private void SuggestionBox_DoubleClick(object? sender, EventArgs e)
        {
            InsertSuggestion();
        }

        /// <summary>
        /// Evento que se dispara al presionar una tecla en la lista de sugerencias.
        /// </summary>
        private void SuggestionBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsertSuggestion();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Inserta la sugerencia seleccionada en el editor.
        /// </summary>
        private void InsertSuggestion()
        {
            if (_suggestionBox.SelectedItem is string suggestion)
            {
                int pos = _editor.SelectionStart;
                string text = _editor.Text.Substring(0, pos);
                string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
                string lastWord = parts.Length > 0 ? parts[^1] : "";
                _editor.Select(pos - lastWord.Length, lastWord.Length);
                _editor.SelectedText = suggestion;
                _suggestionBox.Visible = false;
            }
        }

        #endregion
    }
}