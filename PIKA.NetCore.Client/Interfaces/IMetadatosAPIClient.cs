using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public interface IMetadatosAPIClient
    {
        Task<PikaAPIResult<Plantilla>> GetPlantillabyName(string Nombre);
        Task<PikaAPIResult<VinculosObjetoPlantilla>> GetObjectMetadataLinks(string ObjectoId, string TipoEntidad);
        Task<PikaAPIResult<DocumentoPlantilla>> CreateMetadatosObject(string PlantillaId, RequestValoresPlantilla Valores);

        Task<PikaAPICommand> UpdateMetadatosObject(string DocumentoId, string PlantillaId, RequestValoresPlantilla Valores);
    }
}
