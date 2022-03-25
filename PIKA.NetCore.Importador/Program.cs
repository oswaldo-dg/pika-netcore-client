using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using PIKA.NetCore.Importador.XLSX;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Argumentos: [Archivo XSLX] [Archivo Omisiones .TXT]");
                return;
            }

            var host = AppStartup();
            var service = ActivatorUtilities.CreateInstance<Importador>(host.Services);
            service.Run(args[0], args[1]).Wait();
            Console.WriteLine("FIN ------------------");
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration() 
                            .ReadFrom.Configuration(builder.Build()) 
                            .Enrich.FromLogContext() 
                            .WriteTo.Console() 
                            .CreateLogger(); 

            var host = Host.CreateDefaultBuilder() 
                        .ConfigureServices((context, services) => {
                            services.AddTransient<IAplicacion, Importador>();
                            services.AddTransient<IImportadorPika, ImportadorXLSX>();
                            services.AddTransient<IDocumentalAPIClient, DocumentalAPIClient>();
                            services.AddTransient<IMetadatosAPIClient, MetadatosAPIClient>();
                            services.AddTransient<IContentAPICLient, ContentAPICLient>();
                        })
                        .UseSerilog() 
                        .Build(); 
            return host;
        }

    }
}
