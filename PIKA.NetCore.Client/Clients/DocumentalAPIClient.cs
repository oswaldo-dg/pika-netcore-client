using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public partial class DocumentalAPIClient: PikaClient, IDocumentalAPIClient, IPikaSession
    {
        public DocumentalAPIClient()
        {

        }

        public async Task<PikaAPIResult<ValorListaOrdenada>> GetUnidadAdministrativaId(string Nombre)
        {
            PikaAPIResult<ValorListaOrdenada> result = new PikaAPIResult<ValorListaOrdenada>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    Console.WriteLine($"{System.Text.Json.JsonSerializer.Serialize("UnidadAdministrativa".ObtieneConsultaNombre(Nombre))}");

                    var response = await apiClient.ApiVGdUnidadAdministrativaArchivoPageAsync("UnidadAdministrativa".ObtieneConsultaNombre(Nombre), Constants.APIVERSION);

                    if (response.Elementos != null && response.Elementos.Count == 1)
                    {
                        result.Payload = new ValorListaOrdenada()
                        {
                            Id = response.Elementos.First().Id,
                            Indice = 0,
                            Texto = response.Elementos.First().UnidadAdministrativa
                        };
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

        public async Task<PikaAPIResult<ValorListaOrdenada>> GetArchivoId(string Nombre)
        {
            PikaAPIResult<ValorListaOrdenada> result = new PikaAPIResult<ValorListaOrdenada>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVGdArchivoParesGetAsync("texto".ObtieneConsultaNombre(Nombre), Constants.APIVERSION);

                    if (response != null && response.Count == 1)
                    {
                        result.Payload = response.First();
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

        public async Task<PikaAPIResult<ValorListaOrdenada>> GetEntradaClasificacionId(string Nombre)
        {
            PikaAPIResult<ValorListaOrdenada> result = new PikaAPIResult<ValorListaOrdenada>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVGdEntradaClasificacionPageAsync("Nombre".ObtieneConsultaNombre(Nombre), Constants.APIVERSION);
                    
                    if(response != null && response.ConteoTotal == 1)
                    {
                        result.Payload= new ValorListaOrdenada()
                        {
                            Id = response.Elementos.First().Id,
                            Indice = 0,
                            Texto = response.Elementos.First().Nombre
                        };
                        result.Success = true;
                    } else
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

        public async Task<PikaAPIResult<List<ValorListaOrdenada>>> GetRepositories()
        {
            PikaAPIResult<List<ValorListaOrdenada>> result = new PikaAPIResult<List<ValorListaOrdenada>>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    result.Payload = (await apiClient.ApiVContenidoPuntoMontajeParesGetAsync(new Consulta() { }, Constants.APIVERSION)).ToList();
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

        public async Task<PikaAPIResult<Plantilla>> GetPlantillabyName(string Nombre)
        {
            PikaAPIResult<Plantilla> result = new PikaAPIResult<Plantilla>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {

                    var response = await apiClient.ApiVMetadatosPlantillaPageAsync("Nombre".ObtieneConsultaNombre(Nombre), Constants.APIVERSION);

                    if (response.Elementos != null && response.Elementos.Count == 1)
                    {

                        
                        result.Payload = response.Elementos.First();


                        var responseProps = await apiClient.ApiVMetadatosPropiedadPlantillaPagePlantillaAsync(result.Payload.Id, null, Constants.APIVERSION);
                        if (responseProps != null)
                        {
                            result.Payload.Propiedades = responseProps.Elementos;
                            foreach(var p in result.Payload.Propiedades)
                            {
                                if (p.TipoDatoId == Metadata.tList)
                                {
                                    var resultList = await apiClient.ApiVMetadatosValorListaPlantillaPagePropiedadplantillaAsync(p.Id, null, Constants.APIVERSION);
                                    if (resultList != null)
                                    {
                                        p.AtributoLista = new AtributoLista() { Valores = new List<ValorLista>() };
                                        
                                        resultList.Elementos.ToList().ForEach(v =>
                                        {
                                            p.AtributoLista.Valores.Add(new ValorLista() { Id = v.Id, Indice = v.Indice, Texto = v.Texto });

                                        });
                                    }
                                }
                            }

                        }
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


        public async Task<PikaAPIResult<Elemento>> GetElementoActivo(string ActivoId)
        {
            PikaAPIResult<Elemento> result = new PikaAPIResult<Elemento>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {

                    var response = await apiClient.ApiVGdActivoContenidoGetAsync(ActivoId, Constants.APIVERSION);


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

        public async Task<PikaAPIResult<Elemento>> LinkElementoActivo(string ActivoId, string ElementoId)
        {
            PikaAPIResult<Elemento> result = new PikaAPIResult<Elemento>();
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVGdActivoContenidoPostAsync(ActivoId, ElementoId, Constants.APIVERSION);

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



        public async Task<PikaAPIResult<Activo>> CreaActivo(Activo Activo)
        {
            PikaAPIResult<Activo> result = new PikaAPIResult<Activo>
            {
                Payload = null
            };
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var query = "Nombre".ObtieneConsultaNombre(Activo.Nombre);

                    List<FiltroConsulta> tmp = query.Filtros.ToList();
                    tmp.Add("ArchivoId".ObtieneFiltroTextoEQ(Activo.ArchivoId));
                    query.Filtros = tmp;

                    

                    var activos = await apiClient.ApiVGdActivoPageAsync(query, Constants.APIVERSION);

                    if(activos.ConteoTotal == 0)
                    {
                        var response = await apiClient.ApiVGdActivoPostAsync(Constants.APIVERSION, Activo);

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
                    } else
                    {
                        result.Payload = activos.Elementos.First();
                        result.Success = true;
                    }
                    
                }
                catch (ApiException ex)
                {
                    result.ErrorCode = ex.StatusCode.ToString();
                    result.Error = ex.Message;
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Constants.ERR_UNKNOWN;
                    result.Error = ex.Message;

                }
            }

            return result;
        }

        public async Task<PikaAPIResult<EntradaClasificacion>> GetEntradaClasificacion(string Id)
        {
            PikaAPIResult<EntradaClasificacion> result = new PikaAPIResult<EntradaClasificacion>
            {
                Payload = null
            };
            if (InSession)
            {
                VerificarHeaders();
                try
                {

                    var response = await apiClient.ApiVGdEntradaClasificacionGetAsync(Id, Constants.APIVERSION);

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
                catch (ApiException ex)
                {
                    result.ErrorCode = ex.StatusCode.ToString();
                    result.Error = ex.Message;
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
