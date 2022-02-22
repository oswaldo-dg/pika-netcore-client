using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using PIKA.NetCore.API;


namespace PIKA.NetCore.Importador
{
    public static class Extensiones
    {
        public static string ToJsonString(this object data, bool indented = true)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = indented });
        }
    }
}
