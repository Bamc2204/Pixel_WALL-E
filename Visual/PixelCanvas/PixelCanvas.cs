using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    public class PixelCanvas : Panel
    {
        #region Fields

        private const int PixelSize = 20;
        private readonly int[,] _pixels;
        private readonly int _cols;
        private readonly int _rows;

        private int _brushSize = 1;
        private Color _brushColor = Color.Black;

        #endregion

        #region Constructor

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

        public void SetColor(Color color) => _brushColor = color;
        public Color GetCurrentColor() => _brushColor;
        public void SetBrushColor(Color color) => _brushColor = color;
        public void SetBrushSize(int size) => _brushSize = size;

        #endregion

        #region Drawing Methods

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

        private void DrawPoint(int cx, int cy)
        {
            DrawPoint(cx, cy, _brushColor, _brushSize);
        }

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

        public void Clear()
        {
            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _pixels[x, y] = Color.White.ToArgb();

            Invalidate();
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < _cols && y >= 0 && y < _rows;
        }

        public void Spawn(int x, int y, Color color, int size)
        {
            DrawPoint(x, y, color, size);
        }


        #endregion

        #region Paint

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
