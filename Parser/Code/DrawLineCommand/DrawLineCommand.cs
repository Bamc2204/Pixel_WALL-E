namespace Wall_E
{
    // Clase que representa el comando DrawLine, utilizada para almacenar los parámetros del comando y su línea
    public class DrawLineCommand : Comando
    {
        // Propiedad que indica la dirección X de la línea a dibujar
        public int DirX { get; set; }
        // Propiedad que indica la dirección Y de la línea a dibujar
        public int DirY { get; set; }
        // Propiedad que indica la distancia (longitud) de la línea a dibujar
        public int Distance { get; set; }

        // Sobrescribe el método ToString para mostrar el comando de forma legible
        public override string ToString()
        {
            // Devuelve una cadena con el formato del comando y la línea donde se encontró
            return $"DrawLine({DirX}, {DirY}, {Distance}) [línea {Linea}]";
        }
    }
}