using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wall_E
{
    #region CanvasDrawer

    /// <summary>
    /// Clase para gestionar el dibujo de píxeles y formas en un PictureBox.
    /// Permite dibujar píxeles, limpiar el canvas y ajustar el tamaño de la cuadrícula.
    /// </summary>
    public class CanvasDrawer
    {
        #region Fields

        // Referencia al PictureBox donde se dibuja.
        private readonly PictureBox _canvas;
        // Imagen en memoria donde se dibuja antes de mostrar en pantalla.
        private Bitmap _bitmap;
        // Objeto Graphics para dibujar sobre el bitmap.
        private Graphics _graphics;
        // Tamaño de cada "píxel" de la cuadrícula.
        private int _pixelSize = 10;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor que recibe el PictureBox en el cual se dibuja.
        /// </summary>
        /// <param name="canvas">PictureBox destino del dibujo.</param>
        public CanvasDrawer(PictureBox canvas)
        {
            _canvas = canvas;
            Initialize();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa el bitmap y el objeto Graphics para el dibujo.
        /// </summary>
        private void Initialize()
        {
            _bitmap = new Bitmap(_canvas.Width, _canvas.Height); // Crea un bitmap del tamaño del PictureBox
            _graphics = Graphics.FromImage(_bitmap);             // Obtiene el objeto Graphics para dibujar
            _canvas.Image = _bitmap;                             // Asigna el bitmap al PictureBox
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Establece el tamaño de cada píxel (cuadrícula).
        /// </summary>
        /// <param name="size">Tamaño del píxel (mínimo 1).</param>
        public void SetPixelSize(int size)
        {
            _pixelSize = Math.Max(1, size); // Asegura que el tamaño sea al menos 1
        }

        /// <summary>
        /// Dibuja un píxel en la posición (x, y) con el color especificado.
        /// </summary>
        /// <param name="x">Coordenada X en la cuadrícula.</param>
        /// <param name="y">Coordenada Y en la cuadrícula.</param>
        /// <param name="color">Color del píxel.</param>
        public void DrawPixel(int x, int y, Color color)
        {
            // Dibuja un rectángulo relleno en la posición correspondiente
            using SolidBrush brush = new(color);
            _graphics.FillRectangle(brush, x * _pixelSize, y * _pixelSize, _pixelSize, _pixelSize);
            _canvas.Invalidate(); // Solicita el repintado del PictureBox
        }

        /// <summary>
        /// Limpia el canvas con el color de fondo blanco.
        /// </summary>
        public void Clear()
        {
            _graphics.Clear(Color.White); // Limpia el bitmap a blanco
            _canvas.Invalidate();         // Solicita el repintado del PictureBox
        }

        #endregion
    }

    #endregion
}