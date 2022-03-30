using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using PIKA.NetCore.Importador.JsonUnico;
using PIKA.NetCore.Importador.XLSX;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<string> Tipos = new List<string>() { "XLSX", "JSON" };

            if(args.Length <2 )
            {
                Console.WriteLine("Argumentos: [XSLS|JSON] [Archivo XSLX] [Archivo Omisiones .TXT]");
                return;
            }

            if (Tipos.IndexOf(args[0].ToUpper()) >= 0)
            {
                Console.WriteLine($"Procesando: {args[0]}, {args[1]} {args[2]}");

                if(File.Exists(args[1]))
                {
                    var host = AppStartup(args[0].ToUpper());
                    var service = ActivatorUtilities.CreateInstance<Importador>(host.Services);
                    service.Run(args[1], args[2]).Wait();

                } else
                {
                    Console.WriteLine($"Archivo no válido {args[1]}");
                }

            } else
            {
                Console.WriteLine($"Tipo no válido de procesamiento {args[0]}");
            }

            
            Console.WriteLine("FIN ------------------");
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        static IHost AppStartup(string Tipo)
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
                            switch(Tipo)
                            {
                                case "XLSX":
                                    services.AddTransient<IImportadorPika, ImportadorJsonUnico>();
                                    break;

                                case "JSON":
                                    services.AddTransient<IImportadorPika, ImportadorXLSX>();
                                    break;
                            }
                            
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
