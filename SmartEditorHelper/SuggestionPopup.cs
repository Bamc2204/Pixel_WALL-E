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
        #region Fields

        private readonly ListBox listBox;

        #endregion

        #region Events

        /// <summary>
        /// Evento que se dispara cuando se selecciona una sugerencia.
        /// </summary>
        public event Action<string>? SuggestionSelected;

        #endregion

        #region Constructor

        public SuggestionPopup()
        {
            #region ListBoxConfiguration

            listBox = new ListBox
            {
                Dock = DockStyle.Fill,                              // Hace que el ListBox ocupe todo el espacio disponible en el formulario
                Font = new Font("Consolas", 10),                    // Establece la fuente monoespaciada (ideal para código) tamaño 10
                TabStop = false                                     // Evita que el ListBox pueda recibir foco con la tecla Tab
            };

            listBox.MouseClick += (s, e) => OnSelectItem();         // Evento cuando se hace clic con el mouse en el ListBox
            listBox.KeyDown += (s, e) =>                            // Evento para manejar teclas presionadas mientras el ListBox tiene el foco
            {
                if (e.KeyCode == Keys.Enter)
                {
                    OnSelectItem();                                 // Ejecuta el método para seleccionar el ítem
                    e.Handled = true;                               // Marca el evento como manejado para evitar procesamiento adicional
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Hide();                                    // Oculta el formulario
                    e.Handled = true;                               // Marca el evento como manejado
                }
            };

            #endregion

            #region FormConfiguration

            FormBorderStyle = FormBorderStyle.None;                 // Elimina los bordes del formulario
            StartPosition = FormStartPosition.Manual;               // Permite posicionar el formulario manualmente
            ShowInTaskbar = false;                                  // Evita que aparezca en la barra de tareas de Windows
            TopMost = true;                                         // Mantiene el formulario siempre visible sobre otras ventanas

            Controls.Add(listBox);                                  // Agrega el ListBox como control principal del formulario

            #endregion
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Establece las sugerencias que se deben mostrar.
        /// </summary>
        public void SetSuggestions(List<string> suggestions)
        {
            listBox.DataSource = suggestions;                       // Asigna la lista de sugerencias como origen de datos del ListBox
            if (suggestions.Count > 0)                              // Si hay sugerencias disponibles
                listBox.SelectedIndex = 0;                          // Selecciona automáticamente la primera sugerencia
        }

        /// <summary>
        /// Devuelve la sugerencia actualmente seleccionada.
        /// </summary>
        public string GetSelected() => listBox.SelectedItem?.ToString() ?? "";  // Obtiene el texto del ítem seleccionado o cadena vacía si no hay selección

        /// <summary>
        /// Selecciona el primer elemento del ListBox.
        /// </summary>
        public void SelectFirst()
        {
            if (listBox.Items.Count > 0)                            // Verifica si hay elementos en el ListBox
                listBox.SelectedIndex = 0;                          // Selecciona el primer elemento
        }

        /// <summary>
        /// Muestra la ventana en la ubicación dada, relativa al editor.
        /// </summary>
        public void ShowAt(Point caretLocation, Control parentEditor)
        {
            Location = parentEditor.PointToScreen(caretLocation);   // Convierte la posición del cursor a coordenadas de pantalla
            Size = new Size(200, 120);                              // Establece un tamaño fijo para la ventana de sugerencias
            Show();                                                 // Muestra la ventana
            parentEditor.Focus();                                   // Mantiene el foco en el editor
        }

        /// <summary>
        /// Mueve la selección en el ListBox.
        /// </summary>
        public void HandleKey(Keys key)
        {
            if (key == Keys.Down && listBox.SelectedIndex < listBox.Items.Count - 1)    // Si es tecla Abajo y no está en el último elemento
            {
                listBox.SelectedIndex++;                            // Mueve la selección hacia abajo
            }
            else if (key == Keys.Up && listBox.SelectedIndex > 0)   // Si es tecla Arriba y no está en el primer elemento
            {
                listBox.SelectedIndex--;                            // Mueve la selección hacia arriba
            }
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Llama al evento cuando se selecciona una sugerencia.
        /// </summary>
        private void OnSelectItem()
        {
            SuggestionSelected?.Invoke(GetSelected());              // Dispara el evento con la sugerencia seleccionada
            Hide();                                                 // Oculta la ventana de sugerencias
        }

        #endregion
    }
}