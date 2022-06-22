using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorJSONConsola
{
    public class AppConfig
    {
        static IConfiguration Config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), ("appSettings.json")))
                .Build();

        public static string RutaBase
        {
            get
            {
                return Config.GetValue<string>("RutaBase");
            }
        }

        public static string RutaPendientes
        {
            get
            {
                return Config.GetValue<string>("RutaPendientes");
            }
        }

    }
}
