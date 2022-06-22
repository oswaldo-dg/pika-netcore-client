using ClosedXML.Excel;
using Newtonsoft.Json;
using PIKA.NetCore.API;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using PIKA.NetCore.Importador.LoteCaja.db;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace PIKA.NetCore.Importador.LoteCaja
{
    public class ImportadorLoteCaja : IImportadorPika
    {
        private MySQLDb db = new MySQLDb();
        public ImportadorLoteCaja()
        {

        }

        public string DateFormat { get; set; }

        private void LogData(string data)
        {
            // File.AppendAllText($"{ArchivoJson}.log", $"{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}\t{data}\r\n");
        }

        private string rutaCaja ="";

        public async Task Importar(string Archivo, IDocumentalAPIClient DocumentalClient, IMetadatosAPIClient MetadatosClient, IContentAPICLient ContentClient, string ArchivoOmisiones = null)
        {

            rutaCaja = Archivo;
            ResultadoImportacion resultado = new ResultadoImportacion() { Ok = false };
            
            if (Directory.Exists(rutaCaja))
            {
                LogData($"Iniciando proceso");
                var activos = ActivosCaja(rutaCaja);


                //foreach(var activo in activos)
                //{
                //    db.Activos.Add(activo);
                //    db.SaveChanges();
                //}


            }
            else
            {
                resultado.Ok = false;
                resultado.Error = $"Ruta inexistente {rutaCaja}";
            }
            
        }

        public DatosActivo ObtieneDatosActivo(string Lote, string Caja, int Indice)
        {

            return null;
        }
        


        private List<Activo> ActivosCaja(string rutaCaja)
        {
            List<Activo> activos = new List<Activo>();
            List<string> partesCaja = rutaCaja.Split(Path.PathSeparator).ToList();
            string caja = partesCaja[partesCaja.Count - 1];
            string lote = partesCaja[partesCaja.Count - 2];


            foreach (string dir in Directory.GetDirectories(rutaCaja).ToList())
            {

                List<string> partes = dir.Split(Path.PathSeparator).ToList();
                string expediente = partes[partes.Count - 1];
                int index = int.Parse(expediente);
                string Id = null;
                DatosActivo datos = ObtieneDatosActivo(lote, caja, index);

                if(datos!=null)
                {
                    Id = Guid.NewGuid().ToString();
                } 

                Activo a = new Activo()
                {
                    Ampliado = false,
                    ArchivoId = datos.ArchivoId,
                    ArchivoOrigenId = datos.ArchivoId,
                    Asunto = datos.Nombre,
                    CodigoElectronico = null,
                    CodigoOptico = null,
                    Confidencial = false,
                    CuadroClasificacionId = datos.CuadroClasificacionId,
                    ElementoId = Id,
                    Eliminada = false,
                    EnPrestamo = false,
                    EntradaClasificacionId = datos.EntradaClasificacionId,
                    EsElectronico = false,
                    FechaApertura = new DateTime(2020,1,1,0,0,0),
                    FechaCierre = null,
                    FechaRetencionAC = null,
                    FechaRetencionAT = null,
                    Id = Id,
                    IDunico = null,
                    Nombre = datos.Nombre,
                    OrigenId = "PIKA",
                    Reservado = false,
                    TieneContenido = true,
                    TipoArchivoId = "a-tra",
                    TipoOrigenId = "unidad-org",
                    UnidadAdministrativaArchivoId = datos.UnidadAdministrativaArchivoId,
                    Vencidos = 0,
                    AlmacenArchivoId = null,
                    ContenedorAlmacenId = null,
                    UbicacionCaja = null,
                    UbicacionRack = null,
                    ZonaAlmacenId = null
                };
                activos.Add(a);

            }

            return activos;

        }
    }
}
