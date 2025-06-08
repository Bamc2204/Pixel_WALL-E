using System;
using System.Drawing;
using System.Windows.Forms;
#nullable enable

namespace Wall_E
{
    /// <summary>
    /// Clase que representa un canvas de píxeles para dibujar en pantalla.
    /// Permite operaciones como dibujar líneas, círculos, rectángulos, rellenar, mover el cursor, etc.
    /// </summary>
    public class PixelCanvas : Panel
    {
        #region Fields

        // Tamaño de cada píxel en pantalla.
        private int _pixelSize = 5;
        // Matriz de colores de los píxeles.
        private readonly int[,] _pixels;
        // Número de columnas y filas del canvas.
        private readonly int _cols;
        private readonly int _rows;
        // Tamaño actual del pincel.
        private int _brushSize = 1;
        // Color actual del pincel.
        private Color _brushColor = Color.Black;
        // Posición actual del cursor en el canvas.
        private Point _cursorPosition;
        // Controla si se muestra el cursor.
        private bool _showCursor = true;
        // Timer para parpadeo del cursor.
        private Timer _cursorTimer;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el canvas con el número de columnas y filas especificado.
        /// </summary>
        public PixelCanvas(int cols = 128, int rows = 128)
        {
            _cols = cols;
            _rows = rows;
            _pixels = new int[cols, rows];

            Width = cols * _pixelSize;
            Height = rows * _pixelSize;
            DoubleBuffered = true;
            BackColor = Color.White;

            // Inicializa el cursor en el centro del canvas.
            _cursorPosition = new Point(cols / 2, rows / 2);

            // Configura el timer para el parpadeo del cursor.
            _cursorTimer = new Timer { Interval = 500 };
            _cursorTimer.Tick += (s, e) =>
            {
                _showCursor = !_showCursor;
                Invalidate();
            };
            _cursorTimer.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Devuelve el tamaño actual del pincel.
        /// </summary>
        public int GetBrushSize() => _brushSize;

        /// <summary>
        /// Devuelve la posición X del cursor.
        /// </summary>
        public int GetCursorX() => _cursorPosition.X;

        /// <summary>
        /// Devuelve la posición Y del cursor.
        /// </summary>
        public int GetCursorY() => _cursorPosition.Y;

        /// <summary>
        /// Devuelve el color del píxel en la posición indicada.
        /// </summary>
        public Color GetPixelColor(int x, int y, int line)
        {
            if (x < 0 || x >= _cols || y < 0 || y >= _rows)
                throw new CanvasOutOfBoundsError(x, y, _cols, _rows, line);

            int colorIndex = _pixels[x, y];
            return Color.FromArgb(colorIndex);
        }

        /// <summary>
        /// Cambia el color actual del pincel.
        /// </summary>
        public void SetColor(Color color) => _brushColor = color;

        /// <summary>
        /// Devuelve el color actual del pincel.
        /// </summary>
        public Color GetCurrentColor() => _brushColor;

        /// <summary>
        /// Cambia el color actual del pincel.
        /// </summary>
        public void SetBrushColor(Color color) => _brushColor = color;

        /// <summary>
        /// Cambia el tamaño actual del pincel.
        /// </summary>
        public void SetBrushSize(int size) => _brushSize = size;

        /// <summary>
        /// Devuelve la posición X del cursor.
        /// </summary>
        public int CursorX => _cursorPosition.X;

        /// <summary>
        /// Devuelve la posición Y del cursor.
        /// </summary>
        public int CursorY => _cursorPosition.Y;

        /// <summary>
        /// Devuelve el número de columnas del canvas.
        /// </summary>
        public int Cols => _cols;

        /// <summary>
        /// Devuelve el número de filas del canvas.
        /// </summary>
        public int Rows => _rows;

        /// <summary>
        /// Devuelve el ancho total del canvas en píxeles.
        /// </summary>
        public int WidthInPixels => _cols * _pixelSize;

        /// <summary>
        /// Devuelve el alto total del canvas en píxeles.
        /// </summary>
        public int HeightInPixels => _rows * _pixelSize;

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Dibuja una línea desde la posición actual del cursor en la dirección y distancia indicadas.
        /// </summary>
        public void DrawLine(int dx, int dy, int distance, Color color, int size)
        {
            int x = _cursorPosition.X;
            int y = _cursorPosition.Y;

            for (int i = 0; i < distance; i++)
            {
                DrawPoint(x, y, color, size);
                x += dx;
                y += dy;
            }

            _cursorPosition = new Point(x, y);
            Invalidate();
        }
        
        /// <summary>
        /// Dibuja un círculo desde la posición actual del cursor.
        /// </summary>
        public void DrawCircle(int radius, Color color, int size)
        {
            int cx = _cursorPosition.X;
            int cy = _cursorPosition.Y;

            for (int angle = 0; angle < 360; angle++)
            {
                double rad = angle * Math.PI / 180;
                int x = cx + (int)(radius * Math.Cos(rad));
                int y = cy + (int)(radius * Math.Sin(rad));
                DrawPoint(x, y, color, size);
            }

            Invalidate();
        }

        /// <summary>
        /// Dibuja un rectángulo desde la posición actual del cursor.
        /// </summary>
        public void DrawRectangle(int width, int height, Color color, int size)
        {
            int x = _cursorPosition.X;
            int y = _cursorPosition.Y;

            for (int i = 0; i < width; i++)
            {
                DrawPoint(x + i, y, color, size);
                DrawPoint(x + i, y + height - 1, color, size);
            }

            for (int j = 0; j < height; j++)
            {
                DrawPoint(x, y + j, color, size);
                DrawPoint(x + width - 1, y + j, color, size);
            }

            Invalidate();
        }

        /// <summary>
        /// Rellena todo el canvas con el color especificado.
        /// </summary>
        public void Fill(Color color)
        {
            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _pixels[x, y] = color.ToArgb();

            Invalidate();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Limpia el canvas y lo deja en blanco.
        /// </summary>
        public void Clear()
        {
            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _pixels[x, y] = Color.White.ToArgb();

            Invalidate();
        }

        /// <summary>
        /// Verifica si una posición está dentro de los límites del canvas.
        /// </summary>
        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < _cols && y >= 0 && y < _rows;
        }

        /// <summary>
        /// Mueve el cursor a una posición específica.
        /// </summary>
        public void SetCursorPosition(int x, int y)
        {
            if (!IsInBounds(x, y))
                throw new CanvasOutOfBoundsError(x, y, _cols, _rows, _pixelSize);

            _cursorPosition = new Point(x, y);
            Invalidate();
        }

        /// <summary>
        /// Centra el scroll del panel en la posición actual del cursor.
        /// </summary>
        public void CenterOnCursor(Panel scrollPanel)
        {
            int scrollX = _cursorPosition.X * _pixelSize - scrollPanel.ClientSize.Width / 2;
            int scrollY = _cursorPosition.Y * _pixelSize - scrollPanel.ClientSize.Height / 2;

            scrollPanel.AutoScrollPosition = new Point(scrollX, scrollY);
        }

        /// <summary>
        /// Dibuja un punto en la posición indicada usando el color y tamaño actuales.
        /// </summary>
        private void DrawPoint(int cx, int cy)
        {
            DrawPoint(cx, cy, _brushColor, _brushSize);
        }

        /// <summary>
        /// Dibuja un punto en la posición indicada usando el color y tamaño especificados.
        /// </summary>
        private void DrawPoint(int cx, int cy, Color color, int size)
        {
            for (int dx = -size / 2; dx <= size / 2; dx++)
            {
                for (int dy = -size / 2; dy <= size / 2; dy++)
                {
                    int x = cx + dx;
                    int y = cy + dy;

                    if (!IsInBounds(x, y))
                        throw new CanvasOutOfBoundsError(x, y, _cols, _rows, _pixelSize);

                    _pixels[x, y] = color.ToArgb();
                }
            }
        }

        /// <summary>
        /// Aumenta el tamaño de los píxeles (zoom in).
        /// </summary>
        public void ZoomIn()
        {
            if (_pixelSize < 40)
            {
                _pixelSize += 2;
                Width = _cols * _pixelSize;
                Height = _rows * _pixelSize;
                Invalidate();
            }
        }

        /// <summary>
        /// Disminuye el tamaño de los píxeles (zoom out).
        /// </summary>
        public void ZoomOut()
        {
            if (_pixelSize > 2)
            {
                _pixelSize -= 2;
                Width = _cols * _pixelSize;
                Height = _rows * _pixelSize;
                Invalidate();
            }
        }

        #endregion

        #region Paint

        /// <summary>
        /// Dibuja el canvas y el cursor en pantalla.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    // Obtiene el color del píxel y lo dibuja.
                    Color color = Color.FromArgb(_pixels[x, y]);
                    using Brush brush = new SolidBrush(color);
                    g.FillRectangle(brush, x * _pixelSize, y * _pixelSize, _pixelSize, _pixelSize);
                    g.DrawRectangle(Pens.LightGray, x * _pixelSize, y * _pixelSize, _pixelSize, _pixelSize);
                }
            }

            // Dibuja el cursor si corresponde.
            if (_showCursor)
            {
                int cx = _cursorPosition.X * _pixelSize;
                int cy = _cursorPosition.Y * _pixelSize;
                using Pen pen = new Pen(Color.Gray, 2);
                g.DrawRectangle(pen, cx, cy, _pixelSize, _pixelSize);
            }
        }

        #endregion
    }
}