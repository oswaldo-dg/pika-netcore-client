using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PIKA.NetCore.Client;
using PIKA.NetCore.Importador.Common;
using System;
using System.Threading.Tasks;

namespace PIKA.NetCore.Importador
{
    public class Importador: IAplicacion
    {
        private readonly ILogger<Importador> _log;
        private readonly IConfiguration _config;
        private readonly PikaAPIConfiguration _apiConfig;
        private readonly IDocumentalAPIClient documentalAPI;
        private readonly IContentAPICLient contentAPI;
        private readonly IMetadatosAPIClient metadatosAPI;
        private readonly IImportadorPika importadorPika;
        public Importador(ILogger<Importador> log, 
                        IConfiguration config, 
                        IDocumentalAPIClient documentalAPI,
                        IContentAPICLient contentAPI,
                        IMetadatosAPIClient metadatosAPI,
                        IImportadorPika importadorPika) {
            _apiConfig = new PikaAPIConfiguration();
            this.documentalAPI = documentalAPI;
            this.metadatosAPI = metadatosAPI;
            this.contentAPI = contentAPI;
            this._log = log;
            this._config = config;
            this.importadorPika = importadorPika;
            config.Bind("pika", _apiConfig);

            // Esto debe cambiar, debe inyectarse el controlador de sesión en los servicios
            ((IPikaSession)this.documentalAPI).SetConfig(_apiConfig);
            ((IPikaSession)this.metadatosAPI).SetConfig(_apiConfig);
            ((IPikaSession)this.contentAPI).SetConfig(_apiConfig);
        }

        public async Task Run()
        {
            if (((IPikaSession)this.documentalAPI).APIConfig.Valid())
            {
                var result = await ((IPikaSession)documentalAPI).Login();
                if(result.Success)
                {
                    var accessResult = await ((IPikaSession)documentalAPI).GetUserDomainAccess();
                    if(accessResult.Success)
                    {
                        // Esto debe cambiar, debe inyectarse el controlador de sesión en los servicios
                        ((PikaClient)this.metadatosAPI).SetLoginInfo(
                            ((PikaClient)this.documentalAPI).UpdateJWTAt, 
                            ((PikaClient)this.documentalAPI).DecodedJWT,
                            ((PikaClient)this.documentalAPI).Token);

                        ((PikaClient)this.contentAPI).SetLoginInfo(
                            ((PikaClient)this.documentalAPI).UpdateJWTAt,
                            ((PikaClient)this.documentalAPI).DecodedJWT,
                            ((PikaClient)this.documentalAPI).Token);


                        var reposResult = await documentalAPI.GetRepositories();
                        if (reposResult.Success)
                        {
                            reposResult.Payload.ForEach(r => {
                                _log.LogInformation(r.ToJsonString());
                            });

                            importadorPika.DateFormat = "dd/MM/yyyy";
                            await importadorPika.Importar(@"C:\Users\oswal\Desktop\importar.xlsx", documentalAPI, metadatosAPI, contentAPI);

                        } else
                        {
                            _log.LogError($"NO AVAILABLE REPOSITORIES {reposResult.Error}" );
                        }
                    } else
                    {
                        _log.LogError(accessResult.ErrorCode);
                    }

                } else
                {
                    _log.LogError(result.ErrorCode);
                }

            } else
            {
                _log.LogError("INVALID_CONFIGURATION");
            }
        }
    }
}
