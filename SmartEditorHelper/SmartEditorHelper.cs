using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
#nullable enable

namespace Wall_E
{
    /// <summary>
    /// Clase auxiliar para proveer autocompletado y sugerencias inteligentes en un editor de código.
    /// </summary>
    public class SmartEditorHelper
    {
        #region CamposPrivados

        // Editor de texto principal donde se escribe el código
        private readonly RichTextBox _editor;
        // Cuadro de lista que muestra sugerencias de autocompletado
        private readonly ListBox _suggestionBox;
        // Lista de palabras clave válidas
        private readonly List<string> _keywords;
        // Lista de nombres de colores válidos
        private readonly List<string> _colors;
        // Tooltip para mostrar descripciones al pasar el cursor
        private readonly ToolTip _toolTip;
        private readonly RichTextBox _ghostEditor;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa una nueva instancia de la clase SmartEditorHelper.
        /// </summary>
        /// <param name="editor">El editor de texto RichTextBox asociado.</param>
        /// <param name="suggestionBox">El ListBox que mostrará las sugerencias.</param>
        /// <param name="keywords">Las palabras clave válidas.</param>
        /// <param name="colors">Los nombres de colores válidos.</param>
        public SmartEditorHelper(RichTextBox editor, RichTextBox ghostEditor, ListBox suggestionBox, IEnumerable<string> keywords, IEnumerable<string> colors)
        {
            _editor = editor;
            _ghostEditor = ghostEditor;
            _suggestionBox = suggestionBox;
            _keywords = keywords.ToList();
            _colors = colors.ToList();
            _toolTip = new ToolTip();

            _editor.KeyUp += Editor_KeyUp;
            _editor.KeyPress += Editor_KeyPress;
            _editor.KeyDown += Editor_KeyDown; // Añadimos esto para detectar Tab
            _editor.MouseMove += Editor_MouseMove;
            _suggestionBox.DoubleClick += SuggestionBox_DoubleClick;
            _suggestionBox.KeyDown += SuggestionBox_KeyDown;
        }


        #endregion

        #region EventosPrincipales

        /// <summary>
        /// Maneja el evento KeyUp del editor para mostrar sugerencias cuando se escribe.
        /// </summary>
        public void Editor_KeyUp(object? sender, KeyEventArgs e)
        {
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
            // Actualizar texto del ghostEditor para mostrar la sugerencia inline
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
                    string firstSuggestion = suggestions[0];
                    if (firstSuggestion.Length > lastWord.Length)
                    {
                        string remaining = firstSuggestion.Substring(lastWord.Length);
                        _ghostEditor.Text = _editor.Text.Insert(pos, remaining);
                    }
                    else
                    {
                        _ghostEditor.Text = _editor.Text;
                    }
                }
                else
                {
                    _ghostEditor.Text = _editor.Text;
                }
            }
            else
            {
                _ghostEditor.Text = _editor.Text;
            }
        }

        public void Editor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                int pos = _editor.SelectionStart;
                string text = _editor.Text.Substring(0, pos);
                string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
                string lastWord = parts.Length > 0 ? parts[^1] : "";

                var match = _keywords.Concat(_colors)
                    .FirstOrDefault(k => k.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase));

                if (match != null && match.Length > lastWord.Length)
                {
                    _editor.Select(pos - lastWord.Length, lastWord.Length);
                    _editor.SelectedText = match;
                    e.Handled = true;
                    _ghostEditor.Text = _editor.Text;
                }
            }
        }


        /// <summary>
        /// Maneja el evento KeyPress para insertar automáticamente pares de símbolos.
        /// </summary>
        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '(')
            {
                InsertAutoClose("()");
                e.Handled = true;
            }
            else if (e.KeyChar == '[')
            {
                InsertAutoClose("[]");
                e.Handled = true;
            }
            else if (e.KeyChar == '"')
            {
                InsertAutoClose("\"\"");
                e.Handled = true;
            }
        }

        /// <summary>
        /// Muestra un tooltip con la descripción de la palabra sobre la que está el cursor.
        /// </summary>
        private void Editor_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _editor.GetCharIndexFromPosition(e.Location);
            if (index >= 0 && index < _editor.Text.Length)
            {
                string hoveredWord = GetWordAt(index);
                if (!string.IsNullOrEmpty(hoveredWord))
                {
                    string? desc = GetDescription(hoveredWord);
                    if (desc != null)
                    {
                        _toolTip.SetToolTip(_editor, desc);
                    }
                }
            }
        }

        #endregion

        #region Sugerencias

        /// <summary>
        /// Inserta la sugerencia seleccionada al hacer doble clic en la lista.
        /// </summary>
        private void SuggestionBox_DoubleClick(object? sender, EventArgs e)
        {
            InsertSuggestion();
        }

        /// <summary>
        /// Inserta la sugerencia seleccionada al presionar Enter en la lista.
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
        /// Inserta la sugerencia elegida en el editor en la posición correspondiente.
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

                if (_keywords.Contains(suggestion))
                {
                    _editor.SelectedText = suggestion + "()";
                    _editor.SelectionStart--;
                }
                else
                {
                    _editor.SelectedText = suggestion;
                }
                _suggestionBox.Visible = false;
            }
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Inserta automáticamente el par de cierre para un carácter abierto.
        /// </summary>
        private void InsertAutoClose(string pair)
        {
            int pos = _editor.SelectionStart;
            _editor.SelectedText = pair;
            _editor.SelectionStart = pos + 1;
        }

        /// <summary>
        /// Obtiene la palabra completa en la posición de índice especificada del texto.
        /// </summary>
        private string GetWordAt(int index)
        {
            if (index < 0 || index >= _editor.Text.Length) return "";
            int start = index;
            int end = index;
            while (start > 0 && char.IsLetterOrDigit(_editor.Text[start - 1])) start--;
            while (end < _editor.Text.Length && char.IsLetterOrDigit(_editor.Text[end])) end++;
            return _editor.Text.Substring(start, end - start);
        }

        /// <summary>
        /// Devuelve una descripción asociada a la palabra especificada, si existe.
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
                "GoTo" => "Goto[label](condition): Salta a una etiqueta si se cumple la condición.",
                _ => null
            };
        }

        public void Editor_TextChanged(object? sender, EventArgs e)
        {
            UpdateGhostText();
        }

        /// <summary>
        /// Actualiza el contenido del ghostEditor para mostrar sugerencias inline en gris.
        /// </summary>
        private void UpdateGhostText()
        {
            // Obtiene el texto actual y la posición del cursor
            string text = _editor.Text;
            int pos = _editor.SelectionStart;

            // Divide en palabras antes de la posición actual
            string[] parts = text.Substring(0, pos).Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string lastWord = parts.Length > 0 ? parts[^1] : "";

            // Busca sugerencias coincidentes
            var suggestions = _keywords.Concat(_colors)
                .Where(k => k.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (suggestions.Count > 0 && lastWord.Length > 0)
            {
                string suggestion = suggestions[0];
                string suffix = suggestion.Substring(lastWord.Length); // Parte que falta

                // Construye texto gris
                string ghostText = text.Insert(pos, suffix);
                _ghostEditor.Text = ghostText;

                // Copia formato de saltos de línea
                _ghostEditor.Select(pos, suffix.Length);
                _ghostEditor.SelectionColor = Color.LightGray;
            }
            else
            {
                // Si no hay sugerencia, solo copia el texto actual
                _ghostEditor.Text = text;
            }
        }


        #endregion
    }
}
