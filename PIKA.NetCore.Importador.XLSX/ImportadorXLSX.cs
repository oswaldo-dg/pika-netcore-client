using ClosedXML.Excel;
using PIKA.NetCore.API;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.XLSX
{
    public class ImportadorXLSX : IImportadorPika
    {


        private XLWorkbook workbook = null;
        private IXLWorksheet ws = null;
        private IXLWorksheet results = null;

        private List<ValorListaOrdenada> CacheEntradaClasificacionId = new List<ValorListaOrdenada>();
        private List<ValorListaOrdenada> CacheArchivoId = new List<ValorListaOrdenada>();
        private List<ValorListaOrdenada> CacheUnidadAdministrativaId = new List<ValorListaOrdenada>();
        private List<Plantilla> CachePlantillaId = new List<Plantilla>();
        private List<Carpeta> CacheCarpetas = new List<Carpeta>();

        public ImportadorXLSX()
        {

        }

        public string DateFormat { get; set; }

        public async Task Importar(string XlsxPath, 
            IDocumentalAPIClient DocumentalClient, 
            IMetadatosAPIClient MetadatosClient, 
            IContentAPICLient ContentClient)
        {
            try
            {
                workbook = new XLWorkbook(XlsxPath);
                ws = workbook.Worksheet(1);


                if (string.IsNullOrEmpty(DateFormat))
                {
                    DateFormat = "dd/MM/yyyy";
                }

                int i = 2;
                int conteo = 0;
                bool empty = string.IsNullOrEmpty(ws.Row(i).Cell(Constants.COL_NOMBRE).GetString());

                while (!empty)
                {
                    conteo++;
                    i++;
                    empty = string.IsNullOrEmpty(ws.Row(i).Cell(Constants.COL_NOMBRE).GetString());
                }

                i = 2;
                empty = string.IsNullOrEmpty(ws.Row(i).Cell(Constants.COL_NOMBRE).GetString());
                while (!empty)
                {
                    ws.Row(i).Cell(Constants.COL_ERROR).SetValue<string>("");
                    ws.Row(i).Cell(Constants.COL_ESTADO_ACTIVO).SetValue<string>("");
                    ws.Row(i).Cell(Constants.COL_ESTADO_ELEMENTO).SetValue<string>("");
                    ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("");

                    ActivoImportacion act = GetActivo(ws.Row(i), ws.Row(1));
                    var validacion = act.ErroresValidacion();
                    if (validacion.Count == 0)
                    {
                        
                        bool verificaIdEntrada = await VerificaIdEntrada(act, DocumentalClient);
                        bool verificaIdArchivo = await VerificaIdArchivo(act, DocumentalClient);
                        bool verificaIdUnidadAdmin = await VerificaIdUnidadAdministrativa(act, DocumentalClient);
                        bool verificaIdPlantilla = await VerificaIdPlantilla(act, DocumentalClient);
                        

                        if(!verificaIdEntrada || !verificaIdArchivo || !verificaIdUnidadAdmin || !verificaIdPlantilla)
                        {
                            ws.Row(i).Cell(Constants.COL_ERROR).SetValue<string>("INVALID_DATA");
                            validacion.Add("INVALID_DATA");
                            
                        } else
                        {
                            Activo activo = act.ToActivo();
                            var activoresult =  await DocumentalClient.CreaActivo(activo);
                            if (activoresult.Success)
                            {
                                Carpeta carpetaElemento = null;
                                Elemento ElementoActivo = null;
                                API.Version VersionElemento = null;
                                // Crea el activo
                                activo = activoresult.Payload;



                                // Verifica si hay un elemento de contenido asociado al activo
                                var elementoactivoResult = await DocumentalClient.GetElementoActivo(activo.Id);
                                if(elementoactivoResult.Success)
                                {
                                    ws.Row(i).Cell(Constants.COL_ESTADO_ACTIVO).SetValue<string>("OK");

                                    // Crea el elemento en caso de no existir
                                    if (string.IsNullOrEmpty(elementoactivoResult.Payload.Id))
                                    {
                                        //1. Primero la ruta para su salvaguarda
                                        carpetaElemento = CacheCarpetas.Where(x => x.PermisoId == activo.EntradaClasificacionId).SingleOrDefault();
                                        // Si la carpeta no estpa en el cache la añade
                                        if(carpetaElemento == null)
                                        {
                                            Log.Information($"CAching"); 
                                            var entradaClasificacion = CacheEntradaClasificacionId.Where(x => x.Id == activo.EntradaClasificacionId).SingleOrDefault();
                                            if (entradaClasificacion != null)
                                            {
                                                var rutaElemento = await ContentClient.CreateCarpetaRuta(
                                                                                            new CarpetaDeRuta()
                                                                                            {
                                                                                                PuntoMontajeId = elementoactivoResult.Payload.PuntoMontajeId,
                                                                                                Ruta = $"/{entradaClasificacion.Texto}",
                                                                                                UsuarioId = ""
                                                                                            });
                                                if (rutaElemento.Success)
                                                {
                                                    carpetaElemento = rutaElemento.Payload;
                                                    // En el cache la clave periso ID se utilzia para almacenar la entrada de clasificación del activo
                                                    carpetaElemento.PermisoId = activo.EntradaClasificacionId;
                                                    CacheCarpetas.Add(carpetaElemento);
                                                }
                                            }
                                        }
                                       
                                        // 2. Crea el elemento en la carpeta
                                        if(carpetaElemento!=null)
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

                                            var elementoResult = await ContentClient.CreateElemento(elemento);
                                            if (elementoResult.Success)
                                            {
                                                ElementoActivo = elementoResult.Payload;
                                                /// Si el elemento fue creado satisfactoriamente se vincula el activo al elemento
                                                await DocumentalClient.LinkElementoActivo(activo.Id, ElementoActivo.Id);
                                                
                                            }
                                        }

                                    } else
                                    {
                                        // YA axiste un elemento asociado al activo
                                        ElementoActivo = elementoactivoResult.Payload;
                                    }

                                    if(ElementoActivo != null)
                                    {
                                        ws.Row(i).Cell(Constants.COL_ESTADO_ELEMENTO).SetValue<string>("OK");
                                        Log.Information($"Activo creado {activo.Nombre} > Elemento {ElementoActivo.Id}");
                                        if (act.TieneContenido)
                                        {
                                            // Añade el conmtenido pendiente
                                            var versionResult = await ContentClient.GetVersionById(ElementoActivo.Id);
                                            bool modificado = false;
                                            if (versionResult.Success)
                                            {
                                                VersionElemento = versionResult.Payload;

                                                if(VersionElemento!=null)
                                                {
                                                    if(VersionElemento.Partes != null)
                                                    {
                                                       foreach(var p in  VersionElemento.Partes.ToList())
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

                                                    if(modificado)
                                                    {
                                                        await ContentClient.UpdateVersion(VersionElemento.Id, VersionElemento);
                                                        ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("OK+VERSION");
                                                    } 
                                                }
                                                
                                                

                                                //if (VersionElemento.Partes == null) VersionElemento.Partes = new List<Parte>();
                                                //List<string> archivos = new List<string>();
                                                //List<string> archivosNuevos = new List<string>();
                                                //if (act.EsFolder)
                                                //{
                                                //    if (System.IO.Directory.Exists(act.Ruta))
                                                //    {
                                                //        archivos = System.IO.Directory.GetFiles(act.Ruta).ToList();
                                                //    }

                                                //} else
                                                //{
                                                //    if (System.IO.File.Exists(act.Ruta))
                                                //    {
                                                //        archivos.Add(act.Ruta);
                                                //    }
                                                //}

                                                //// Añade sólo los archivos inextsiontes como partes
                                                //archivos.ForEach(a => {
                                                //    FileInfo fi = new FileInfo(a);
                                                //    // Log.Error($"{fi.Name} {fi.Length}");
                                                //    if(!VersionElemento.Partes.Any(x=>x.NombreOriginal ==  fi.Name))
                                                //    {
                                                //        archivosNuevos.Add(a);
                                                //    }
                                                //});

                                                //if(archivosNuevos.Count>0)
                                                //{
                                                //    string sesion = Guid.NewGuid().ToString();
                                                //    foreach (string archivo in archivosNuevos)
                                                //    {
                                                //        await ContentClient.UploadContent(archivo, sesion, ElementoActivo.VolumenId, ElementoActivo.Id, ElementoActivo.PuntoMontajeId, ElementoActivo.Id);
                                                //    }
                                                //    await ContentClient.CompleteUploadContent(sesion);
                                                //}

                                            } // Version obtenida OK

                                            ws.Row(i).Cell(Constants.COL_ESTADO_ELEMENTO).SetValue<string>("OK+CONTENT");
                                        } // Tiene contenido

                                        if(!string.IsNullOrEmpty(act.PlantillaId))
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
                                                if (respuestaPlantillas.Payload == null || !respuestaPlantillas.Payload.Documentos.Any(x=>x.PlantillaId == act.PlantillaId))
                                                {
                                                   var plantillaresult = await MetadatosClient.CreateMetadatosObject(act.PlantillaId, rqPlantilla);
                                                    if (plantillaresult.Success)
                                                    {
                                                        ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("OK");
                                                        Log.Information("Plantilla añadida");
                                                    }
                                                }
                                                else
                                                {
                                                    var doc = respuestaPlantillas.Payload.Documentos.Where(x => x.PlantillaId == act.PlantillaId).FirstOrDefault();
                                                    if(doc != null)
                                                    {
                                                        ws.Row(i).Cell(Constants.COL_ESTADO_METADATOS).SetValue<string>("UPDATED");
                                                        var plantillaresult = await MetadatosClient.UpdateMetadatosObject(doc.DocumentoId, act.PlantillaId, rqPlantilla);
                                                        Log.Information("Plantilla Actualizada");
                                                    }
                                                }
                                            }
                                        }
                                    } else
                                    {
                                        ws.Row(i).Cell(Constants.COL_ESTADO_ELEMENTO).SetValue<string>("ERROR");
                                    }
                                    
                                } else
                                {
                                    Log.Information($"Cuak {activo.Nombre} {activo.Id}");
                                }

                              

                            } else
                            {
                                ws.Row(i).Cell(Constants.COL_ESTADO_ACTIVO).SetValue<string>($"Error al crear activo {activo.Nombre} {activoresult.ErrorCode} {activoresult.Error}");
                                Log.Error($"Error al crear activo {activo.Nombre} {activoresult.ErrorCode} {activoresult.Error}");
                            }
                        }
                    }
                    else
                    {
                        ws.Row(i).Cell(Constants.COL_ERROR).SetValue<string>($"Datos no validos renglon {i}");
                        Log.Warning($"Datos no validos renglon {i}");
                    }

                    Log.Information($"Pendientes {conteo - (i-1)}");
                    workbook.Save();
                    i++;
                    empty = string.IsNullOrEmpty(ws.Row(i).Cell(Constants.COL_NOMBRE).GetString());
                }

                workbook.Save();
            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
            }
            
        }


        private async Task<bool> VerificaIdPlantilla(ActivoImportacion act, IDocumentalAPIClient DocumentalClient)
        {
            bool IdOK = false;
            var IdEntrada = CachePlantillaId.Where(x => x.Nombre == act.PlantillaId).FirstOrDefault();
            if (IdEntrada == null)
            {
                var PlantillaId = await DocumentalClient.GetPlantillabyName(act.PlantillaId);
                if (PlantillaId.Success)
                {
                    act.PlantillaId = PlantillaId.Payload.Id;
                    CachePlantillaId.Add(PlantillaId.Payload);
                    IdEntrada = PlantillaId.Payload;
                    IdOK = true;
                }
                else
                {
                    Log.Error($"Plantilla {PlantillaId.ErrorCode}:{PlantillaId.Error}");
                }
            }
            else
            {
                act.PlantillaId = IdEntrada.Id;
                IdOK = true;
            }
            if (IdEntrada != null)
            {
                foreach (var vp in act.ValoresPlantilla)
                {
                    var pp = IdEntrada.Propiedades.Where(x => x.Nombre == vp.Id).FirstOrDefault();
                    if (pp != null)
                    {
                        vp.Id = pp.Id;
                    }
                }
            }
            return IdOK;
        }

        private async Task<bool> VerificaIdUnidadAdministrativa(ActivoImportacion act, IDocumentalAPIClient DocumentalClient)
        {
            bool IdOK = false;
            var IdEntrada = CacheUnidadAdministrativaId.Where(x => x.Texto == act.UnidadAdministrativaArchivoId).FirstOrDefault();
            if (IdEntrada == null)
            {
                var UniadadAdminId = await DocumentalClient.GetUnidadAdministrativaId(act.UnidadAdministrativaArchivoId);
                if (UniadadAdminId.Success)
                {
                    act.UnidadAdministrativaArchivoId = UniadadAdminId.Payload.Id;
                    CacheUnidadAdministrativaId.Add(UniadadAdminId.Payload);
                    IdOK = true;
                }
                else
                {
                    Log.Error($"Unidad Administrativa {UniadadAdminId.ErrorCode}:{UniadadAdminId.Error}");
                }
            }
            else
            {
                act.UnidadAdministrativaArchivoId = IdEntrada.Id;
                IdOK = true;
            }
            return IdOK;
        }

        private async Task<bool> VerificaIdArchivo(ActivoImportacion act, IDocumentalAPIClient DocumentalClient)
        {
            bool IdOK = false;
            var IdEntrada = CacheArchivoId.Where(x => x.Texto == act.ArchivoOrigenId).FirstOrDefault();
            if (IdEntrada == null)
            {
                var ArchivoId = await DocumentalClient.GetArchivoId(act.ArchivoOrigenId);
                if (ArchivoId.Success)
                {
                    act.ArchivoId = ArchivoId.Payload.Id;
                    act.ArchivoOrigenId = ArchivoId.Payload.Id;
                    CacheArchivoId.Add(ArchivoId.Payload);
                    IdOK = true;
                }
                else
                {
                    Log.Error($"Archivo {ArchivoId.ErrorCode}:{ArchivoId.Error}");
                }
            }
            else
            {
                act.ArchivoId = IdEntrada.Id;
                act.ArchivoOrigenId = IdEntrada.Id;
                IdOK = true;
            }
            return IdOK;
        }

        private async Task<bool> VerificaIdEntrada(ActivoImportacion act, IDocumentalAPIClient DocumentalClient)
        {
            bool IdEntradaOK = false;
            var IdEntrada = CacheEntradaClasificacionId.Where(x => x.Texto == act.EntradaClasificacionId).FirstOrDefault();
            if (IdEntrada == null)
            {
                var EntradaClasificacionId = await DocumentalClient.GetEntradaClasificacionId(act.EntradaClasificacionId);
                if (EntradaClasificacionId.Success)
                {
                    act.EntradaClasificacionId = EntradaClasificacionId.Payload.Id;
                    CacheEntradaClasificacionId.Add(EntradaClasificacionId.Payload);
                    IdEntradaOK = true;
                }
                else
                {
                    Log.Error($"Entrada Clasificación {EntradaClasificacionId.ErrorCode}:{EntradaClasificacionId.Error}");
                }
            }
            else
            {
                act.EntradaClasificacionId = IdEntrada.Id;
                IdEntradaOK = true;
            }
            return IdEntradaOK;
        }


        private ActivoImportacion GetActivo(IXLRow Row, IXLRow Header)
        {
            ActivoImportacion a = new();

            a.Id = null;
            a.CuadroClasificacionId = null;
            a.EntradaClasificacionId = Constants.COL_ENTRADACLASIFICACIONID.GetCellAsText(Row);
            a.Nombre = Constants.COL_NOMBRE.GetCellAsText(Row);
            a.IDunico = Constants.COL_IDUNICO.GetCellAsText(Row);
            a.FechaApertura = Constants.COL_FECHAAPERTURA.GetCellAsDate(Row, DateFormat) ?? DateTime.MinValue;
            a.FechaCierre = Constants.COL_FECHACIERRE.GetCellAsDate(Row, DateFormat);
            a.Asunto = Constants.COL_ASUNTO.GetCellAsText(Row);
            a.CodigoOptico = Constants.COL_CODIGOOPTICO.GetCellAsText(Row);
            a.CodigoElectronico = Constants.COL_CODIGOELECTRONICO.GetCellAsText(Row);
            a.EsElectronico = Constants.COL_ESELECTRONICO.GetCellAsBoolean(Row);
            a.Reservado = Constants.COL_ESRESERVADO.GetCellAsBoolean(Row);
            a.Confidencial = Constants.COL_ESCONFIDENCIAL.GetCellAsBoolean(Row);
            a.ArchivoOrigenId = Constants.COL_ARCHIVOORIGENID.GetCellAsText(Row);
            a.ArchivoId = Constants.COL_ARCHIVOORIGENID.GetCellAsText(Row);
            a.UnidadAdministrativaArchivoId = Constants.COL_UNIDADADMINISTRATIVAARCHIVOID.GetCellAsText(Row);
            a.EnPrestamo = Constants.COL_ENPRESTAMO.GetCellAsBoolean(Row);
            a.Eliminada = false;
            a.Ampliado = Constants.COL_AMPLIADO.GetCellAsBoolean(Row);
            a.TipoOrigenId = Constants.ORIGEN_ARCHIVO;
            a.OrigenId = Constants.COL_ORIGENID.GetCellAsText(Row);
            a.TieneContenido = Constants.COL_TIENECONTENIDO.GetCellAsBoolean(Row);
            a.Ruta = Constants.COL_RUTA.GetCellAsText(Row);
            a.EsFolder = Constants.COL_ESFOLDER.GetCellAsBoolean(Row);
            a.PlantillaId = Constants.COL_PLANTILLAID.GetCellAsText(Row);

            if (!string.IsNullOrEmpty(a.PlantillaId))
            {
                int index = Constants.COL_PLANTILLAID + 1;
                string FieldId = index.GetCellAsText(Header);
                bool EmptyCol = string.IsNullOrEmpty(FieldId);
                while (!EmptyCol)
                {
                    if (!string.IsNullOrEmpty(FieldId))
                    {
                        a.ValoresPlantilla.Add(new ValorPlantilla()
                        {
                            Id = FieldId,
                            Valor = index.GetCellAsText(Row)
                        });
                        index++;
                        FieldId = index.GetCellAsText(Header);
                        EmptyCol = string.IsNullOrEmpty(FieldId);
                    }

                }
            }

            return a;
        }




    }
}
