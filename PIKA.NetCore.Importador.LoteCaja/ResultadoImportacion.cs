using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja
{
    public class ResultadoImportacion
    {

        public string Id { get; set; }
        public bool Ok { get; set; }
        public string ActivoId { get; set; }
        public string ElementoId { get; set; }
        public long Tamano { get; set; }
        public int Paginas { get; set; }

        public string Error { get; set; }

    }
}
