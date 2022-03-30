using ClosedXML.Excel;
using Newtonsoft.Json;
using PIKA.NetCore.API;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.JsonUnico
{
    public class ImportadorJsonUnico : IImportadorPika
    {

        private List<string> Omisiones = new List<string>();
        public ImportadorJsonUnico()
        {

        }

        public string DateFormat { get; set; }

        private void LogData(string data)
        {
            File.AppendAllText($"{ArchivoJson}.log", $"{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}\t{data}\r\n");
        }

        public string ArchivoJson { get; set; }
        public async Task Importar(string Archivo,
            IDocumentalAPIClient DocumentalClient,
            IMetadatosAPIClient MetadatosClient,
            IContentAPICLient ContentClient,
            string ArchivoOmisiones = null)
        {
            ResultadoImportacion resultado = new ResultadoImportacion() { Ok = false };
            ArchivoJson = Archivo;


            LogData($"Iniciando proceso");

            bool hayOmisiones;

            if (ArchivoOmisiones != null)
            {
                if (File.Exists(ArchivoOmisiones))
                {
                    var data = File.ReadAllText(ArchivoOmisiones).Split('\n');
                    foreach (string s in data)
                    {
                        Omisiones.Add(s.Trim().ToLower());
                    }

                }
            }
            hayOmisiones = Omisiones.Count > 0;

            if (string.IsNullOrEmpty(DateFormat))
            {
                DateFormat = "dd/MM/yyyy";
            }

            try
            {
                ActivoImportacion act = JsonConvert.DeserializeObject<ActivoImportacion>(File.ReadAllText(Archivo));
                Activo activo = act.ToActivo();
                var resultEntrada = await DocumentalClient.GetEntradaClasificacion(act.EntradaClasificacionId);
                if(resultEntrada != null)
                {
                    var activoresult = await DocumentalClient.CreaActivo(activo);
                    if (activoresult.Success)
                    {
                        LogData($"Activo creado");
                        Carpeta carpetaElemento = null;
                        Elemento ElementoActivo = null;
                        API.Version VersionElemento = null;
                        // Crea el activo
                        activo = activoresult.Payload;

                        resultado.ActivoId = activo.Id;


                        // Verifica si hay un elemento de contenido asociado al activo
                        LogData($"Verificando elemento");
                        var elementoactivoResult = await DocumentalClient.GetElementoActivo(activo.Id);
                        if (elementoactivoResult.Success)
                        {
                            // Crea el elemento en caso de no existir
                            if (string.IsNullOrEmpty(elementoactivoResult.Payload.Id))
                            {
                                LogData($"Creando elemento");
                                LogData($"Verificación de Carpeta");
                                if (resultEntrada.Payload != null)
                                {
                                    var rutaElemento = await ContentClient.CreateCarpetaRuta(
                                                                                new CarpetaDeRuta()
                                                                                {
                                                                                    PuntoMontajeId = elementoactivoResult.Payload.PuntoMontajeId,
                                                                                    Ruta = $"/{resultEntrada.Payload.Nombre}",
                                                                                    UsuarioId = ""
                                                                                });
                                    if (rutaElemento.Success)
                                    {
                                        LogData($"Carpeta creada");
                                        carpetaElemento = rutaElemento.Payload;
                                    }
                                }
                                else
                                {
                                    LogData($"No hay datos para crear la carpeta");
                                }


                                // 2. Crea el elemento en la carpeta
                                if (carpetaElemento != null)
                                {
                                    Elemento elemento = new Elemento()
                                    {
                                        AutoNombrar = true,
                                        CarpetaId = carpetaElemento.Id,
                                        CreadorId = "any", // Esta valor se lee en el backend de la identidad en el JWT
                                        Eliminada = false,
                                        FechaCreacion = DateTime.UtcNow,
                                        Id = null,
                                        IdExterno = null,
                                        Nombre = activo.Nombre,
                                        OrigenId = activo.Id, // EN el caso de los activos el origen es el Id del mismo para el elemento
                                        PermisoId = null,
                                        PuntoMontajeId = carpetaElemento.PuntoMontajeId,
                                        TipoElemento = null,
                                        TipoOrigenDefault = "",
                                        TipoOrigenId = "Activo",
                                        Versionado = true,
                                        VersionId = null,
                                        VolumenId = elementoactivoResult.Payload.VolumenId
                                    };
                                    LogData($"Creando elemento");
                                    var elementoResult = await ContentClient.CreateElemento(elemento);
                                    if (elementoResult.Success)
                                    {
                                        LogData($"Elemento Creado");
                                        ElementoActivo = elementoResult.Payload;
                                        /// Si el elemento fue creado satisfactoriamente se vincula el activo al elemento
                                        await DocumentalClient.LinkElementoActivo(activo.Id, ElementoActivo.Id);

                                    }
                                }

                            }
                            else
                            {
                                LogData($"Ya existe un elemento");
                                // YA axiste un elemento asociado al activo
                                ElementoActivo = elementoactivoResult.Payload;
                            }

                            if (ElementoActivo != null)
                            {
                                LogData($"Activo creado {activo.Nombre} > Elemento {ElementoActivo.Id}");
                                resultado.ElementoId = ElementoActivo.Id;

                                Log.Information($"Activo creado {activo.Nombre} > Elemento {ElementoActivo.Id}");
                                if (act.TieneContenido)
                                {
                                    // Añade el conmtenido pendiente
                                    var versionResult = await ContentClient.GetVersionById(ElementoActivo.Id);
                                    bool modificado = false;
                                    if (versionResult.Success)
                                    {
                                        VersionElemento = versionResult.Payload;

                                        if (VersionElemento != null)
                                        {
                                            if (VersionElemento.Partes != null && VersionElemento.Partes.ToList().Count > 0)
                                            {
                                                Log.Information($"Reordenando");
                                                foreach (var p in VersionElemento.Partes.ToList())
                                                {
                                                    string[] partes = (p.NombreOriginal ?? "").Split('.');
                                                    if (partes.Length == 2)
                                                    {
                                                        int index;
                                                        if (int.TryParse(partes[0], out index))
                                                        {
                                                            p.Indice = index;
                                                            modificado = true;
                                                        }
                                                    }
                                                }
                                            }

                                            if (modificado)
                                            {
                                                await ContentClient.UpdateVersion(VersionElemento.Id, VersionElemento);
                                            }
                                        }

                                        if (VersionElemento.Partes == null) VersionElemento.Partes = new List<Parte>();
                                        List<string> archivos = new List<string>();
                                        List<string> archivosNuevos = new List<string>();

                                        archivos = act.Archivos.OrderBy(f => f).ToList();

                                        // Añade sólo los archivos inextsiontes como partes
                                        archivos.ForEach(a =>
                                        {
                                            FileInfo fi = new FileInfo(a);
                                            // Log.Error($"{fi.Name} {fi.Length}");
                                            if (!VersionElemento.Partes.Any(x => x.NombreOriginal == fi.Name))
                                            {
                                                archivosNuevos.Add(a);
                                            }
                                        });

                                        if (archivosNuevos.Count > 0)
                                        {
                                            string sesion = Guid.NewGuid().ToString();
                                            int indice = 0;
                                            foreach (string archivo in archivosNuevos)
                                            {
                                                await ContentClient.UploadContent(archivo, sesion, ElementoActivo.VolumenId, ElementoActivo.Id, ElementoActivo.PuntoMontajeId, ElementoActivo.Id, indice, null, null);
                                                indice++;
                                            }
                                            await ContentClient.CompleteUploadContent(sesion);
                                        }

                                        versionResult = await ContentClient.GetVersionById(ElementoActivo.Id);
                                        if(versionResult.Success)
                                        {

                                            File.WriteAllText($"{Archivo}.result.json", JsonConvert.SerializeObject(versionResult.Payload, Formatting.Indented));

                                            resultado.Tamano = versionResult.Payload.Partes.Sum(x => x.LongitudBytes);
                                            resultado.Paginas = versionResult.Payload.Partes.Count;

                                        } else
                                        {
                                            LogData($"No version result {versionResult.Error} {versionResult.ErrorCode}");
                                            throw new Exception("No version result");
                                        }
                                        

                                    } // Version obtenida OK

                                    // ws.Row(i).Cell(Constants.COL_ESTADO_ELEMENTO).SetValue<string>("OK+CONTENT");
                                } // Tiene contenido

                                if (!string.IsNullOrEmpty(act.PlantillaId))
                                {
                                    RequestValoresPlantilla rqPlantilla = new RequestValoresPlantilla()
                                    {
                                        Tipo = "PIKA.Modelo.Contenido.Elemento",
                                        Id = ElementoActivo.Id,
                                        FiltroJerarquico = ElementoActivo.CarpetaId,
                                        Filtro = ElementoActivo.VolumenId,
                                        Valores = new List<ValorPropiedad>()
                                    };

                                    act.ValoresPlantilla.ForEach(valor =>
                                    {
                                        rqPlantilla.Valores.Add(new ValorPropiedad() { PropiedadId = valor.Id, Valor = valor.Valor });
                                    });

                                    var respuestaPlantillas = await MetadatosClient.GetObjectMetadataLinks(ElementoActivo.Id, "PIKA.Modelo.Contenido.Elemento");
                                    if (respuestaPlantillas.Success)
                                    {
                                        if (respuestaPlantillas.Payload == null || !respuestaPlantillas.Payload.Documentos.Any(x => x.PlantillaId == act.PlantillaId))
                                        {
                                            var plantillaresult = await MetadatosClient.CreateMetadatosObject(act.PlantillaId, rqPlantilla);
                                            if (plantillaresult.Success)
                                            {
                                                // ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("OK");
                                                Log.Information("Plantilla añadida");
                                            }
                                        }
                                        else
                                        {
                                            var doc = respuestaPlantillas.Payload.Documentos.Where(x => x.PlantillaId == act.PlantillaId).FirstOrDefault();
                                            if (doc != null)
                                            {
                                                // ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("UPDATED");
                                                var plantillaresult = await MetadatosClient.UpdateMetadatosObject(doc.DocumentoId, act.PlantillaId, rqPlantilla);
                                                Log.Information("Plantilla Actualizada");
                                            }
                                        }
                                    }
                                }

                                resultado.Ok = true;
                                resultado.Id = act.Id;
                            }
                            else
                            {
                                LogData($"Error al crear el elemento 1");
                            }

                        }
                        else
                        {
                            LogData($"Error al crear el elemento 2");
                        }



                    }
                    else
                    {
                        Log.Error($"Error al crear activo {activo.Nombre} {activoresult.ErrorCode} {activoresult.Error}");
                        LogData($"Error al crear activo {activo.Nombre} {activoresult.ErrorCode} {activoresult.Error}");
                        resultado.Error = $"Error al crear activo {activo.Nombre} {activoresult.ErrorCode} {activoresult.Error}";
                    }
                } else
                {
                    LogData($"Entrada clasifiación no valida {act.EntradaClasificacionId}");
                    resultado.Error = $"Entrada clasifiación no valida {act.EntradaClasificacionId}";
                }

                

            }
            catch (Exception ex)
            {

                LogData($"Error de proceso {ex}");
                resultado.Error = $"Error de proceso {ex}";
            }



            if (File.Exists($"{Archivo}.done"))
            {
                File.Delete($"{Archivo}.done");
            }
            File.WriteAllText($"{Archivo}.done", JsonConvert.SerializeObject(resultado, Formatting.Indented));

        }

       

    }
}
