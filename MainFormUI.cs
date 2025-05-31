using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    /// <summary>
    /// Main window of the Pixel Wall-E IDE.
    /// </summary>
    public class MainForm : Form
    {
        #region Fields
        // TextBox para editar el código fuente
        private TextBox codeEditor;
        // Control personalizado para dibujar píxeles
        private PixelCanvas pixelCanvas;
        // TextBox para mostrar mensajes de error
        private TextBox errorBox;
        // Botón para ejecutar el código
        private Button runButton;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa la ventana principal.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        #region UI Setup
        /// <summary>
        /// Configura todos los controles y el layout de la ventana principal.
        /// </summary>
        private void InitializeComponent()
        {
            // Configuración general de la ventana principal
            Text = "Pixel Wall-E IDE";
            Width = 1375;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;

            // === Panel principal dividido verticalmente ===
            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 50,
                IsSplitterFixed = false
            };
            Controls.Add(mainSplit);

            // === Editor de código (panel izquierdo) ===
            codeEditor = new TextBox
            {
                Multiline = true,
                Font = new Font("Consolas", 10),
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };
            mainSplit.Panel1.Controls.Add(codeEditor);

            // === Panel derecho: canvas y errores ===
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            // El canvas ocupa el 85% superior, los errores el 15% inferior
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 85f));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));
            mainSplit.Panel2.Controls.Add(rightPanel);

            // === Canvas de píxeles (parte superior del panel derecho) ===
            pixelCanvas = new PixelCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            rightPanel.Controls.Add(pixelCanvas, 0, 0);

            // === Panel de errores (parte inferior del panel derecho) ===
            errorBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                BackColor = Color.LightYellow,
                ForeColor = Color.DarkRed,
                Font = new Font("Consolas", 9),
                ScrollBars = ScrollBars.Vertical
            };
            rightPanel.Controls.Add(errorBox, 0, 1);

            // === Botón para ejecutar el código ===
            runButton = new Button
            {
                Text = "Run Code",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            Controls.Add(runButton);

            // Asocia el evento click del botón al método de ejecución
            runButton.Click += RunCode_Click;
        }
        #endregion

        #region Run Button Logic
        /// <summary>
        /// Evento que se dispara al pulsar el botón de ejecutar.
        /// Realiza el proceso de análisis léxico, sintáctico y ejecución del código.
        /// </summary>
        private void RunCode_Click(object sender, EventArgs e)
        {
            // Limpia los errores y el canvas antes de ejecutar
            errorBox.Clear();
            pixelCanvas.Clear();

            try
            {
                // 1. Análisis léxico: convierte el texto en tokens
                string input = codeEditor.Text;
                Lexer lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();

                // 2. Análisis sintáctico: convierte los tokens en instrucciones
                Parser parser = new Parser(tokens);
                List<ICode> codes = parser.Parse();

                // 3. Ejecución de las instrucciones
                Executor executor = new Executor(pixelCanvas);
                executor.Execute(codes);
            }
            catch (RuntimeError ex)
            {
                // Muestra errores de ejecución con la línea correspondiente
                errorBox.Text = $"[Runtime Error] Line {ex.Line}: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Muestra errores internos inesperados
                errorBox.Text = $"[Internal Error] {ex.Message}";
            }
        }
        #endregion
    }
}