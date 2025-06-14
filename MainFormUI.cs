using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Wall_E
{
    /// <summary>
    /// Ventana principal del IDE de Pixel Wall-E. Gestiona la interfaz gráfica, el editor de código,
    /// el canvas de dibujo, la consola de errores y la interacción con el usuario.
    /// </summary>
    public class MainForm : Form
    {
        #region Fields

        // Editor de código principal.
        private RichTextBox codeEditor = null!;
        // Panel para mostrar los números de línea.
        private Panel lineNumberPanel = null!;
        // Canvas de píxeles para dibujar.
        private PixelCanvas pixelCanvas = null!;
        // Caja de texto para mostrar errores.
        private TextBox wall_EConsole = null!;
        // Splitter principal de la ventana.
        private SplitContainer mainSplit = null!;
        // ListBox para sugerencias de autocompletado.
        private ListBox suggestionBox = null!;
        // Helper para autocompletado inteligente.
        private RichTextBox ghostEditor = null!;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa la ventana principal y sus componentes.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #endregion

        #region UIInitialization

        /// <summary>
        /// Inicializa y configura todos los controles de la interfaz gráfica.
        /// </summary>
        private void InitializeComponent()
        {
            #region Configuración de la ventana principal
            Text = "Pixel Wall-E IDE";
            Width = 1400;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;
            #endregion

            #region División principal
            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };
            Controls.Add(mainSplit);
            #endregion

            #region Panel izquierdo - Editor de código y botones
            var editorPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            mainSplit.Panel1.Controls.Add(editorPanel);

            // Botones de archivo y ejecución
            var fileButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill
            };

            var openButton = new Button { Text = "Open" };
            var saveButton = new Button { Text = "Save" };
            var undoButton = new Button { Text = "Undo" };
            var redoButton = new Button { Text = "Redo" };
            var runButton = new Button { Text = "Run Code" };

            openButton.Click += OpenFile;
            saveButton.Click += SaveFile;
            undoButton.Click += (s, e) => codeEditor.Undo();
            redoButton.Click += (s, e) => codeEditor.Redo();
            runButton.Click += RunCode_Click;

            fileButtons.Controls.Add(openButton);
            fileButtons.Controls.Add(saveButton);
            fileButtons.Controls.Add(undoButton);
            fileButtons.Controls.Add(redoButton);
            fileButtons.Controls.Add(runButton);
            editorPanel.Controls.Add(fileButtons, 0, 0);
            #endregion

            #region Editor principal, ghost y sugerencias
            // Panel de números de línea
            lineNumberPanel = new Panel
            {
                Width = 40,
                Dock = DockStyle.Left,
                BackColor = Color.LightGray
            };

            // Editor real
            codeEditor = new RichTextBox
            {
                Font = new Font("Consolas", 10),
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                AcceptsTab = true,
                BorderStyle = BorderStyle.None
            };

            // Editor fantasma
            ghostEditor = new RichTextBox
            {
                Font = codeEditor.Font,
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                ForeColor = Color.LightGray,
                BorderStyle = BorderStyle.None,
                Enabled = false,
                TabStop = false,
                ScrollBars = RichTextBoxScrollBars.None,
                WordWrap = codeEditor.WordWrap,
                Location = codeEditor.Location,
                Text = ""
            };

            // Superposición de editores
            var editorOverlay = new Panel { Dock = DockStyle.Fill };
            editorOverlay.Controls.Add(ghostEditor);
            editorOverlay.Controls.Add(codeEditor);
            codeEditor.BringToFront();
            ghostEditor.SendToBack();

            // ListBox de sugerencias
            suggestionBox = new ListBox
            {
                Visible = false,
                Font = codeEditor.Font,
                Height = 100,
                Width = 200
            };

            // Panel que contiene todo
            var editorContainer = new Panel { Dock = DockStyle.Fill };
            editorContainer.Controls.Add(editorOverlay);
            editorContainer.Controls.Add(lineNumberPanel);
            editorContainer.Controls.Add(suggestionBox);

            editorPanel.Controls.Add(editorContainer, 0, 1);
            #endregion

            #region Eventos del editor de código
            codeEditor.TextChanged += (s, e) => lineNumberPanel.Invalidate();
            codeEditor.VScroll += (s, e) => lineNumberPanel.Invalidate();
            codeEditor.Resize += (s, e) => lineNumberPanel.Invalidate();
            codeEditor.Leave += (s, e) => ApplySyntaxHighlighting();

            lineNumberPanel.Paint += (s, e) =>
            {
                int firstIndex = codeEditor.GetCharIndexFromPosition(new Point(0, 0));
                int firstLine = codeEditor.GetLineFromCharIndex(firstIndex);

                int lastIndex = codeEditor.GetCharIndexFromPosition(new Point(0, codeEditor.ClientSize.Height - 1));
                int lastLine = codeEditor.GetLineFromCharIndex(lastIndex);

                for (int i = firstLine; i <= lastLine + 1 && i < codeEditor.Lines.Length; i++)
                {
                    int y = codeEditor.GetPositionFromCharIndex(codeEditor.GetFirstCharIndexFromLine(i)).Y;
                    e.Graphics.DrawString((i + 1).ToString(), codeEditor.Font, Brushes.Gray, 0, y);
                }
            };
            #endregion

            #region Panel derecho - Canvas y consola
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 85f));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));
            mainSplit.Panel2.Controls.Add(rightPanel);

            // Scroll para el canvas
            var canvasScroll = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            pixelCanvas = new PixelCanvas();
            canvasScroll.Controls.Add(pixelCanvas);
            rightPanel.Controls.Add(canvasScroll, 0, 0);

            // Botones de zoom y centrado
            var buttonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill
            };

            var zoomInButton = new Button { Text = "Zoom +" };
            var zoomOutButton = new Button { Text = "Zoom -" };
            var centerButton = new Button { Text = "Center" };

            zoomInButton.Click += (s, e) => pixelCanvas.ZoomIn();
            zoomOutButton.Click += (s, e) => pixelCanvas.ZoomOut();
            centerButton.Click += (s, e) => pixelCanvas.CenterOnCursor(canvasScroll);

            buttonsPanel.Controls.Add(zoomInButton);
            buttonsPanel.Controls.Add(zoomOutButton);
            buttonsPanel.Controls.Add(centerButton);
            rightPanel.Controls.Add(buttonsPanel, 0, 1);

            // Consola de errores
            wall_EConsole = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.LightYellow,
                ForeColor = Color.DarkRed,
                Font = new Font("Consolas", 9)
            };
            rightPanel.Controls.Add(wall_EConsole, 0, 2);
            #endregion

            #region SmartEditorHelper: autocompletado y sugerencias
            var colors = new List<string> { "Red", "Green", "Blue", "Black", "White", "Yellow", "Gray", "Purple", "Cyan" };

            // Inicializar SuggestionPopup
            SuggestionPopup suggestionPopup = new SuggestionPopup();
            var keywords = new[] { "Spawn", "Color", "Size", "DrawLine", "DrawCircle", "DrawRectangle", "Fill", "GoTo" };
            SmartEditorHelper helper = new SmartEditorHelper(codeEditor, ghostEditor, suggestionPopup, keywords, colors);
            #endregion
        }


        #endregion

        #region CodeExecution

        /// <summary>
        /// Ejecuta el código del editor, muestra errores y actualiza el canvas.
        /// </summary>
        private void RunCode_Click(object sender, EventArgs e)
        {
            wall_EConsole.Clear();
            pixelCanvas.Clear();
            ErrorManager.Clear();

            try
            {
                string input = codeEditor.Text;
                var lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();

                // Muestra los tokens generados (para depuración)
                // foreach (var t in tokens)
                //     MessageBox.Show($"{t.Type} '{t.Lexeme}' línea {t.Line}");

                var errors = new List<string>();
                var parser = new Parser(tokens, errors);
                var codes = parser.Parse(errors);

                // Muestra los errores de análisis directamente
                if (errors.Count > 0)
                {
                    wall_EConsole.Text = string.Join(Environment.NewLine, errors);
                    return;
                }

                var executor = new Executor(pixelCanvas, errors);
                executor.Execute(codes);

                // Muestra los errores de ejecución si los hay
                if (errors.Count > 0)
                    wall_EConsole.Text = string.Join(Environment.NewLine, errors);
            }
            catch (RuntimeError ex)
            {
                wall_EConsole.Text = $"[Runtime Error] Línea {ex.Line}: {ex.Message}";
            }
            catch (Exception ex)
            {
                wall_EConsole.Text = $"[Internal Error] {ex.Message}";
            }
        }

        #endregion

        #region FileManagement

        /// <summary>
        /// Abre un archivo y carga su contenido en el editor.
        /// </summary>
        private void OpenFile(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "WLE Files (*.wle)|*.wle|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                codeEditor.Text = File.ReadAllText(dialog.FileName);
        }

        /// <summary>
        /// Guarda el contenido del editor en un archivo.
        /// </summary>
        private void SaveFile(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "WLE Files (*.wle)|*.wle|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dialog.FileName, codeEditor.Text);
        }

        #endregion

        #region FormEvents

        /// <summary>
        /// Ajusta la posición del splitter al cargar el formulario.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mainSplit.SplitterDistance = (int)(ClientSize.Width * 0.55);
        }

        #endregion

        #region SyntaxHighlighting

        /// <summary>
        /// Aplica resaltado de sintaxis al editor de código.
        /// </summary>
        private void ApplySyntaxHighlighting()
        {
            int selStart = codeEditor.SelectionStart;
            int selLength = codeEditor.SelectionLength;

            codeEditor.SuspendLayout();

            // Guarda el color original de fondo y texto
            Color defaultColor = Color.Black;

            // Aplica color por defecto solo si es necesario
            codeEditor.Select(0, codeEditor.Text.Length);
            codeEditor.SelectionColor = defaultColor;

            string text = codeEditor.Text;

            // Palabras clave del lenguaje.
            string[] keywords = { "Spawn", "Color", "Size", "DrawLine", "DrawCircle", "DrawRectangle", "Fill", "Goto" };
            foreach (string word in keywords) HighlightWord(word, Color.Blue);

            // Nombres de colores conocidos.
            string[] colorWords = { "Red", "Green", "Blue", "Black", "White", "Gray", "Yellow", "Cyan", "Magenta" };
            foreach (string color in colorWords) HighlightWord(color, Color.DarkGreen);

            // Números.
            foreach (Match m in Regex.Matches(text, @"\b\d+\b"))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.DarkCyan;
            }

            // Cadenas de texto.
            foreach (Match m in Regex.Matches(text, "\"[^\"]*\""))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.Brown;
            }

            // Comentarios.
            foreach (Match m in Regex.Matches(text, @"#.*$", RegexOptions.Multiline))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.DarkGray;
            }

            // Restaura la selección original
            codeEditor.Select(selStart, selLength);
            codeEditor.SelectionColor = defaultColor;

            codeEditor.ResumeLayout();
        }

        /// <summary>
        /// Resalta una palabra específica en el editor con el color dado.
        /// </summary>
        private void HighlightWord(string word, Color color)
        {
            foreach (Match match in Regex.Matches(codeEditor.Text, $@"\b{Regex.Escape(word)}\b"))
            {
                codeEditor.Select(match.Index, match.Length);
                codeEditor.SelectionColor = color;
            }
        }

        #endregion
    }
}