using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
#nullable enable

namespace Wall_E
{
    /// <summary>
    /// Clase auxiliar para manejar autocompletado y sugerencias inteligentes.
    /// Proporciona funcionalidades avanzadas de edición como:
    /// - Autocompletado de código
    /// - Sugerencias inteligentes
    /// - Cierre automático de llaves, paréntesis y comillas
    /// - Tooltips descriptivos
    /// - Texto fantasma para sugerencias
    /// </summary>
    public class SmartEditorHelper
    {
        #region Fields

        private readonly RichTextBox _editor;           // Editor de código principal
        private readonly RichTextBox _ghostEditor;      // Editor fantasma para sugerencias
        private readonly SuggestionPopup _suggestionPopup; // Popup de sugerencias
        private readonly List<string> _keywords;       // Lista de palabras clave del lenguaje
        private readonly List<string> _colors;         // Lista de colores disponibles
        private readonly ToolTip _toolTip;             // Tooltip para mostrar descripciones

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa una nueva instancia del ayudante de edición inteligente.
        /// </summary>
        /// <param name="editor">Editor de código principal</param>
        /// <param name="ghostEditor">Editor fantasma para sugerencias</param>
        /// <param name="suggestionPopup">Popup para mostrar sugerencias</param>
        /// <param name="keywords">Palabras clave del lenguaje</param>
        /// <param name="colors">Colores disponibles</param>
        public SmartEditorHelper(RichTextBox editor, RichTextBox ghostEditor, 
                               SuggestionPopup suggestionPopup, 
                               IEnumerable<string> keywords, 
                               IEnumerable<string> colors)
        {
            _editor = editor;
            _ghostEditor = ghostEditor;
            _suggestionPopup = suggestionPopup;
            _keywords = keywords.ToList();
            _colors = colors.ToList();
            _toolTip = new ToolTip();

            #region EventSubscriptions
            // Suscribe los eventos del editor
            _editor.KeyDown += Editor_KeyDown;
            _editor.KeyUp += Editor_KeyUp;
            _editor.KeyPress += Editor_KeyPress;
            _editor.MouseMove += Editor_MouseMove;
            _editor.TextChanged += Editor_TextChanged;

            // Suscribe el evento de selección de sugerencia
            _suggestionPopup.SuggestionSelected += OnSuggestionSelected;
            #endregion
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Maneja el evento KeyDown del editor para navegar en las sugerencias.
        /// </summary>
        private void Editor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_suggestionPopup.Visible)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    _suggestionPopup.HandleKey(e.KeyCode);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    InsertSuggestion(_suggestionPopup.GetSelected());
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _suggestionPopup.Hide();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Maneja el evento KeyUp del editor para mostrar sugerencias.
        /// </summary>
        private void Editor_KeyUp(object? sender, KeyEventArgs e)
        {
            if (!char.IsLetterOrDigit((char)e.KeyCode) && e.KeyCode != Keys.Back)
                return;

            int pos = _editor.SelectionStart;
            string prefix = GetCurrentWordBeforeCursor();

            var suggestions = _keywords.Concat(_colors)
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (suggestions.Count > 0 && prefix.Length > 0)
            {
                _suggestionPopup.SetSuggestions(suggestions);
                _suggestionPopup.SelectFirst();

                Point caretPos = _editor.GetPositionFromCharIndex(pos);
                _suggestionPopup.ShowAt(caretPos, _editor);
            }
            else
            {
                _suggestionPopup.Hide();
            }

            UpdateGhostText();
        }

        /// <summary>
        /// Maneja el evento KeyPress del editor para auto-cerrado de caracteres.
        /// </summary>
        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '(':
                    InsertAutoClose("()");
                    e.Handled = true;
                    break;
                case '[':
                    InsertAutoClose("[]");
                    e.Handled = true;
                    break;
                case '{':
                    InsertAutoClose("{}");
                    e.Handled = true;
                    break;
                case '"':
                    InsertAutoClose("\"\"");
                    e.Handled = true;
                    break;
                case '\'':
                    InsertAutoClose("''");
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Maneja el movimiento del mouse para mostrar tooltips descriptivos.
        /// </summary>
        private void Editor_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _editor.GetCharIndexFromPosition(e.Location);
            string word = GetWordAt(index);
            string? desc = GetDescription(word);
            if (!string.IsNullOrEmpty(desc))
                _toolTip.SetToolTip(_editor, desc);
        }

