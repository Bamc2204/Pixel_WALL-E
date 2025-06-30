using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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
        private int _pixelSize = 6;
        // Matriz de colores de los píxeles.
        private readonly int[,] _pixels;
        // Número de columnas y filas del canvas.
        private readonly int _cols;
        private readonly int _rows;
        // Tamaño actual del pincel.
        private int _brushSize = 1;
        // Color actual del pincel.
        private Color _brushColor;
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
        public PixelCanvas(int cols = 512, int rows = 512)
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
        /// Cambia el tamaño actual del pincel.
        /// </summary>
        public void SetBrushSize(int size) => _brushSize = size;

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
        public void DrawLine(int dx, int dy, int distance, Color color, int size, int line)
        {
            int x = _cursorPosition.X;
            int y = _cursorPosition.Y;

            for (int i = 0; i < distance; i++)
            {
                DrawPoint(x, y, color, size, line);
                x += dx;
                y += dy;
            }

            _cursorPosition = new Point(x, y);
            Invalidate();
        }

        /// <summary>
        /// Dibuja un círculo en la dirección indicada desde la posición actual.
        /// </summary>
        public void DrawCircle(int dirX, int dirY, int radius, Color color, int brushSize, int line)
        {
            int newX = _cursorPosition.X + dirX;
            int newY = _cursorPosition.Y + dirY;

            SetCursorPosition(newX, newY);

            for (int angle = 0; angle < 360; angle++)
            {
                double rad = angle * Math.PI / 180;
                int x = newX + (int)(radius * Math.Cos(rad));
                int y = newY + (int)(radius * Math.Sin(rad));
                DrawPoint(x, y, color, brushSize, line);
            }

            Invalidate();
        }


        /// <summary>
        /// Dibuja un rectángulo en una dirección y distancia desde la posición actual.
        /// </summary>
        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height, Color color, int brushSize, int line)
        {
            int newX = _cursorPosition.X + dirX * distance;
            int newY = _cursorPosition.Y + dirY * distance;

            SetCursorPosition(newX, newY);

            int left = newX - width / 2;
            int top = newY - height / 2;

            for (int i = 0; i < width; i++)
            {
                DrawPoint(left + i, top, color, brushSize, line);
                DrawPoint(left + i, top + height - 1, color, brushSize, line);
            }

            for (int j = 0; j < height; j++)
            {
                DrawPoint(left, top + j, color, brushSize, line);
                DrawPoint(left + width - 1, top + j, color, brushSize, line);
            }

            Invalidate();
        }



        /// <summary>
        /// Rellena todo el canvas con el color especificado.
        /// </summary>
        public void Fill(Color color)
        {
            // Posición inicial del cursor (desde donde empieza el relleno)
            int startX = _cursorPosition.X;
            int startY = _cursorPosition.Y;

            // Si el cursor está fuera de los límites del canvas, no hace nada
            if (startX < 0 || startX >= _cols || startY < 0 || startY >= _rows)
                return;

            // Color que se va a reemplazar (el color actual en la posición del cursor)
            int targetColor = _pixels[startX, startY];

            // Color nuevo que queremos pintar (convertido a entero con ToArgb)
            int fillColor = color.ToArgb();

            // Si el color de destino ya es el color nuevo, no hay nada que hacer
            if (targetColor == fillColor)
                return;

            // Cola para llevar el control de los píxeles que se deben procesar
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY)); // Agrega el punto inicial

            // Mientras haya puntos en la cola por procesar...
            while (queue.Count > 0)
            {
                Point p = queue.Dequeue(); // Saca el siguiente punto
                int x = p.X;
                int y = p.Y;

                // Verifica si está dentro del canvas
                if (x < 0 || x >= _cols || y < 0 || y >= _rows)
                    continue;

                // Si este píxel no tiene el color original, no lo tocamos
                if (_pixels[x, y] != targetColor)
                    continue;

                // Cambia el color del píxel actual
                _pixels[x, y] = fillColor;

                // Agrega los píxeles vecinos a la cola (izquierda, derecha, arriba, abajo)
                queue.Enqueue(new Point(x + 1, y)); // derecha
                queue.Enqueue(new Point(x - 1, y)); // izquierda
                queue.Enqueue(new Point(x, y + 1)); // abajo
                queue.Enqueue(new Point(x, y - 1)); // arriba
            }
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
        private void DrawPoint(int cx, int cy, int line)
        {
            DrawPoint(cx, cy, _brushColor, _brushSize, line);
        }

        /// <summary>
        /// Dibuja un punto en la posición indicada usando el color y tamaño especificados.
        /// </summary>
        private void DrawPoint(int cx, int cy, Color color, int size, int line)
        {
            for (int dx = -size / 2; dx <= size / 2; dx++)
            {
                for (int dy = -size / 2; dy <= size / 2; dy++)
                {
                    int x = cx + dx;
                    int y = cy + dy;

                    if (!IsInBounds(x, y))
                        throw new CanvasOutOfBoundsError(x, y, _cols, _rows, line);

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

        /// <summary>
        /// Cuenta cuántos píxeles tienen un color específico dentro de un área rectangular.
        /// </summary>
        public int CountColorPixels(Color color, int x1, int y1, int x2, int y2, int line)
        {
            // Si alguno de los puntos está fuera del canvas, se retorna 0 como indica el requisito
            if (!IsInBounds(x1, y1) || !IsInBounds(x2, y2)) return 0;

            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            int colorArgb = color.ToArgb();
            int count = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (_pixels[x, y] == colorArgb)
                        count++;
                }
            }

            return count;
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