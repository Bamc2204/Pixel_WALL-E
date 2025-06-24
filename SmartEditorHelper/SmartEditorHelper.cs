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

        private readonly RichTextBox _editor;                                       // Editor de código principal
        private readonly RichTextBox _ghostEditor;                                  // Editor fantasma para sugerencias
        private readonly SuggestionPopup _suggestionPopup;                          // Popup de sugerencias
        private readonly List<string> _keywords;                                    // Lista de palabras clave del lenguaje
        private readonly List<string> _colors;                                      // Lista de colores disponibles
        private readonly ToolTip _toolTip;                                          // Tooltip para mostrar descripciones
        private int _posCursor;                                                            // Posición del cursor en el editor

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
            if (_suggestionPopup.Visible)                                           // Si el popup de sugerencias está visible
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)                 // Teclas de navegación
                {
                    _suggestionPopup.HandleKey(e.KeyCode);                          // Mueve la selección en el ListBox
                    e.Handled = true;                                               // Marca el evento como manejado
                }
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)          // Teclas de selección
                {
                    InsertSuggestion(_suggestionPopup.GetSelected());               // Inserta la sugerencia seleccionada
                    e.Handled = true;                                               // Marca el evento como manejado
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _suggestionPopup.Hide();                                        // Tecla para cancelar
                    e.Handled = true;                                               // Marca el evento como manejado
                }
            }
        }

        /// <summary>
        /// Maneja el evento KeyUp del editor para mostrar sugerencias.
        /// </summary>
        private void Editor_KeyUp(object? sender, KeyEventArgs e)
        {
            // Solo procesa teclas alfanuméricas y Backspace
            if (!char.IsLetterOrDigit((char)e.KeyCode) && e.KeyCode != Keys.Back)
                return;

            _posCursor = _editor.SelectionStart;                                    // Obtiene posición actual del cursor        
            string prefix = GetCurrentWordBeforeCursor();                           // Obtiene la palabra parcial antes del cursor

            // Filtra sugerencias que coincidan con el prefijo
            var suggestions = _keywords.Concat(_colors)
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))           
                .ToList();

            if (suggestions.Count > 0 && prefix.Length > 0)                         // Si hay sugerencias válidas
            {
                _suggestionPopup.SetSuggestions(suggestions);                       // Carga las sugerencias
                _suggestionPopup.SelectFirst();                                     // Selecciona la primera

                Point caretPos = _editor.GetPositionFromCharIndex(_posCursor);      // Obtiene posición visual del cursor
                _suggestionPopup.ShowAt(caretPos, _editor);                         // Muestra el popup en esa posición
            }
            else
            {
                _suggestionPopup.Hide();                                            // Oculta el popup si no hay sugerencias
            }

            UpdateGhostText();                                                      // Actualiza el texto fantasma
        }

        /// <summary>
        /// Maneja el evento KeyPress del editor para auto-cerrado de caracteres.
        /// </summary>
        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)                                                      // Detecta caracteres de apertura
            {
                case '(':                                                           // Paréntesis
                    InsertAutoClose("()");
                    e.Handled = true;
                    break;
                case '[':                                                           // Corchetes
                    InsertAutoClose("[]");  
                    e.Handled = true;
                    break;
                case '{':                                                           // Llaves
                    InsertAutoClose("{}");
                    e.Handled = true;
                    break;
                case '"':                                                           // Comillas dobles
                    InsertAutoClose("\"\"");
                    e.Handled = true;
                    break;
                case '\'':                                                          // Comillas simples
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
            int index = _editor.GetCharIndexFromPosition(e.Location);               // Posición del carácter bajo el mouse
            string word = GetWordAt(index);                                         // Obtiene la palabra completa
        string? desc = GetDescription(word);                                        // Busca descripción
            if (!string.IsNullOrEmpty(desc))
                _toolTip.SetToolTip(_editor, desc);                                 // Muestra tooltip si existe descripción
        }

        /// <summary>
        /// Maneja cambios en el texto para actualizar el texto fantasma.
        /// </summary>
        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            UpdateGhostText();                                                      // Actualiza sugerencias en texto fantasma
        }

        /// <summary>
        /// Maneja la selección de una sugerencia del popup.
        /// </summary>
        private void OnSuggestionSelected(string suggestion)
        {
            InsertSuggestion(suggestion);                                           // Inserta la sugerencia seleccionada
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Inserta la sugerencia seleccionada en el editor.
        /// </summary>
        private void InsertSuggestion(string suggestion)
        {
            _posCursor = _editor.SelectionStart;                                    // Obtiene posición actual del cursor
            string word = GetCurrentWordBeforeCursor();                             // Palabra parcial a reemplazar
            
            // Caso especial para "GoTo" (agrega corchetes adicionales)
            if (suggestion == "GoTo")
            {
                _editor.Select(_posCursor - word.Length, word.Length);
                _editor.SelectedText = suggestion + (IsFunction(suggestion) ? "[]()" : "");
            }
            // Caso normal
            else
            {
                _editor.Select(_posCursor - word.Length, word.Length);
                _editor.SelectedText = suggestion + (IsFunction(suggestion) ? "()" : "");
            }
            
            _editor.SelectionStart = _editor.Text.Length;                           // Mueve cursor al final
            _suggestionPopup.Hide();                                                // Oculta el popup
        }

        /// <summary>
        /// Inserta pares de caracteres (como paréntesis) y coloca el cursor entre ellos.
        /// </summary>
        private void InsertAutoClose(string pair)
        {
            _posCursor = _editor.SelectionStart;                                    // Obtiene posición actual del cursor
            _editor.SelectedText = pair;                                            // Inserta ambos caracteres
            _editor.SelectionStart = _posCursor + 1;                                // Posiciona cursor en medio
        }

        /// <summary>
        /// Obtiene la palabra actual antes del cursor.
        /// </summary>
        private string GetCurrentWordBeforeCursor()
        {
            _posCursor = _editor.SelectionStart;                                    // Obtiene posición actual del cursor
            if (_posCursor == 0) return "";                                         // Si el cursor está al principio del texto, devuelve vacío
            string text = _editor.Text.Substring(0, _posCursor);                    // Extrae la palabra antes del cursor
            // Divide usando varios delimitadoress
            string[] parts = text.Split(new[] { ' ', '\n', '\r', '(', '[', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[^1] : "";                               // Devuelve el último elemento
        }

        /// <summary>
        /// Obtiene la palabra en la posición especificada.
        /// </summary>
        private string GetWordAt(int index)
        {
            if (index < 0 || index >= _editor.Text.Length) return "";               // Si la posición está fuera de los límites, devuelve vacío
            int start = index, end = index;                                         // Inicializa índices de inicio y fin

            // Avanza hasta el inicio de la palabra
            while (start > 0 && char.IsLetterOrDigit(_editor.Text[start - 1])) start--;
            // Avanza hasta el final de la palabra
            while (end < _editor.Text.Length && char.IsLetterOrDigit(_editor.Text[end])) end++;

            return _editor.Text.Substring(start, end - start);                      // Extrae la palabra
        }

        /// <summary>
        /// Obtiene la descripción de una palabra clave.
        /// </summary>
        private string? GetDescription(string word)
        {
            // Diccionario de descripciones para palabras clave
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
            return _keywords.Contains(word);                                        // Verifica en la lista de palabras clave
        }

        /// <summary>
        /// Actualiza el texto fantasma con sugerencias de autocompletado.
        /// </summary>
        private void UpdateGhostText()
        {
            string text = _editor.Text;                                             // Obtiene el texto completo del editor principal
            _posCursor = _editor.SelectionStart;                                    // Obtiene posición actual del cursor 
            string prefix = GetCurrentWordBeforeCursor();                           // Obtiene la palabra parcial que está siendo escrita (antes del cursor)

            // Busca la primera coincidencia entre palabras clave y colores que empiece con el prefijo
            // (ignorando mayúsculas/minúsculas)
            var match = _keywords.Concat(_colors).FirstOrDefault(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            // Si encontró una coincidencia válida y el match es más largo que el prefijo
            if (!string.IsNullOrEmpty(match) && match.Length > prefix.Length)
            {
                // Crea texto fantasma con la parte faltante
                string ghost = text.Insert(_posCursor, match.Substring(prefix.Length));
                _ghostEditor.Text = ghost;

                // Resalta en gris la parte sugerida
                _ghostEditor.Select(_posCursor, match.Length - prefix.Length);
                _ghostEditor.SelectionColor = Color.LightGray;
            }
            else
            {
                _ghostEditor.Text = text;                                           // Sin sugerencia, muestra texto normal
            }
        }

        #endregion
    }
}