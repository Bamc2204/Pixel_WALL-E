using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    /// <summary>
    /// Panel personalizado para dibujar píxeles, líneas, círculos y otras figuras.
    /// </summary>
    public class PixelCanvas : Panel
    {
        #region Fields
        // Tamaño de cada píxel en pantalla
        private const int PixelSize = 20;
        // Matriz que almacena el color de cada píxel
        private readonly int[,] _pixels;
        // Número de columnas del canvas
        private readonly int _cols;
        // Número de filas del canvas
        private readonly int _rows;

        // Tamaño actual del pincel
        private int _brushSize = 1;
        // Color actual del pincel
        private Color _brushColor = Color.Black;
        #endregion

        #region Constructor
        /// <summary>
        /// Inicializa el canvas con el número de columnas y filas especificado.
        /// </summary>
        public PixelCanvas(int cols = 32, int rows = 32)
        {
            _cols = cols;
            _rows = rows;
            _pixels = new int[cols, rows];

            this.Width = cols * PixelSize;
            this.Height = rows * PixelSize;
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Establece el color actual del pincel.
        /// </summary>
        public void SetColor(Color color) => _brushColor = color;

        /// <summary>
        /// Obtiene el color actual del pincel.
        /// </summary>
        public Color GetCurrentColor() => _brushColor;

        /// <summary>
        /// Establece el color del pincel.
        /// </summary>
        public void SetBrushColor(Color color) => _brushColor = color;

        /// <summary>
        /// Establece el tamaño del pincel.
        /// </summary>
        public void SetBrushSize(int size) => _brushSize = size;
        #endregion

        #region Drawing Methods
        /// <summary>
        /// Dibuja una línea desde el centro del canvas en la dirección y distancia indicadas.
        /// </summary>
        public void DrawLine(int dx, int dy, int distance)
        {
            int x = _cols / 2;
            int y = _rows / 2;

            for (int i = 0; i < distance; i++)
            {
                DrawPoint(x, y);
                x += dx;
                y += dy;
            }

            Invalidate();
        }

        /// <summary>
        /// Dibuja un círculo centrado en el canvas.
        /// </summary>
        public void DrawCircle(int radius, Color brushColor, int brushSize)
        {
            int cx = _cols / 2;
            int cy = _rows / 2;

            for (int angle = 0; angle < 360; angle++)
            {
                double rad = angle * Math.PI / 180;
                int x = cx + (int)(radius * Math.Cos(rad));
                int y = cy + (int)(radius * Math.Sin(rad));
                DrawPoint(x, y, brushColor, brushSize);
            }

            Invalidate();
        }

        /// <summary>
        /// Dibuja un rectángulo centrado en el canvas.
        /// </summary>
        public void DrawRectangle(int width, int height, Color color, int size)
        {
            int cx = _cols / 2;
            int cy = _rows / 2;

            for (int dx = 0; dx < width; dx++)
            {
                DrawPoint(cx + dx, cy, color, size); // Línea superior
                DrawPoint(cx + dx, cy + height - 1, color, size); // Línea inferior
            }

            for (int dy = 0; dy < height; dy++)
            {
                DrawPoint(cx, cy + dy, color, size); // Línea izquierda
                DrawPoint(cx + width - 1, cy + dy, color, size); // Línea derecha
            }

            Invalidate();
        }

        /// <summary>
        /// Rellena todo el canvas con el color especificado.
        /// </summary>
        public void Fill(Color color)
        {
            for (int x = 0; x < _cols; x++)
            {
                for (int y = 0; y < _rows; y++)
                {
                    _pixels[x, y] = color.ToArgb();
                }
            }

            this.Invalidate(); // Redibuja el canvas con el nuevo color
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Dibuja un punto en la posición indicada usando el color y tamaño actuales del pincel.
        /// </summary>
        private void DrawPoint(int cx, int cy)
        {
            DrawPoint(cx, cy, _brushColor, _brushSize);
        }

        /// <summary>
        /// Dibuja un punto en la posición indicada con el color y tamaño especificados.
        /// </summary>
        private void DrawPoint(int cx, int cy, Color color, int size)
        {
            for (int dx = -size / 2; dx <= size / 2; dx++)
            {
                for (int dy = -size / 2; dy <= size / 2; dy++)
                {
                    int x = cx + dx;
                    int y = cy + dy;

                    if (x >= 0 && x < _cols && y >= 0 && y < _rows)
                        _pixels[x, y] = color.ToArgb();
                }
            }
        }

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
        /// Dibuja un punto en la posición indicada con el color y tamaño especificados (alias para DrawPoint).
        /// </summary>
        public void Spawn(int x, int y, Color color, int size)
        {
            DrawPoint(x, y, color, size);
        }
        #endregion

        #region Paint
        /// <summary>
        /// Dibuja el contenido del canvas en pantalla.
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
                    g.FillRectangle(brush, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                    g.DrawRectangle(Pens.Gray, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }
        }
        #endregion
    }
}