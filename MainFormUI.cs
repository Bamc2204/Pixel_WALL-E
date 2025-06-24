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
            #region MainWindowConfiguration

            // Configuración de la ventana principal
            Text = "Pixel Wall-E INTERPRETE";
            Width = 1400;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;

            #endregion

            #region MainSplit

            // División principal en dos paneles (izquierdo y derecho)
            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,                                  // Ocupa todo el espacio disponible del contenedor padre.
                Orientation = Orientation.Vertical                      // Divide los paneles verticalmente (izquierda/derecha).

            };
            Controls.Add(mainSplit);

            #endregion

            #region LeftPanel - CodeEditor and Buttons

            // Panel izquierdo: contiene botones y el editor de código
            var editorPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,                                  // Ocupa todo el espacio disponible en Panel1.
                RowCount = 2,                                           // Dos filas.
                ColumnCount = 1                                         // Una sola columna.
            };
            // Fila para botones
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            // Fila para el editor
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            // Agregar editorPanel al panel izquierdo
            mainSplit.Panel1.Controls.Add(editorPanel);

            // Botones de archivo y ejecución
            var fileButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,              // Botones en horizontal (izq → der).
                Dock = DockStyle.Fill                                   // Ocupa toda la fila asignada.
            };

            // Creación de botones
            var openButton = new Button { Text = "Open" };
            var saveButton = new Button { Text = "Save" };
            var undoButton = new Button { Text = "Undo" };
            var redoButton = new Button { Text = "Redo" };
            var runButton = new Button { Text = "Run Code" };

            // Agrega las funciones a los botones
            openButton.Click += OpenFile;
            saveButton.Click += SaveFile;
            undoButton.Click += (s, e) => codeEditor.Undo();
            redoButton.Click += (s, e) => codeEditor.Redo();
            runButton.Click += RunCode_Click;

            // Agrega los botones al fileButtons 
            fileButtons.Controls.Add(openButton);
            fileButtons.Controls.Add(saveButton);
            fileButtons.Controls.Add(undoButton);
            fileButtons.Controls.Add(redoButton);
            fileButtons.Controls.Add(runButton);

            // Agregar fileButtons a la primera fila del TableLayoutPanel.
            editorPanel.Controls.Add(fileButtons, 0, 0);

            #endregion

            #region MainEditor, Ghost and Suggestions

            // Panel de números de línea
            lineNumberPanel = new Panel
            {
                Width = 40,                                             // Ancho fijo de 40 píxeles.
                Dock = DockStyle.Left,                                  // Se ancla a la izquierda del contenedor padre.
                BackColor = Color.LightGray                             // Fondo gris claro.
            };

            // Editor de código principal
            codeEditor = new RichTextBox
            {
                Font = new Font("Consolas", 10),                        // Usa fuente monoespaciada (ideal para código).
                Dock = DockStyle.Fill,                                  // Ocupa todo el espacio restante.
                Multiline = true,                                       // Permite múltiples líneas.
                ScrollBars = RichTextBoxScrollBars.Vertical,            // Barra de desplazamiento vertica
                AcceptsTab = true,                                      // Permite usar la tecla TAB para indentación.
                BorderStyle = BorderStyle.None                          // Sin borde (estilo minimalista).
            };

            // Editor fantasma para mostrar sugerencias en gris claro
            ghostEditor = new RichTextBox
            {
                Font = codeEditor.Font,                                 // Misma fuente que el editor principal.
                Dock = DockStyle.Fill,                                  // Ocupa todo el espacio.
                Multiline = true,                                       // Permite múltiples líneas (como un editor de código real).
                ReadOnly = true,                                        // No editable.
                BackColor = Color.White,                                // Fondo blanco.
                ForeColor = Color.LightGray,                            // Texto gris claro (para sugerencias).
                BorderStyle = BorderStyle.None,                         // Sin bordes (estilo minimalista).
                Enabled = false,                                        // No interactuable.
                TabStop = false,                                        // No recibe foco con TAB.
                ScrollBars = RichTextBoxScrollBars.None,                // Sin barras de desplazamiento.
                WordWrap = codeEditor.WordWrap,                         // Mismo ajuste de línea que el editor principal.
                Location = codeEditor.Location,                         // Misma posición.
                Text = ""                                               // Vacío inicialmente.
            };

            // Superposición de ghostEditor y codeEditor
            var editorOverlay = new Panel { Dock = DockStyle.Fill };    // Panel contenedor.
            editorOverlay.Controls.Add(ghostEditor);                    // Agrega el editor fantasma.
            editorOverlay.Controls.Add(codeEditor);                     // Agrega el editor principal.
            codeEditor.BringToFront();                                  // Asegura que el editor principal esté encima.
            ghostEditor.SendToBack();                                   // El fantasma queda detrás.

            // ListBox para mostrar sugerencias
            suggestionBox = new ListBox
            {
                Visible = false,                                        // Oculto inicialmente.
                Font = codeEditor.Font,                                 // Misma fuente.
                Height = 100,                                           // Altura fija.
                Width = 200                                             // Ancho fijo.
            };

            // Contenedor final del editor, panel de línea y sugerencias
            var editorContainer = new Panel { Dock = DockStyle.Fill };  // Panel principal.
            editorContainer.Controls.Add(editorOverlay);                // Agrega la superposición de editores.
            editorContainer.Controls.Add(lineNumberPanel);              // Agrega el panel de números.
            editorContainer.Controls.Add(suggestionBox);                // Agrega el ListBox de sugerencias.

            // Integración con el TableLayoutPanel Padre
            editorPanel.Controls.Add(editorContainer, 0, 1);            // Columna 0, Fila 1.

            #endregion

            #region CodeEditorEvents

            // Eventos para actualizar los números de línea
            // Cada vez que el texto del editor (codeEditor) cambia (se escribe, borra, etc.), se fuerza un redibujado del panel de números (lineNumberPanel).
            codeEditor.TextChanged += (s, e) => lineNumberPanel.Invalidate();
            // Cuando el usuario desplaza el editor verticalmente (con la rueda del mouse o la barra de desplazamiento), se actualizan los números de línea.
            codeEditor.VScroll += (s, e) => lineNumberPanel.Invalidate();
            // Si el editor cambia de tamaño (por redimensión de la ventana), se vuelven a dibujar los números para ajustarse al nuevo espacio visible.
            codeEditor.Resize += (s, e) => lineNumberPanel.Invalidate();
            // Cuando el editor pierde el foco (el usuario hace clic en otro control), se aplica el resaltado de sintaxis (ApplySyntaxHighlighting).
            codeEditor.Leave += (s, e) => ApplySyntaxHighlighting();

            // Evento Paint para dibujar los números de línea a la izquierda
            lineNumberPanel.Paint += (s, e) =>
            {
                // Obtiene el índice del primer carácter visible en la parte superior del editor.
                int firstIndex = codeEditor.GetCharIndexFromPosition(new Point(0, 0));
                // Convierte ese índice en un número de línea.
                int firstLine = codeEditor.GetLineFromCharIndex(firstIndex);

                // Obtiene el índice del último carácter visible (en la parte inferior del editor).
                int lastIndex = codeEditor.GetCharIndexFromPosition(new Point(0, codeEditor.ClientSize.Height - 1));
                // Convierte ese índice en un número de línea.
                int lastLine = codeEditor.GetLineFromCharIndex(lastIndex);

                // Itera desde la primera línea visible hasta la última línea visible (+1 para cubrir bordes).
                for (int i = firstLine; i <= lastLine + 1 && i < codeEditor.Lines.Length; i++)
                {
                    // Obtiene la coordenada Y (vertical) donde debe dibujarse el número de línea.
                    int y = codeEditor.GetPositionFromCharIndex(codeEditor.GetFirstCharIndexFromLine(i)).Y;
                    // Dibuja el número (i + 1 porque las líneas empiezan en 0) en gris.
                    e.Graphics.DrawString((i + 1).ToString(), codeEditor.Font, Brushes.Gray, 0, y);
                }
            };

            #endregion

            #region RightPanel - Canvas and Console

            // Panel derecho: contiene el canvas y consola
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,                                  // Ocupa todo el espacio disponible en Panel2 del SplitContainer.
                RowCount = 3,                                           // Tres filas: canvas, botones y consola.
                ColumnCount = 1                                         // Una sola columna.
            };
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 85f));  // Canvas (85% del espacio).
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f)); // Botones (altura fija: 40px).
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));  // Consola (15% restante).
            mainSplit.Panel2.Controls.Add(rightPanel);                      // Aquí se coloca el rightPanel con todo su contenido.

            // Panel con scroll que contiene el canvas
            var canvasScroll = new Panel
            {
                AutoScroll = true,                                      // Habilita barras de desplazamiento si el contenido es grande.
                Dock = DockStyle.Fill,                                  // Ocupa toda el área asignada (fila 0 del rightPanel).
                BackColor = Color.LightGray                             // Fondo gris claro.
            };

            pixelCanvas = new PixelCanvas();                            // Lienzo personalizado para dibujar.
            canvasScroll.Controls.Add(pixelCanvas);                     // Añade el canvas al panel con scroll.
            rightPanel.Controls.Add(canvasScroll, 0, 0);                // Posición: fila 0, columna 0.            

            // Botones de control para el canvas
            var buttonsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,              // Botones en fila horizontal.
                Dock = DockStyle.Fill                                   // Ocupa toda la segunda fila (40px).
            };

            // Botones
            var zoomInButton = new Button { Text = "Zoom +" };
            var zoomOutButton = new Button { Text = "Zoom -" };
            var centerButton = new Button { Text = "Center" };

            // Asignar funciones a los botones
            zoomInButton.Click += (s, e) => pixelCanvas.ZoomIn();
            zoomOutButton.Click += (s, e) => pixelCanvas.ZoomOut();
            centerButton.Click += (s, e) => pixelCanvas.CenterOnCursor(canvasScroll);

            // Añadir botones al Panel
            buttonsPanel.Controls.Add(zoomInButton);
            buttonsPanel.Controls.Add(zoomOutButton);
            buttonsPanel.Controls.Add(centerButton);
            rightPanel.Controls.Add(buttonsPanel, 0, 1);

            // Consola inferior para mensajes y errores
            wall_EConsole = new TextBox
            {
                Multiline = true,                                       // Permite múltiples líneas.
                ReadOnly = true,                                        // No editable (solo lectura).
                Dock = DockStyle.Fill,                                  // Ocupa toda la tercera fila (15%).
                ScrollBars = ScrollBars.Vertical,                       // Barra de desplazamiento vertical.
                BackColor = Color.LightYellow,                          // Fondo amarillo claro.
                ForeColor = Color.DarkRed,                              // Texto en rojo oscuro.
                Font = new Font("Consolas", 9)                          // Fuente monoespaciada (ideal para logs)
            };
            rightPanel.Controls.Add(wall_EConsole, 0, 2);               // Fila 2, columna 0.

            #endregion

            #region SmartEditorHelper: Autocomplete and Suggestions

            // Paleta de colores permitidos
            var colors = new List<string> { "\"Red\"", "\"Green\"", "\"Blue\"", "\"Black\"", "\"White\"", "\"Yellow\"", "\"Gray\"", "\"Purple\"", "\"Cyan\"", "\"Transparent\"" };

            // Inicialización del autocompletado inteligente
            SuggestionPopup suggestionPopup = new SuggestionPopup();
            var keywords = new[]
            {
                "Spawn", "Color", "Size", "DrawLine", "DrawCircle", "DrawRectangle",
                "Fill", "GoTo", "GetActualX", "GetActualY", "GetCanvasSize",
                "GetColorCount", "IsBrushColor", "IsBrushSize", "IsCanvasColor"
            };

            // Configuración del asistente inteligente
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
                Filter = "WLE Files (*.pw)|*.pw|All Files (*.*)|*.*"
            };
            // Muestra un cuadro de diálogo para abrir un archivo. Si el usuario pulsa "Aceptar"
            if (dialog.ShowDialog() == DialogResult.OK)
                // Entonces, lee todo el contenido del archivo seleccionado y lo muestra en el editor de texto.
                codeEditor.Text = File.ReadAllText(dialog.FileName);

        }

        /// <summary>
        /// Guarda el contenido del editor en un archivo.
        /// </summary>
        private void SaveFile(object sender, EventArgs e)
        {
            // Crea un diálogo para guardar un archivo con el filtro "WLE Files (*.pw)|*.pw|Todos los archivos (*.*)|*.*"
            using var dialog = new SaveFileDialog
            {
                Filter = "WLE Files (*.pw)|*.pw|All Files (*.*)|*.*"
            };

            // Genera nombre por defecto tipo "Wall_E"
            string fileName = "Wall_E";

            // Asigna el nombre por defecto al diálogo
            dialog.FileName = Path.GetFileName(fileName);

            // Si el usuario selecciona una ubicación y pulsa "Guardar":
            if (dialog.ShowDialog() == DialogResult.OK)
                // Escribe el texto actual del editor (codeEditor.Text) en el archivo seleccionado.
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