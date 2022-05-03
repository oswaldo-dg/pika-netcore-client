using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = PIKA.NetCore.API.Version;

namespace PIKA.NetCore.Client
{
    public class ContentAPICLient : PikaClient, IContentAPICLient, IPikaSession
    {
        public ContentAPICLient()
        {

        }
        public async Task<PikaAPIResult<Elemento>> GetElementoById(string Id)
        {
            PikaAPIResult<Elemento> result = new PikaAPIResult<Elemento>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVContenidoElementoGetAsync(Id, Constants.APIVERSION);

                    if (response != null)
                    {
                        result.Payload = response;
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_FOUND;
                        result.ErrorCode = Constants.ERR_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Volumen>> GetVolumenById(string Id)
        {
            PikaAPIResult<Volumen> result = new PikaAPIResult<Volumen>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVContenidoVolumenGetAsync(Id, Constants.APIVERSION);

                    if (response != null)
                    {
                        result.Payload = response;
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_FOUND;
                        result.ErrorCode = Constants.ERR_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Version>> GetVersionById(string Id)
        {
            PikaAPIResult<Version> result = new PikaAPIResult<Version>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVContenidoVersionGetAsync(Id, Constants.APIVERSION);

                    if (response != null)
                    {
                        result.Payload = response;
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_FOUND;
                        result.ErrorCode = Constants.ERR_NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Version>> CreateVersion(Version Version)
        {
            PikaAPIResult<Version> result = new PikaAPIResult<Version>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVContenidoVersionPostAsync(Constants.APIVERSION, Version);

                    if (response != null)
                    {
                        result.Payload = response;
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_CREATED;
                        result.ErrorCode = Constants.ERR_NOT_CREATED;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Version>> UpdateVersion(string Id, Version Version)
        {
            PikaAPIResult<Version> result = new PikaAPIResult<Version>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    await apiClient.ApiVContenidoVersionPutAsync(Id, Constants.APIVERSION, Version);
                    result.Payload = Version;
                    result.Success = true;

                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Elemento>> CreateElemento(Elemento Elemento)
        {
            PikaAPIResult<Elemento> result = new PikaAPIResult<Elemento>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {

                    var query = "Nombre".ObtieneConsultaNombre(Elemento.Nombre);

                    List<FiltroConsulta> tmp = query.Filtros.ToList();
                    tmp.Add("CarpetaId".ObtieneFiltroTextoEQ(Elemento.CarpetaId));
                    query.Filtros = tmp;

                    var elementos = await apiClient.ApiVContenidoElementoPageAsync(query, Constants.APIVERSION);
                    if (elementos.ConteoTotal > 0)
                    {
                        if (elementos.ConteoTotal == 1)
                        {
                            result.Payload = elementos.Elementos.ToList()[0];
                            result.Success = true;
                        }
                        else
                        {
                            result.Error = Constants.ERR_MULTIPLE_RESULTS;
                            result.ErrorCode = Constants.ERR_MULTIPLE_RESULTS;
                        }

                    }
                    else
                    {
                        var response = await apiClient.ApiVContenidoElementoPostAsync(Constants.APIVERSION, Elemento);

                        if (response != null)
                        {
                            result.Payload = response;
                            result.Success = true;
                        }
                        else
                        {
                            result.Error = Constants.ERR_NOT_CREATED;
                            result.ErrorCode = Constants.ERR_NOT_CREATED;
                        }
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

        public async Task<PikaAPIResult<Carpeta>> CreateCarpeta(Carpeta Carpeta)
        {
            PikaAPIResult<Carpeta> result = new PikaAPIResult<Carpeta>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var query = "Nombre".ObtieneConsultaNombre(Carpeta.Nombre);

                    List<FiltroConsulta> tmp = query.Filtros.ToList();
                    tmp.Add("CarpetaId".ObtieneFiltroTextoEQ(Carpeta.CarpetaPadreId));
                    query.Filtros = tmp;
                    var carpetas = await apiClient.ApiVContenidoCarpetaPageAsync(query, Constants.APIVERSION);

                    if (carpetas.ConteoTotal > 0)
                    {
                        if (carpetas.ConteoTotal == 1)
                        {
                            result.Payload = carpetas.Elementos.ToList()[0];
                            result.Success = true;
                        }
                        else
                        {
                            result.Error = Constants.ERR_MULTIPLE_RESULTS;
                            result.ErrorCode = Constants.ERR_MULTIPLE_RESULTS;
                        }

                    }
                    else
                    {
                        var response = await apiClient.ApiVContenidoCarpetaPostAsync(Constants.APIVERSION, Carpeta);

                        if (response != null)
                        {
                            result.Payload = response;
                            result.Success = true;
                        }
                        else
                        {
                            result.Error = Constants.ERR_NOT_CREATED;
                            result.ErrorCode = Constants.ERR_NOT_CREATED;
                        }
                    }


                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }


        public async Task<PikaAPIResult<Carpeta>> CreateCarpetaRuta(CarpetaDeRuta Carpeta)
        {
            PikaAPIResult<Carpeta> result = new PikaAPIResult<Carpeta>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVContenidoCarpetaRutaAsync(Constants.APIVERSION, Carpeta);

                    if (response != null)
                    {
                        result.Payload = response;
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_CREATED;
                        result.ErrorCode = Constants.ERR_NOT_CREATED;
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }

    

        public async Task<PikaAPICommand> UploadContent(string FilePath, string TransaccionId, string VolumenId, string ElementoId, string PuntoMontajeId, string versionId, int? indice, PosicionCarga? posicionCarga, int? posicioninicio )
        {
            PikaAPICommand cmd = new PikaAPICommand();
            if (InSession)
            {
                VerificarHeaders();
                if (File.Exists(FilePath))
                {
                    FileInfo fi = new FileInfo(FilePath);
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                    FileParameter p = new FileParameter(fs, fi.Name);
                    var response = await apiClient.ApiVUploadAsync(p, TransaccionId, VolumenId, ElementoId, PuntoMontajeId, versionId, indice, posicionCarga, posicioninicio, Constants.APIVERSION);
                    if (response.StatusCode == 200)
                    {
                        cmd.Success = true;
                    }
                    else
                    {
                        cmd.Error = response.StatusCode.ToString();
                        cmd.ErrorCode = response.StatusCode.ToString();
                    }
                }
                else
                {
                    cmd.Error = Constants.ERR_FILE_NOT_FOUND;
                    cmd.ErrorCode = Constants.ERR_FILE_NOT_FOUND;
                }
            }

            return cmd;
        }


        public async Task<PikaAPIResult<List<Pagina>>> CompleteUploadContent(string TransaccionId)
        {
            PikaAPIResult<List<Pagina>> result = new PikaAPIResult<List<Pagina>>();
            if (InSession)
            {
                VerificarHeaders();
                try
                {

                    var response = await apiClient.ApiVUploadCompletarAsync(TransaccionId, Constants.APIVERSION);
                    if (response != null)
                    {
                        result.Payload = response.ToList();
                        result.Success = true;
                    }
                    else
                    {
                        result.Error = Constants.ERR_NOT_CREATED;
                        result.ErrorCode = Constants.ERR_NOT_CREATED;
                    }

                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;
                }
            }

            return result;
        }
    }
}
