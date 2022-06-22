using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja
{
    public class DatosActivo
    {
        public string Lote { get; set; }
        public string Caja { get; set; }
        public int Indice { get; set; }
        public string Nombre { get; set; }
        public string CuadroClasificacionId { get; set; }
        public string EntradaClasificacionId { get; set; }
        public string ArchivoId { get; set; }
        public string VolumenId { get; set; }
        public string UnidadAdministrativaArchivoId { get; set; }
    }
}
