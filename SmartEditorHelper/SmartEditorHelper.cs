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

        private readonly RichTextBox _editor;
        private readonly RichTextBox _ghostEditor;
        private readonly ListBox _suggestionBox;
        private readonly List<string> _keywords;
        private readonly List<string> _colors;
        private readonly ToolTip _toolTip;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa una nueva instancia de la clase SmartEditorHelper.
        /// </summary>
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
            _editor.KeyDown += Editor_KeyDown;
            _editor.MouseMove += Editor_MouseMove;
            _editor.TextChanged += Editor_TextChanged;

            _suggestionBox.DoubleClick += SuggestionBox_DoubleClick;
            _suggestionBox.KeyDown += SuggestionBox_KeyDown;
        }

        #endregion

        #region EventosEditor

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

                    // NUEVO: Posicionar el ListBox debajo del cursor
                    Point caretPos = _editor.GetPositionFromCharIndex(pos);
                    caretPos = _editor.PointToScreen(caretPos); // Pasar a coordenadas de pantalla
                    caretPos = _suggestionBox.Parent!.PointToClient(caretPos); // Convertir a coordenadas relativas al contenedor del ListBox
                    caretPos.Offset(0, (int)Math.Ceiling(_editor.Font.GetHeight()));
                    _suggestionBox.Location = caretPos;
                    _suggestionBox.BringToFront();
                }
                else
                {
                    _suggestionBox.Visible = false;
                }

                UpdateGhostText();
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                _suggestionBox.Visible = false;
                _ghostEditor.Text = _editor.Text;
            }
            else
            {
                UpdateGhostText();
            }
        }

        public void Editor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_suggestionBox.Visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    _suggestionBox.Focus();
                    if (_suggestionBox.SelectedIndex < _suggestionBox.Items.Count - 1)
                        _suggestionBox.SelectedIndex++;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    _suggestionBox.Focus();
                    if (_suggestionBox.SelectedIndex > 0)
                        _suggestionBox.SelectedIndex--;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    InsertSuggestion();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _suggestionBox.Visible = false;
                }
            }
        }

        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            int pos = _editor.SelectionStart;

            switch (e.KeyChar)
            {
                case '(':
                    if (!NextCharIs(')')) InsertAutoClose("()");
                    e.Handled = true;
                    break;
                case '[':
                    if (!NextCharIs(']')) InsertAutoClose("[]");
                    e.Handled = true;
                    break;
                case '{':
                    if (!NextCharIs('}')) InsertAutoClose("{}");
                    e.Handled = true;
                    break;
                case '"':
                    if (!NextCharIs('"')) InsertAutoClose("\"\"");
                    e.Handled = true;
                    break;
                case '\'':
                    if (!NextCharIs('\'')) InsertAutoClose("''");
                    e.Handled = true;
                    break;
            }
        }

        private bool NextCharIs(char expected)
        {
            int pos = _editor.SelectionStart;
            return pos < _editor.Text.Length && _editor.Text[pos] == expected;
        }

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

        public void Editor_TextChanged(object? sender, EventArgs e)
        {
            UpdateGhostText();
        }

        #endregion

        #region Sugerencias

        private void SuggestionBox_DoubleClick(object? sender, EventArgs e)
        {
            InsertSuggestion();
        }

        private void SuggestionBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InsertSuggestion();
                e.Handled = true;

                _editor.Focus(); // <- vuelve al editor
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _suggestionBox.Visible = false;
                _editor.Focus();
                e.Handled = true;
            }
        }

        private void InsertSuggestion()
        {
            if (_suggestionBox.SelectedItem is string suggestion)
            {
                int pos = _editor.SelectionStart;
                string text = _editor.Text.Substring(0, pos);

                // Detecta la última palabra que se está escribiendo
                string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
                string lastWord = parts.Length > 0 ? parts[^1] : "";

                // Reemplaza solo la palabra escrita
                _editor.Select(pos - lastWord.Length, lastWord.Length);

                // Agrega paréntesis solo si el carácter siguiente no es ')'
                bool shouldInsertParentheses = pos >= _editor.Text.Length || _editor.Text[pos] != ')';

                if (_keywords.Contains(suggestion) && shouldInsertParentheses)
                {
                    _editor.SelectedText = suggestion + "()";
                    _editor.SelectionStart -= 1; // Coloca el cursor dentro de los paréntesis
                }
                else
                {
                    _editor.SelectedText = suggestion;
                }

                _suggestionBox.Visible = false;
                _ghostEditor.Text = _editor.Text;
            }
        }


        #endregion

        #region Utilidades

        private void InsertAutoClose(string pair)
        {
            int pos = _editor.SelectionStart;
            _editor.SelectedText = pair;
            _editor.SelectionStart = pos + 1;
        }

        private string GetWordAt(int index)
        {
            if (index < 0 || index >= _editor.Text.Length) return "";
            int start = index;
            int end = index;
            while (start > 0 && char.IsLetterOrDigit(_editor.Text[start - 1])) start--;
            while (end < _editor.Text.Length && char.IsLetterOrDigit(_editor.Text[end])) end++;
            return _editor.Text.Substring(start, end - start);
        }

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

        private void UpdateGhostText()
        {
            string text = _editor.Text;
            int pos = _editor.SelectionStart;

            string[] parts = text.Substring(0, pos).Split(new[] { ' ', '\n', '\r', '(', '[', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string lastWord = parts.Length > 0 ? parts[^1] : "";

            var suggestions = _keywords.Concat(_colors)
                .Where(k => k.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (suggestions.Count > 0 && lastWord.Length > 0)
            {
                string suggestion = suggestions[0];
                string suffix = suggestion.Substring(lastWord.Length);

                string ghostText = text.Insert(pos, suffix);
                _ghostEditor.Text = ghostText;

                _ghostEditor.Select(pos, suffix.Length);
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
