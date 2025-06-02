using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
#nullable enable

namespace Wall_E
{
    public class MainForm : Form
    {
        private RichTextBox codeEditor = null!;
        private PixelCanvas pixelCanvas = null!;
        private TextBox errorBox = null!;
        private SplitContainer mainSplit = null!;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Pixel Wall-E IDE";
            Width = 1400;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;

            // === División principal ===
            mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };
            Controls.Add(mainSplit);

            // === Panel izquierdo (editor + botones) ===
            var editorPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            editorPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            mainSplit.Panel1.Controls.Add(editorPanel);

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
            undoButton.Click += (s, e) => { if (codeEditor.CanUndo) codeEditor.Undo(); };
            redoButton.Click += (s, e) => { if (codeEditor.CanRedo) codeEditor.Redo(); };
            runButton.Click += RunCode_Click;

            fileButtons.Controls.Add(openButton);
            fileButtons.Controls.Add(saveButton);
            fileButtons.Controls.Add(undoButton);
            fileButtons.Controls.Add(redoButton);
            fileButtons.Controls.Add(runButton);
            editorPanel.Controls.Add(fileButtons, 0, 0);

            codeEditor = new RichTextBox
            {
                Font = new Font("Consolas", 10),
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                AcceptsTab = true
            };
            codeEditor.TextChanged += (s, e) => ApplySyntaxHighlighting();
            editorPanel.Controls.Add(codeEditor, 0, 1);

            codeEditor.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Z)
                {
                    codeEditor.Undo();
                    e.SuppressKeyPress = true;
                }
                else if (e.Control && e.KeyCode == Keys.Y)
                {
                    codeEditor.Redo();
                    e.SuppressKeyPress = true;
                }
            };


            // === Panel derecho ===
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

            var canvasScroll = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            pixelCanvas = new PixelCanvas(200, 200);
            canvasScroll.Controls.Add(pixelCanvas);
            rightPanel.Controls.Add(canvasScroll, 0, 0);

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

            errorBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.LightYellow,
                ForeColor = Color.DarkRed,
                Font = new Font("Consolas", 9)
            };
            rightPanel.Controls.Add(errorBox, 0, 2);
        }

        private void RunCode_Click(object? sender, EventArgs e)
        {
            errorBox.Clear();
            pixelCanvas.Clear();

            try
            {
                string input = codeEditor.Text;
                Lexer lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();

                Parser parser = new Parser(tokens);
                List<ICode> codes = parser.Parse();

                Executor executor = new Executor(pixelCanvas);
                executor.Execute(codes);
            }
            catch (RuntimeError ex)
            {
                errorBox.Text = $"[Runtime Error] Line {ex.Line}: {ex.Message}";
            }
            catch (Exception ex)
            {
                errorBox.Text = $"[Internal Error] {ex.Message}";
            }
        }

        private void OpenFile(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "WLE Files (*.wle)|*.wle|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                codeEditor.Text = File.ReadAllText(dialog.FileName);
            }
        }

        private void SaveFile(object? sender, EventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "WLE Files (*.wle)|*.wle|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, codeEditor.Text);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mainSplit.SplitterDistance = (int)(ClientSize.Width * 0.55);
        }

        #region Syntax Highlighting

        private void ApplySyntaxHighlighting()
        {
            int selectionStart = codeEditor.SelectionStart;
            int selectionLength = codeEditor.SelectionLength;

            codeEditor.SuspendLayout();

            string text = codeEditor.Text;
            codeEditor.SelectAll();
            codeEditor.SelectionColor = Color.Black;

            // Keywords
            string[] keywords = { "Spawn", "Color", "Size", "DrawLine", "DrawCircle", "DrawRectangle", "Fill", "Goto" };
            foreach (string kw in keywords) HighlightWord(kw, Color.Blue);

            // Colors
            string[] colorNames = { "Red", "Green", "Blue", "Black", "White", "Gray", "Yellow", "Cyan", "Magenta" };
            foreach (string color in colorNames) HighlightWord(color, Color.DarkGreen);

            // Numbers
            foreach (Match m in Regex.Matches(text, @"\b\d+\b"))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.DarkCyan;
            }

            // Strings
            foreach (Match m in Regex.Matches(text, "\"[^\"]*\""))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.Brown;
            }

            // Comments (opcional si usas #)
            foreach (Match m in Regex.Matches(text, @"#.*$", RegexOptions.Multiline))
            {
                codeEditor.Select(m.Index, m.Length);
                codeEditor.SelectionColor = Color.DarkGray;
            }

            codeEditor.SelectionStart = selectionStart;
            codeEditor.SelectionLength = selectionLength;
            codeEditor.SelectionColor = Color.Black;

            codeEditor.ResumeLayout();
        }

        private void HighlightWord(string word, Color color)
        {
            foreach (Match match in Regex.Matches(codeEditor.Text, $@"\b{word}\b"))
            {
                codeEditor.Select(match.Index, match.Length);
                codeEditor.SelectionColor = color;
            }
        }

        #endregion
    }
}
