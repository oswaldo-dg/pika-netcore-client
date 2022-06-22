using GeneradorJSON.modelo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneradorJSON
{
    public class Generador
    {

        private string RutaBase;
        private string rutaPendientes;
        public Generador(string RutaBase, string RutaPendientes)
        {
            this.RutaBase = RutaBase;
            this.rutaPendientes = RutaPendientes;
        }
        private List<DatosActivo> ObtieneDatos()
        {

            var lista = new List<DatosActivo>();
            if (File.Exists(rutaPendientes))
            {
                FileStream fs = new FileStream(rutaPendientes, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                while (!sr.EndOfStream)
                {
                    string l = sr.ReadLine();
                    if (!string.IsNullOrEmpty(l))
                    {
                        List<string> partes = l.Split('\t').ToList();
                        DatosActivo d = new DatosActivo()
                        {
                            Lote = partes[0],
                            Caja = partes[1],
                            Indice = partes[2],
                            Nombre = partes[3],
                            Expediente = partes[4],
                            PlantillaId = partes[5],
                            CuadroClasificacionId = partes[6],
                            Area = partes[7],
                            RutaPre = partes[8],
                            EntradaClasificacionId = partes[9],
                            ArchivoId = partes[10],
                            UnidadAdministrativaArchivoId = partes[11],
                            VolumenId = partes[12],
                            RutaFisica = Path.Combine(RutaBase, partes[0], partes[1]),
                            RutaPost = "",
                        };
                    }

                }
            }
            return lista;
        }

        private void Log(string msg)
        {
            File.AppendAllText(Path.Combine(RutaBase, "log.txt"), $"{msg}\r\n");
            Console.WriteLine($"{msg}");
        }

        public void Iniciar()
        {
            Log("Iniciando proceso");
            List<DatosActivo> datos = ObtieneDatos();
            Log($"Expedientes {datos.Count}");
            List<string> Cajas = datos.Select(x => x.Caja).Distinct().ToList();
            Log($"Cajas {Cajas.Count}");
            foreach (string caja in Cajas)
            {

                
                var expedientes = datos.Where(x=>x.Caja ==caja).ToList();
                Log($"\r\n\r\nProcesando caja {caja} con {expedientes.Count} expedientes");
                var duplicados = expedientes.GroupBy(e => e.Nombre).Select(g => new { codigo = g.Key, count = g.Count() }).Where(r => r.count > 1).ToList();
                foreach (var dup in duplicados)
                {

                    var actualizar = expedientes.Where(x => x.Nombre == dup.codigo).ToList();
                    int idx = 1;
                    foreach (var item in actualizar)
                    {
                        item.Nombre = $"{item.Nombre}-{idx}";
                        idx++;
                    }
                }

                if (!Directory.Exists(Path.Combine(RutaBase, "cajas", caja)))
                {
                    Directory.CreateDirectory(Path.Combine(RutaBase, "cajas", caja));
                }


                    foreach (var expediente in expedientes)
                {
                    Log($"Procesando expediente {expediente.Nombre}@{expediente.Expediente}");
                    ActivoImportacion act = ObtieneActivoBase(expediente.Nombre, expediente.Expediente);
                    EstableceConfiguracionActivo(act, expediente);
                    act.Archivos = this.ObtieneArchivosValidos(act.Ruta);
                    act.RutaRepositorio = $"/{((string.IsNullOrEmpty(expediente.RutaPre) ? "" : $"{expediente.RutaPre}/"))}{expediente.Lote}/{expediente.Caja}{((string.IsNullOrEmpty(expediente.RutaPost) ? "" : $"/{expediente.RutaPost}"))}";
                    string runFile = Path.Combine(RutaBase, "cajas", expediente.Caja, $"{expediente.Expediente}.json");
                    if (File.Exists(runFile))
                    {
                        File.Delete(runFile);
                    }
                    File.WriteAllText(runFile, JsonConvert.SerializeObject(act, Formatting.Indented));
                }

            }
            Log("Fin proceso");
        }

        private List<string> ObtieneArchivosValidos(string RutaExpediente)
        {
            List<string> strs = new List<string>();
            var exts = "jpg,jpeg".Split(',').ToList();
         
            if(Directory.Exists(RutaExpediente))
            {
                foreach (string list in Directory.GetFiles(RutaExpediente, "*.*").ToList<string>())
                {
                    foreach (string extensionesImportacion in exts)
                    {
                        if (list.ToLower().EndsWith(extensionesImportacion.ToLower()))
                        {
                            strs.Add(list);
                            break;
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    if (strs.Count > 0)
                    {
                        strs.RemoveAt(0);
                    }
                }
            }
           

            //if (this.config.RemoverInicio > 0)
            //{
            //    for (int i = 0; i < this.config.RemoverInicio; i++)
            //    {
            //        if (strs.Count > 0)
            //        {
            //            strs.RemoveAt(0);
            //        }
            //    }
            //}
            //else
            //{
            //    // Si la primera hoja es un parche remueve las 2 primeras
            //    if (detector.EsPArche(strs[0], this.config.RemoverPorUmbral))
            //    {
            //        for (int i = 0; i < 2; i++)
            //        {
            //            if (strs.Count > 0)
            //            {
            //                strs.RemoveAt(0);
            //            }
            //        }
            //    }

            //}
            return strs;
        }


        private ActivoImportacion EstableceConfiguracionActivo(ActivoImportacion act, DatosActivo config)
        {
            act.Ruta = config.RutaFisica;
            act.VolumentId = config.VolumenId;
            act.PlantillaId = config.PlantillaId;
            act.ArchivoId = config.ArchivoId;
            act.CuadroClasificacionId = config.CuadroClasificacionId;
            act.UnidadAdministrativaArchivoId = config.UnidadAdministrativaArchivoId;
            act.ArchivoOrigenId = config.ArchivoId;
            act.EntradaClasificacionId = config.EntradaClasificacionId;
            act.ValoresPlantilla.Add(new ValorPlantilla() { Valor = config.Caja, Id = "62939005-3397-4abd-a361-0f2ae2f31345" }); // Caja
            act.ValoresPlantilla.Add(new ValorPlantilla() { Valor = config.Expediente, Id = "692a684a-a2ff-4d65-b844-6d2cb81a1bcf" }); // ID Digitalizacion
            act.ValoresPlantilla.Add(new ValorPlantilla() { Valor = config.Lote, Id = "4e5cc854-60fd-401e-baf0-5afbd5948b83" }); // Lote
            act.ValoresPlantilla.Add(new ValorPlantilla() { Valor = config.Area, Id = "1648beb4-da4a-4bde-bd13-b702381bc5e1" }); // Area
            return act;
        }


        private ActivoImportacion ObtieneActivoBase(string nombre, string Id)
        {
            ActivoImportacion act = new ActivoImportacion
            {
                Id = Id,
                Nombre = nombre,
                IDunico = "",
                FechaApertura = new DateTime(2020, 1, 1, 0, 0, 0),
                FechaCierre = null,
                Asunto = nombre,
                CodigoOptico = "",
                CodigoElectronico = "",
                EsElectronico = false,
                Reservado = false,
                Confidencial = false,
                EnPrestamo = false,
                Ampliado = false,
                TieneContenido = true,
                Ruta = "",
                EsFolder = true,
                CuadroClasificacionId = "",
                EntradaClasificacionId = "",
                ElementoId = "",
                ArchivoId = "",
                OrigenId = "PIKA",
                ArchivoOrigenId = "",
                UnidadAdministrativaArchivoId = "",
                PlantillaId = "",
                ValoresPlantilla = new List<ValorPlantilla>()
            };

            return act;
        }

    }
}
