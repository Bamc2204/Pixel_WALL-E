using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
#nullable enable

namespace Wall_E
{
    /// <summary>
    /// Ventana emergente para mostrar sugerencias de autocompletado.
    /// </summary>
    public class SuggestionPopup : Form
    {
        private readonly ListBox listBox;

        /// <summary>
        /// Evento que se dispara cuando se selecciona una sugerencia.
        /// </summary>
        public event Action<string>? SuggestionSelected;

        public SuggestionPopup()
        {
            listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                TabStop = false // No roba el foco del editor
            };

            listBox.MouseClick += (s, e) => OnSelectItem();
            listBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    OnSelectItem();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Hide();
                    e.Handled = true;
                }
            };

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;

            Controls.Add(listBox);
        }

        /// <summary>
        /// Establece las sugerencias que se deben mostrar.
        /// </summary>
        public void SetSuggestions(List<string> suggestions)
        {
            listBox.DataSource = suggestions;
            if (suggestions.Count > 0)
                listBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Devuelve la sugerencia actualmente seleccionada.
        /// </summary>
        public string GetSelected() => listBox.SelectedItem?.ToString() ?? "";

        /// <summary>
        /// Selecciona el primer elemento del ListBox.
        /// </summary>
        public void SelectFirst()
        {
            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Muestra la ventana en la ubicación dada, relativa al editor.
        /// </summary>
        public void ShowAt(Point caretLocation, Control parentEditor)
        {
            Location = parentEditor.PointToScreen(caretLocation);
            Size = new Size(200, 120);
            Show();
            parentEditor.Focus(); // Mantiene el foco en el editor
        }

        /// <summary>
        /// Mueve la selección en el ListBox.
        /// </summary>
        public void HandleKey(Keys key)
        {
            if (key == Keys.Down && listBox.SelectedIndex < listBox.Items.Count - 1)
            {
                listBox.SelectedIndex++;
            }
            else if (key == Keys.Up && listBox.SelectedIndex > 0)
            {
                listBox.SelectedIndex--;
            }
        }

        /// <summary>
        /// Llama al evento cuando se selecciona una sugerencia.
        /// </summary>
        private void OnSelectItem()
        {
            SuggestionSelected?.Invoke(GetSelected());
            Hide();
        }
    }
}
