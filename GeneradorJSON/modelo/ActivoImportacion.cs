using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorJSON
{
    public class ActivoImportacion : Activo
    {
        public ActivoImportacion()
        {
            ValoresPlantilla = new List<ValorPlantilla>();
            Archivos = new List<string>();
        }
        public string RutaRepositorio { get; set; }
        public string Ruta { get; set; }
        public bool EsFolder { get; set; }
        public string PlantillaId { get; set; }
        public string VolumentId { get; set; }
        public List<string> Archivos { get; set; }
        public List<ValorPlantilla> ValoresPlantilla { get; set; }

    }


    public class ValorPlantilla
    {
        public string Id { get; set; }
        public string Valor { get; set; }
    }
}
