namespace Wall_E
{
    // Clase que representa el comando Size, derivada de la clase base Comando
    public class SizeCommand : Comando
    {
        // Propiedad pública que almacena el valor del tamaño a establecer
        public int Valor { get; set; }

        // Sobrescribe el método ToString para mostrar el comando de forma legible
        public override string ToString() => $"Size({Valor}) [línea {Linea}]";
        // Devuelve una cadena con el formato del comando y la línea donde se encontró
    }
}