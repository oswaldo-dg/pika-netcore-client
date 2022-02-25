using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Version = PIKA.NetCore.API.Version;

namespace PIKA.NetCore.Client
{
    public interface IContentAPICLient
    {
        Task<PikaAPIResult<Version>> GetVersionById(string Id);
        Task<PikaAPIResult<Version>> CreateVersion(Version Version);
        Task<PikaAPIResult<Version>> UpdateVersion(string Id, Version Version);
        Task<PikaAPIResult<Elemento>> CreateElemento(Elemento Elemento);
        Task<PikaAPIResult<Carpeta>> CreateCarpeta(Carpeta Carpeta);
        Task<PikaAPIResult<Elemento>> GetElementoById(string Id);
        Task<PikaAPIResult<Carpeta>> CreateCarpetaRuta(CarpetaDeRuta Carpeta);
      
        Task<PikaAPICommand> UploadContent(string FilePath, string TransaccionId, string VolumenId, string ElementoId, string PuntoMontajeId, string versionId, int? indice, PosicionCarga? posicionCarga, int? posicioninicio);
        Task<PikaAPIResult<List<Pagina>>> CompleteUploadContent(string TransaccionId);
    }
}
