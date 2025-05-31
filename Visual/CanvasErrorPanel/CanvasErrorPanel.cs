using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// Panel personalizado que contiene un TextBox para mostrar mensajes de error.
/// </summary>
public class CanvasErrorPanel : Panel
{
    #region Fields

    // TextBox interno utilizado para mostrar los mensajes de error
    private TextBox errorBox;

    #endregion

    #region Constructor

    /// <summary>
    /// Inicializa el panel y configura el TextBox para mostrar errores.
    /// </summary>
    public CanvasErrorPanel()
    {
        errorBox = new TextBox
        {
            Multiline = true,           // Permite mostrar varias líneas de error
            Dock = DockStyle.Fill,      // Ocupa todo el panel
            ReadOnly = true,            // Solo lectura, no editable por el usuario
            ScrollBars = ScrollBars.Vertical, // Barra de desplazamiento vertical
            BackColor = Color.White,    // Fondo blanco para los errores
            ForeColor = Color.DarkRed   // Texto en rojo oscuro para resaltar errores
        };
        Controls.Add(errorBox);         // Agrega el TextBox al panel
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra un mensaje de error en el TextBox.
    /// </summary>
    /// <param name="message">Mensaje de error a mostrar</param>
    public void ShowError(string message)
    {
        errorBox.Text = message;
    }

    /// <summary>
    /// Limpia el mensaje de error mostrado.
    /// </summary>
    public void Clear()
    {
        errorBox.Clear();
    }

    #endregion
}