namespace Wall_E
{
    // Clase que representa el comando Color, derivada de la clase base Comando
    public class ColorCommand : Comando
    {
        // Propiedad pública que almacena el nombre del color a utilizar
        public string NombreColor { get; set; }

        // Sobrescribe el método ToString para mostrar el comando de forma legible
        public override string ToString() => $"Color(\"{NombreColor}\") [línea {Linea}]";
        // Devuelve una cadena con el formato del comando y la línea donde se encontró
    }
}