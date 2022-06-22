using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja
{
    public partial class Activo 
    {
        public string Id { get; set; }

        public string CuadroClasificacionId { get; set; }
        public string EntradaClasificacionId { get; set; }

        public string Nombre { get; set; }

        public string IDunico { get; set; }

        public System.DateTimeOffset FechaApertura { get; set; }

        public System.DateTimeOffset? FechaCierre { get; set; }

        public string Asunto { get; set; }

        public string CodigoOptico { get; set; }

        public string CodigoElectronico { get; set; }

        public bool EsElectronico { get; set; }

        public bool Reservado { get; set; }

        public bool Confidencial { get; set; }

        public bool Eliminada { get; set; }

        public string UbicacionCaja { get; set; }

        public string UbicacionRack { get; set; }

        public string ArchivoOrigenId { get; set; }

        public string ArchivoId { get; set; }

        public string UnidadAdministrativaArchivoId { get; set; }

        public bool EnPrestamo { get; set; }

        public bool Ampliado { get; set; }

        public System.DateTimeOffset? FechaRetencionAT { get; set; }

        public System.DateTimeOffset? FechaRetencionAC { get; set; }

        public string AlmacenArchivoId { get; set; }

        public string ZonaAlmacenId { get; set; }

        public string ContenedorAlmacenId { get; set; }

        public string TipoOrigenDefault { get; set; }

        public string TipoOrigenId { get; set; }

        public string OrigenId { get; set; }

        public string TipoArchivoId { get; set; }
        public int? Vencidos { get; set; }
        public bool TieneContenido { get; set; }
        public string ElementoId { get; set; }

    }
}
