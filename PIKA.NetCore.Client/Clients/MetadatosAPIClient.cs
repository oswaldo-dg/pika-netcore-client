using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public partial class MetadatosAPIClient : PikaClient, IMetadatosAPIClient, IPikaSession
    {
        public MetadatosAPIClient()
        {

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
        public async Task<PikaAPIResult<VinculosObjetoPlantilla>> GetObjectMetadataLinks(string ObjectoId, string TipoEntidad)
        {
            PikaAPIResult<VinculosObjetoPlantilla> result = new PikaAPIResult<VinculosObjetoPlantilla>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVMetadatosVinculosAsync(TipoEntidad, ObjectoId, Constants.APIVERSION);

                    result.Payload = response;
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

        public async Task<PikaAPIResult<DocumentoPlantilla>> CreateMetadatosObject(string PlantillaId, RequestValoresPlantilla Valores)
        {
            PikaAPIResult<DocumentoPlantilla> result = new PikaAPIResult<DocumentoPlantilla>();
            result.Payload = null;
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    var response = await apiClient.ApiVMetadatosPostAsync(PlantillaId, Constants.APIVERSION, Valores);

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

        public async Task<PikaAPICommand> UpdateMetadatosObject(string DocumentoId, string PlantillaId, RequestValoresPlantilla Valores)
        {
            PikaAPICommand result = new PikaAPICommand();
            if (InSession)
            {
                VerificarHeaders();
                try
                {
                    await apiClient.ApiVMetadatosPutAsync(DocumentoId, PlantillaId, Constants.APIVERSION, Valores);
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


    }
}
