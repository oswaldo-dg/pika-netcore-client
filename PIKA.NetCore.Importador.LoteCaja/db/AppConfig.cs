using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.RegularExpressions;
namespace PIKA.NetCore.Importador.LoteCaja.db
{
    public class LongitudDatos
    {
        public static int NombreLargo { get => 500; }
        public static int Nombre { get => 200; }
        public static int IDunico { get => 250; }
        public static int GUID { get => 128; }

        public static int Tipo { get => 64; }
        public static int Descripcion { get => 500; }

        public static int RegExp { get => 1024; }

        public static int CodigoPostal { get => 10; }
        public static int UICulture { get => 10; }
        public static int Version { get => 10; }
        public static int Icono { get => 100; }
        public static int ControlHTML { get => 128; }
        public static int MIME { get => 100; }

        public static int TEXTO_INDEXABLE_LARGO { get => 512; }

        public static int PAYLOAD_EVENTO { get => 2048; }

        public static int CadenaConexion { get => 2000; }

    }

    public class AppConfig
    {
        static IConfiguration Config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), ("appSettings.json")))
                .Build();

        public static string MySQlConnString
        {
            get
            {
                return Config.GetSection("ConnectionStrings")["mysql"];
            }
        }

    }
}
