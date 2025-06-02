using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    public class PixelCanvas : Panel
    {
        #region Fields

        private int _pixelSize = 5;
        private readonly int[,] _pixels;
        private readonly int _cols;
        private readonly int _rows;
        private int _brushSize = 1;
        private Color _brushColor = Color.Black;

        private Point _cursorPosition;
        private bool _showCursor = true;
        private Timer _cursorTimer;

        #endregion

        #region Constructor

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

        public void SetColor(Color color) => _brushColor = color;
        public Color GetCurrentColor() => _brushColor;
        public void SetBrushColor(Color color) => _brushColor = color;
        public void SetBrushSize(int size) => _brushSize = size;
        public int Cols => _cols;
        public int Rows => _rows;

        #endregion

        #region Drawing Methods

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

        public void Fill(Color color)
        {
            for (int x = 0; x < _cols; x++)
                for (int y = 0; y < _rows; y++)
                    _pixels[x, y] = color.ToArgb();

            Invalidate();
        }

        #endregion

        #region Utility Methods

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

        public void Spawn(int x, int y)
        {
            if (!IsInBounds(x, y))
                throw new CanvasOutOfBoundsError(x, y, _cols, _rows, _pixelSize);

            _cursorPosition = new Point(x, y);
            Invalidate();
        }

        public void ZoomIn()
        {
            _pixelSize = Math.Min(_pixelSize + 2, 60);
            ResizeCanvas();
        }

        public void ZoomOut()
        {
            _pixelSize = Math.Max(_pixelSize - 2, 2);
            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            Width = _cols * _pixelSize;
            Height = _rows * _pixelSize;
            Invalidate();
        }

        public void CenterOnCursor(Panel scrollPanel)
        {
            int scrollX = _cursorPosition.X * _pixelSize - scrollPanel.ClientSize.Width / 2;
            int scrollY = _cursorPosition.Y * _pixelSize - scrollPanel.ClientSize.Height / 2;

            scrollPanel.AutoScrollPosition = new Point(scrollX, scrollY);
        }


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

                    if (!IsInBounds(x, y))
                        throw new CanvasOutOfBoundsError(x, y, _cols, _rows, _pixelSize);

                    _pixels[x, y] = color.ToArgb();
                }
            }
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
                    g.FillRectangle(brush, x * _pixelSize, y * _pixelSize, _pixelSize, _pixelSize);
                    g.DrawRectangle(Pens.LightGray, x * _pixelSize, y * _pixelSize, _pixelSize, _pixelSize);
                }
            }

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
