using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    /// <summary>
    /// Panel personalizado para dibujar una cuadrícula de píxeles.
    /// </summary>
    public class PixelCanvas : Panel
    {
        #region Fields

        // Tamaño de cada "pixel" en la cuadrícula
        private const int PixelSize = 20;
        // Ancho de la cuadrícula en cantidad de píxeles
        private const int GridWidth = 32;
        // Alto de la cuadrícula en cantidad de píxeles
        private const int GridHeight = 32;

        // Matriz que almacena el color de cada "pixel"
        private Color[,] pixels = new Color[GridWidth, GridHeight];

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el canvas, activa el doble buffer y lo deja en blanco.
        /// </summary>
        public PixelCanvas()
        {
            DoubleBuffered = true; // Evita parpadeos al dibujar
            Width = GridWidth * PixelSize;
            Height = GridHeight * PixelSize;

            Clear(); // Inicializa todos los píxeles en blanco
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dibuja un pixel en la posición (x, y) con el color indicado.
        /// </summary>
        /// <param name="x">Coordenada X del pixel</param>
        /// <param name="y">Coordenada Y del pixel</param>
        /// <param name="color">Color a aplicar</param>
        public void DrawPixel(int x, int y, Color color)
        {
            // Verifica que la posición esté dentro de la cuadrícula
            if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
            {
                pixels[x, y] = color; // Asigna el color
                Invalidate(); // Solicita repintar el control
            }
        }

        /// <summary>
        /// Limpia el canvas, poniendo todos los píxeles en blanco.
        /// </summary>
        public void Clear()
        {
            for (int x = 0; x < GridWidth; x++)
                for (int y = 0; y < GridHeight; y++)
                    pixels[x, y] = Color.White;

            Invalidate(); // Fuerza el repintado del canvas
        }

        #endregion

        #region Paint

        /// <summary>
        /// Dibuja la cuadrícula de píxeles y las líneas de la cuadrícula.
        /// </summary>
        /// <param name="e">Argumentos del evento de pintado</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Recorre toda la cuadrícula y dibuja cada pixel
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    // Dibuja el rectángulo del pixel con su color
                    using Brush brush = new SolidBrush(pixels[x, y]);
                    g.FillRectangle(brush, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                    // Dibuja el borde del pixel
                    g.DrawRectangle(Pens.Gray, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                }
            }
        }

        #endregion
    }
}