        /// <summary>
        /// Maneja cambios en el texto para actualizar el texto fantasma.
        /// </summary>
        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            UpdateGhostText();
        }

        /// <summary>
        /// Maneja la selección de una sugerencia del popup.
        /// </summary>
        private void OnSuggestionSelected(string suggestion)
        {
            InsertSuggestion(suggestion);
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Inserta la sugerencia seleccionada en el editor.
        /// </summary>
        private void InsertSuggestion(string suggestion)
        {
            if (suggestion == "GoTo")
            {
                int pos = _editor.SelectionStart;
                string word = GetCurrentWordBeforeCursor();
                _editor.Select(pos - word.Length, word.Length);
                _editor.SelectedText = suggestion + (IsFunction(suggestion) ? "[]()" : "");
                _editor.SelectionStart = _editor.Text.Length;
                _suggestionPopup.Hide();
            }
            else
            {
                int pos = _editor.SelectionStart;
                string word = GetCurrentWordBeforeCursor();
                _editor.Select(pos - word.Length, word.Length);
                _editor.SelectedText = suggestion + (IsFunction(suggestion) ? "()" : "");
                _editor.SelectionStart = _editor.Text.Length;
                _suggestionPopup.Hide();
            }
        }

        /// <summary>
        /// Inserta pares de caracteres (como paréntesis) y coloca el cursor entre ellos.
        /// </summary>
        private void InsertAutoClose(string pair)
        {
            int pos = _editor.SelectionStart;
            _editor.SelectedText = pair;
            _editor.SelectionStart = pos + 1;
        }

        /// <summary>
        /// Obtiene la palabra actual antes del cursor.
        /// </summary>
        private string GetCurrentWordBeforeCursor()
        {
            int pos = _editor.SelectionStart;
            if (pos == 0) return "";
            string text = _editor.Text.Substring(0, pos);
            string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : "";
        }

        /// <summary>
        /// Obtiene la palabra en la posición especificada.
        /// </summary>
        private string GetWordAt(int index)
        {
            if (index < 0 || index >= _editor.Text.Length) return "";
            int start = index, end = index;
            while (start > 0 && char.IsLetterOrDigit(_editor.Text[start - 1])) start--;
            while (end < _editor.Text.Length && char.IsLetterOrDigit(_editor.Text[end])) end++;
            return _editor.Text.Substring(start, end - start);
        }

        /// <summary>
        /// Obtiene la descripción de una palabra clave.
        /// </summary>
        private string? GetDescription(string word)
        {
            return word switch
            {
                "Spawn" => "Spawn(int x, int y): Posiciona a Wall-E.",
                "Color" => "Color(string name): Cambia el color del pincel.",
                "Size" => "Size(int value): Cambia el tamaño del pincel.",
                "DrawLine" => "DrawLine(int dx, int dy, int distance): Dibuja una línea.",
                "DrawCircle" => "DrawCircle(int dx, int dy, int radius): Dibuja un círculo.",
                "DrawRectangle" => "DrawRectangle(int dx, int dy, int distance, int width, int height): Dibuja un rectángulo.",
                "Fill" => "Fill(): Rellena la región actual.",
                "Goto" => "Goto[label](condition): Salta a una etiqueta si se cumple la condición.",
                _ => null
            };
        }

        /// <summary>
        /// Determina si una palabra es una función.
        /// </summary>
        private bool IsFunction(string word)
        {
            return _keywords.Contains(word);
        }

        /// <summary>
        /// Actualiza el texto fantasma con sugerencias de autocompletado.
        /// </summary>
        private void UpdateGhostText()
        {
            string text = _editor.Text;
            int pos = _editor.SelectionStart;
            string prefix = GetCurrentWordBeforeCursor();

            var match = _keywords.Concat(_colors).FirstOrDefault(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(match) && match.Length > prefix.Length)
            {
                string ghost = text.Insert(pos, match.Substring(prefix.Length));
                _ghostEditor.Text = ghost;
                _ghostEditor.Select(pos, match.Length - prefix.Length);
                _ghostEditor.SelectionColor = Color.LightGray;
            }
            else
            {
                _ghostEditor.Text = text;
            }
        }

        #endregion
    }
}