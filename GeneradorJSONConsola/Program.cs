using System;

namespace GeneradorJSONConsola
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GeneradorJSON.Generador g = new GeneradorJSON.Generador(AppConfig.RutaBase, AppConfig.RutaPendientes);
            g.Iniciar();
        }
    }
}
