using PIKA.NetCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.NetCore.Client
{
    public static class Extensions
    {

        public static Consulta ObtieneConsultaNombre(this string Propiedad, string Valor)
        {
            var consulta = new Consulta()
            {
                Consecutivo = 0,
                Indice = 0,
                Ord_columna = "",
                Ord_direccion = "",
                Recalcular_totales = true,
                Tamano = 1,
                IdCache = "",
                IdSeleccion = ""
            };
            Valor = System.Net.WebUtility.UrlEncode(Valor);
            consulta.Filtros = new List<FiltroConsulta>() { new FiltroConsulta() { Negacion = false,
                 Operador = "eq", Propiedad = Propiedad, Valor =Valor, ValorString = Valor, NivelFuzzy =0}  }.ToArray();

            return consulta;

        }


        public static FiltroConsulta ObtieneFiltroTextoEQ(this string Propiedad, string Valor) {
            return new FiltroConsulta()
            {
                Negacion = false,
                Operador = "eq",
                Propiedad = Propiedad,
                Valor = Valor,
                ValorString = Valor,
                NivelFuzzy = 0
            };
        }

        public static string QueryString(this Consulta consulta)
        {
            string q = $"i={consulta.Indice}&t={consulta.Tamano}";
            q += $"&ordc={consulta.Ord_columna}&ordd={consulta.Ord_direccion}&idcache={consulta.IdCache ?? ""}";

            if (!string.IsNullOrEmpty(consulta.IdSeleccion))
            {
                q += $"&sel={consulta.IdSeleccion}";
            }

            if (consulta.Filtros!=null && consulta.Filtros.Count > 0)
            {
                int index = 0;
                consulta.Filtros.ToList().ForEach((f) =>
                {
                    q += $"&f[{index}][p]={f.Propiedad}&f[{index}][o]={f.Operador}&f[{index}][v]={f.ValorString}";
                    if (f.Negacion)
                    {
                        q += $"&f[{ index}][n] = 1";
                    }
                    index++;
                });
            }

            // El espacio en blanco extra es debido a que el ciente de swagger remueve el estacio final en estos casos
            return $"{q} ";
        }
    }
}
