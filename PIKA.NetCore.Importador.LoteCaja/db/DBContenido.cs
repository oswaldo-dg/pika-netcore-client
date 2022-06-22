using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.LoteCaja.db
{
    public partial class MySQLDb 
    {
        public Activo BuscaActivoPorId(string Id)
        {
            return this.Activos.Where(x => x.Id == Id).FirstOrDefault();
            
        }
    }
}
