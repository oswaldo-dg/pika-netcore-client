using ClosedXML.Excel;
using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.XLSX
{
    public static class Extensiones
    {
        public static string ToJsonString(this object data, bool indented = true)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = indented });
        }

        public static bool GetCellAsBoolean(this int i, IXLRow Row)
        {
            return Row.Cell(i).GetString().Trim() == "1";
        }

        public static string GetCellAsText(this int i, IXLRow Row)
        {
            return Row.Cell(i).GetString().Trim();
        }

        public static DateTime? GetCellAsDate(this int i, IXLRow Row, string DateFormat)
        {
            string Data = i.GetCellAsText(Row);
            
            if (string.IsNullOrEmpty(Data))
            {
                return null;
            }

            if (DateTime.TryParse(Data, null, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                return dt;
            }

            return null;
        }

        public static List<string> ErroresValidacion(this ActivoImportacion a)
        {
            List<string> l = new();
            if (string.IsNullOrEmpty (a.Nombre) )
            {
                l.Add(nameof(a.Nombre).ErrorNUlo());
            }

            if (string.IsNullOrEmpty(a.EntradaClasificacionId))
            {
                l.Add(nameof(a.EntradaClasificacionId).ErrorNUlo());
            }

            if (string.IsNullOrEmpty(a.EntradaClasificacionId))
            {
                l.Add(nameof(a.EntradaClasificacionId).ErrorNUlo());
            }

            if (string.IsNullOrEmpty(a.ArchivoOrigenId))
            {
                l.Add(nameof(a.ArchivoOrigenId).ErrorNUlo());
            }

            if (string.IsNullOrEmpty(a.UnidadAdministrativaArchivoId))
            {
                l.Add(nameof(a.UnidadAdministrativaArchivoId).ErrorNUlo());
            }

            if (a.FechaApertura == DateTime.MinValue)
            {
                l.Add(nameof(a.FechaApertura).ErrorNUlo());
            }

            if(a.TieneContenido && string.IsNullOrEmpty(a.Ruta))
            {
                l.Add(nameof(a.Ruta).ErrorNUlo());
            }

            if (!string.IsNullOrEmpty(a.PlantillaId) && a.ValoresPlantilla.Count == 0)
            {
                l.Add(nameof(a.ValoresPlantilla).ErrorNUlo());
            }

            return l;
        }

        public static string ErrorNUlo(this string Campo)
        {
            return $"{Campo} is NULL or EMPTY";
        }

        public static Activo ToActivo(this ActivoImportacion activo)
        {

            if (activo == null) return null;

            Activo a = new Activo
            {
                Ampliado = activo.Ampliado,
                ArchivoId = activo.ArchivoId,
                ArchivoOrigenId = activo.ArchivoOrigenId,
                Asunto = activo.Asunto,
                CodigoElectronico = activo?.CodigoElectronico,
                CodigoOptico = activo?.CodigoOptico,
                Confidencial = activo.Confidencial,
                CuadroClasificacionId = activo.CuadroClasificacionId,
                ElementoId = activo.ElementoId,
                Eliminada = activo.Eliminada,
                EnPrestamo = activo.EnPrestamo,
                EntradaClasificacionId = activo.EntradaClasificacionId,
                EsElectronico = activo.EsElectronico,
                FechaApertura = activo.FechaApertura,
                FechaCierre = activo.FechaCierre,
                Id = activo.Id,
                IDunico = activo.IDunico,
                OrigenId = activo.OrigenId,
                UnidadAdministrativaArchivoId = activo.UnidadAdministrativaArchivoId,
                TipoOrigenId = activo.TipoOrigenId,
                TipoArchivoId = activo.TipoArchivoId,
                TipoOrigenDefault = activo.TipoOrigenDefault,
                TieneContenido = activo.TieneContenido,
                Reservado = activo.Reservado,
                Nombre = activo.Nombre,
                FechaRetencionAT = activo.FechaRetencionAT,
                FechaRetencionAC = activo.FechaRetencionAC,
            };
            return a;
        }




    }
}
