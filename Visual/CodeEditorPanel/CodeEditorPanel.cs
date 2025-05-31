using System.Windows.Forms;
using System.Drawing;

/// <summary>
/// Panel personalizado que contiene un editor de código basado en TextBox.
/// </summary>
public class CodeEditorPanel : Panel
{
    #region Fields

    // TextBox interno que actúa como editor de código
    private TextBox codeTextBox;

    #endregion

    #region Constructor

    /// <summary>
    /// Inicializa el panel y configura el TextBox para edición de código.
    /// </summary>
    public CodeEditorPanel()
    {
        codeTextBox = new TextBox
        {
            Multiline = true,           // Permite varias líneas de código
            Dock = DockStyle.Fill,      // Ocupa todo el panel
            ScrollBars = ScrollBars.Both, // Barras de desplazamiento horizontal y vertical
            Font = new Font("Consolas", 10), // Fuente monoespaciada para código
            AcceptsTab = true           // Permite tabulaciones dentro del editor
        };
        Controls.Add(codeTextBox);      // Agrega el TextBox al panel
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Obtiene el código actual del editor.
    /// </summary>
    public string GetCode() => codeTextBox.Text;

    /// <summary>
    /// Establece el texto del editor de código.
    /// </summary>
    public void SetCode(string code) => codeTextBox.Text = code;

    #endregion
}