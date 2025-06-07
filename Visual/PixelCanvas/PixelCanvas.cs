using System;
using System.Drawing;
using System.Windows.Forms;

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
        // Matriz que almacena el color de cada píxel (como ARGB).
        private readonly int[,] _pixels;
        // Número de columnas del canvas.
        private readonly int _cols;
        // Número de filas del canvas.
        private readonly int _rows;
        // Tamaño actual del pincel.
        private int _brushSize = 1;
        // Color actual del pincel.
        private Color _brushColor = Color.Black;
        // Posición actual del cursor.
        private Point _cursorPosition;
        // Indica si el cursor debe mostrarse (para parpadeo).
        private bool _showCursor = true;
        // Timer para el parpadeo del cursor.
        private Timer _cursorTimer;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el canvas con el número de columnas y filas especificado.
        /// </summary>
        /// <param name="cols">Columnas del canvas.</param>
        /// <param name="rows">Filas del canvas.</param>
        public PixelCanvas(int cols = 128, int rows = 128)
        {
            _cols = cols;
            _rows = rows;
            _pixels = new int[cols, rows];

            Width = cols * _pixelSize;
            Height = rows * _pixelSize;
            DoubleBuffered = true;
            BackColor = Color.White;

            _cursorPosition = new Point(cols / 2, rows / 2);

            // Timer para cursor intermitente
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
        /// Obtiene el tamaño actual del pincel.
        /// </summary>
        public int GetBrushSize() => _brushSize;

        /// <summary>
        /// Obtiene la coordenada X del cursor.
        /// </summary>
        public int GetCursorX() => _cursorPosition.X;

        /// <summary>
        /// Obtiene la coordenada Y del cursor.
        /// </summary>
        public int GetCursorY() => _cursorPosition.Y;

        /// <summary>
        /// Obtiene el color del píxel en la posición dada.
        /// Lanza excepción si está fuera de los límites.
        /// </summary>
        public Color GetPixelColor(int x, int y, int line)
        {
            if (x < 0 || x >= _cols || y < 0 || y >= _rows)
                throw new CanvasOutOfBoundsError(x, y, _cols, _rows, line);

            int colorIndex = _pixels[y, x];
            return Color.FromArgb(colorIndex);
        }

        /// <summary>
        /// Establece el color actual del pincel.
        /// </summary>
        public void SetColor(Color color) => _brushColor = color;

        /// <summary>
        /// Obtiene el color actual del pincel.
        /// </summary>
        public Color GetCurrentColor() => _brushColor;

        /// <summary>
        /// Establece el color actual del pincel.
        /// </summary>
        public void SetBrushColor(Color color) => _brushColor = color;

        /// <summary>
        /// Establece el tamaño actual del pincel.
        /// </summary>
        public void SetBrushSize(int size) => _brushSize = size;

        /// <summary>
        /// Obtiene la coordenada X del cursor (propiedad).
        /// </summary>
        public int CursorX => _cursorPosition.X;

        /// <summary>
        /// Obtiene la coordenada Y del cursor (propiedad).
        /// </summary>
        public int CursorY => _cursorPosition.Y;

        /// <summary>
        /// Obtiene el número de columnas del canvas.
        /// </summary>
        public int Cols => _cols;

        /// <summary>
        /// Obtiene el número de filas del canvas.
        /// </summary>
        public int Rows => _rows;

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Dibuja una línea desde la posición actual del cursor en la dirección y distancia dadas.
        /// </summary>
        public void DrawLine(int dx, int dy, int distance)
        {
            int x = _cursorPosition.X;
            int y = _cursorPosition.Y;

            for (int i = 0; i < distance; i++)
            {
                DrawPoint(x, y);
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
        public void Spawn(int x, int y)
        {
            if (!IsInBounds(x, y))
                throw new CanvasOutOfBoundsError(x, y, _cols, _rows, _pixelSize);

            _cursorPosition = new Point(x, y);
            Invalidate();
        }

        /// <summary>
        /// Aumenta el tamaño de los píxeles (zoom in).
        /// </summary>
        public void ZoomIn()
        {
            _pixelSize = Math.Min(_pixelSize + 2, 60);
            ResizeCanvas();
        }

        /// <summary>
        /// Disminuye el tamaño de los píxeles (zoom out).
        /// </summary>
        public void ZoomOut()
        {
            _pixelSize = Math.Max(_pixelSize - 2, 2);
            ResizeCanvas();
        }

        /// <summary>
        /// Ajusta el tamaño del canvas visual según el tamaño de los píxeles.
        /// </summary>
        private void ResizeCanvas()
        {
            Width = _cols * _pixelSize;
            Height = _rows * _pixelSize;
            Invalidate();
        }

        /// <summary>
        /// Centra el área visible del panel en la posición del cursor.
        /// </summary>
        public void CenterOnCursor(Panel scrollPanel)
        {
            int scrollX = _cursorPosition.X * _pixelSize - scrollPanel.ClientSize.Width / 2;
            int scrollY = _cursorPosition.Y * _pixelSize - scrollPanel.ClientSize.Height / 2;

            scrollPanel.AutoScrollPosition = new Point(scrollX, scrollY);
        }

        /// <summary>
        /// Dibuja un punto en la posición dada usando el color y tamaño actuales.
        /// </summary>
        private void DrawPoint(int cx, int cy)
        {
            DrawPoint(cx, cy, _brushColor, _brushSize);
        }

        /// <summary>
        /// Dibuja un punto en la posición dada con color y tamaño específicos.
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