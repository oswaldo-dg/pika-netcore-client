using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using PIKA.NetCore.Importador.JsonUnico;
using PIKA.NetCore.Importador.LoteCaja;
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
            List<string> Tipos = new List<string>() { "XLSX", "JSON", "CAJA" };

            //if (args.Length < 2)
            //{
            //    Console.WriteLine("Argumentos: [XLSX|JSON|CAJA] [Archivo XSLX] [Archivo Omisiones .TXT]");
            //    return;
            //}

            string tipo = "";
            string archivo = "";
            if (args.Length > 1)
            {
                tipo = args[0].ToUpper();
                archivo = args[1].ToUpper();
            } else
            {
                tipo = "CAJA";
            }
            
            string omisiones = null;
            if(args.Length > 2)
            {
                omisiones = args[2];
            }

            if (Tipos.IndexOf(tipo) >= 0)
            {
                Console.WriteLine($"Procesando: {tipo}, {archivo} {omisiones}");

                if(File.Exists(archivo) || archivo =="")
                {
                    var host = AppStartup(tipo.ToUpper());
                    var service = ActivatorUtilities.CreateInstance<Importador>(host.Services);
                    

                    switch (tipo)
                    {
                        case "XLSX":
                            service.Run(archivo, omisiones).Wait();
                            break;

                        case "JSON":
                            service.Run(archivo, omisiones).Wait();
                            break;

                        case "CAJA":
                            service.RunSinconexion(archivo).Wait();
                            break;

                        default:
                            
                            break;
                    }

                } else
                {
                    Console.WriteLine($"Archivo no válido {archivo}");
                }

            } else
            {
                Console.WriteLine($"Tipo no válido de procesamiento {tipo}");
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
                                    services.AddTransient<IImportadorPika, ImportadorXLSX>();
                                    break;

                                case "JSON":
                                    services.AddTransient<IImportadorPika, ImportadorJsonUnico>();
                                    break;
                                case "CAJA":
                                    services.AddTransient<IImportadorPika, ImportadorLoteCaja>();
                                    break;

                                default:
                                    services.AddTransient<IImportadorPika, ImportadorLoteCaja>();
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
