using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador
{
    public interface IAplicacion
    {
        Task Run(string ArchivoXLSX, string ArchivOmisiones);
        Task RunSinconexion(string ruta);
    }
}
