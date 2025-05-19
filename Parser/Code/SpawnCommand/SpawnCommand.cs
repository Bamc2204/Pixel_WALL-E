namespace Wall_E
{
    // Clase que representa el comando Spawn, derivada de la clase base Comando
    public class SpawnCommand : Comando
    {
        // Propiedad que almacena la coordenada X donde se va a hacer el "spawn"
        public int X { get; set; }
        // Propiedad que almacena la coordenada Y donde se va a hacer el "spawn"
        public int Y { get; set; }

        // Sobrescribe el método ToString para mostrar el comando de forma legible
        public override string ToString() => $"Spawn({X}, {Y}) [línea {Linea}]";
        // Devuelve una cadena con el formato del comando y la línea donde se encontró
    }
}