using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.XLSX
{
    public class ActivoImportacion: Activo
    {
        public ActivoImportacion()
        {
            ValoresPlantilla = new List<ValorPlantilla>();
        }

        public string Ruta { get; set; }
        public bool EsFolder { get; set; }
        public string PlantillaId { get; set; }

        public List<ValorPlantilla> ValoresPlantilla { get; set; }
    }


    public class ValorPlantilla
    {
        public string Id { get; set; }
        public string Valor { get; set; }
    }
}
