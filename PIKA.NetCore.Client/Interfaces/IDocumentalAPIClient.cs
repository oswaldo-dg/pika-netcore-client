using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public interface IDocumentalAPIClient
    {
        Task<PikaAPIResult<List<ValorListaOrdenada>>> GetRepositories();

        Task<PikaAPIResult<ValorListaOrdenada>> GetEntradaClasificacionId(string Nombre);

        Task<PikaAPIResult<ValorListaOrdenada>> GetArchivoId(string Nombre);

        Task<PikaAPIResult<ValorListaOrdenada>> GetUnidadAdministrativaId(string Nombre);

        Task<PikaAPIResult<Plantilla>> GetPlantillabyName(string Nombre);

        Task<PikaAPIResult<Activo>> CreaActivo(Activo Activo);

        Task<PikaAPIResult<Elemento>> LinkElementoActivo(string ActivoId, string ElementoId);
        Task<PikaAPIResult<Elemento>> GetElementoActivo(string ActivoId);

        Task<PikaAPIResult<EntradaClasificacion>> GetEntradaClasificacion(string Id);
    }
}
