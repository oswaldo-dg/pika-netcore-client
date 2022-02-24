﻿using PIKA.NetCore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador.Common
{
    public interface IImportadorPika
    {
        Task Importar(string XlsxPath, 
            IDocumentalAPIClient DocumentalClient,
            IMetadatosAPIClient MetadatosClient,
            IContentAPICLient ContentClient);
        string DateFormat { get; set; }
    }
}