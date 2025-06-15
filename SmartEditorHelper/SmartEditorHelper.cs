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
    /// </summary>
    public class SmartEditorHelper
    {
        private readonly RichTextBox _editor;
        private readonly RichTextBox _ghostEditor;
        private readonly SuggestionPopup _suggestionPopup;
        private readonly List<string> _keywords;
        private readonly List<string> _colors;
        private readonly ToolTip _toolTip;

        public SmartEditorHelper(RichTextBox editor, RichTextBox ghostEditor, SuggestionPopup suggestionPopup, IEnumerable<string> keywords, IEnumerable<string> colors)
        {
            _editor = editor;
            _ghostEditor = ghostEditor;
            _suggestionPopup = suggestionPopup;
            _keywords = keywords.ToList();
            _colors = colors.ToList();
            _toolTip = new ToolTip();

            _editor.KeyDown += Editor_KeyDown;
            _editor.KeyUp += Editor_KeyUp;
            _editor.KeyPress += Editor_KeyPress;
            _editor.MouseMove += Editor_MouseMove;
            _editor.TextChanged += Editor_TextChanged;

            _suggestionPopup.SuggestionSelected += OnSuggestionSelected;
        }

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

        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '(') InsertAutoClose("()");
            else if (e.KeyChar == '[') InsertAutoClose("[]");
            else if (e.KeyChar == '{') InsertAutoClose("{}");
            else if (e.KeyChar == '"') InsertAutoClose("\"\"");
            else if (e.KeyChar == '\'') InsertAutoClose("''");
        }

        private void Editor_MouseMove(object? sender, MouseEventArgs e)
        {
            int index = _editor.GetCharIndexFromPosition(e.Location);
            string word = GetWordAt(index);
            string? desc = GetDescription(word);
            if (!string.IsNullOrEmpty(desc))
                _toolTip.SetToolTip(_editor, desc);
        }

        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            UpdateGhostText();
        }

        private void OnSuggestionSelected(string suggestion)
        {
            InsertSuggestion(suggestion);
        }

        private void InsertSuggestion(string suggestion)
        {
            int pos = _editor.SelectionStart;
            string word = GetCurrentWordBeforeCursor();
            _editor.Select(pos - word.Length, word.Length);
            _editor.SelectedText = suggestion + (IsFunction(suggestion) ? "()" : "");
            _editor.SelectionStart = _editor.Text.Length;
            _suggestionPopup.Hide();
        }

        private void InsertAutoClose(string pair)
        {
            int pos = _editor.SelectionStart;
            _editor.SelectedText = pair;
            _editor.SelectionStart = pos + 1;
        }

        private string GetCurrentWordBeforeCursor()
        {
            int pos = _editor.SelectionStart;
            if (pos == 0) return "";
            string text = _editor.Text.Substring(0, pos);
            string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : "";
        }

        private string GetWordAt(int index)
        {
            if (index < 0 || index >= _editor.Text.Length) return "";
            int start = index, end = index;
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
                "Goto" => "Goto[label](condition): Salta a una etiqueta si se cumple la condición.",
                _ => null
            };
        }

        private bool IsFunction(string word)
        {
            return _keywords.Contains(word);
        }

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
    }
}
