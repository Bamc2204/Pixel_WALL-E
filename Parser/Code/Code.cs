namespace Wall_E
{
    // Clase abstracta base para todos los comandos que se pueden reconocer en el parser
    public abstract class Comando
    {
        // Propiedad que almacena el número de línea del código fuente donde se encontró el comando
        public int Linea { get; set; }
    }
}