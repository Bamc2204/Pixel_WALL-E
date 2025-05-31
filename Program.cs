using System;
using System.Windows.Forms;

namespace Wall_E
{
    static class Program
    {
        #region Main Method
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Inicializa la configuración visual y ejecuta el formulario principal.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Establece el modo de DPI alto para la aplicación (mejor visualización en pantallas modernas)
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Habilita los estilos visuales para los controles de Windows Forms
            Application.EnableVisualStyles();

            // Usa el renderizado de texto compatible para los controles
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia y muestra el formulario principal de la aplicación
            Application.Run(new MainForm());
        }
        #endregion
    }
}