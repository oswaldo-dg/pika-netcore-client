using IdentityModel.Client;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public abstract class PikaClient : IPikaSession
    {
        /// <summary>
        /// Http client to API calls
        /// </summary>
        protected static HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// Expiration JWT check
        /// </summary>
        public DateTime UpdateJWTAt;

        /// <summary>
        /// Is there a valid session?
        /// </summary>
        public bool InSession = false;

        /// <summary>
        /// Token response from AUTH service
        /// </summary>
        public TokenResponse Token;

        /// <summary>
        /// AUTH decodes token
        /// </summary>
        public string DecodedJWT;

        /// <summary>
        /// AP configuration 
        /// </summary>
        protected PikaAPIConfiguration Config;

        /// <summary>
        /// Current domain and OU for API calls
        /// </summary>
        protected DomainAccess currentDomainAccess;

        /// <summary>
        /// List of domains available for the in session user
        /// </summary>
        protected List<DomainAccess> domainAccesses;

        /// <summary>
        /// Current API configuration
        /// </summary>
        PikaAPIConfiguration IPikaSession.APIConfig => this.Config;

        /// <summary>
        /// Shared API client 
        /// </summary>
        protected API.Client apiClient;

        /// <summary>
        /// Set API configuration
        /// </summary>
        /// <param name="Config"></param>
        public virtual void SetConfig(PikaAPIConfiguration Config) {
            this.Config = Config;
        }

        public void SetLoginInfo(DateTime UpdateJWTAt, string DecodedJWT, TokenResponse Token)
        {
            InSession = true;
            this.UpdateJWTAt = UpdateJWTAt;
            this.DecodedJWT = DecodedJWT;
            this.Token = Token;
            
            apiClient = new API.Client(HttpClient)
            {
                BaseUrl = Config.URLAPI
            };

        }

        /// <summary>
        /// UPdate headers for API call
        /// </summary>
        protected void VerificarHeaders()
        {
            if (DateTime.Now.Ticks > UpdateJWTAt.Ticks)
            {
                var t = Task.Run(() => Login());
                t.Wait();
            }

            AddHeader("Authorization", "Bearer " + Token.AccessToken);
            if (currentDomainAccess != null)
            {
                AddHeader("did", currentDomainAccess.DomainId);
                AddHeader("tid", currentDomainAccess.OrgUnitId);
            }
        }

        /// <summary>
        /// Add a header to the API call
        /// </summary>
        /// <param name="Nombre"></param>
        /// <param name="Valor"></param>
        private void AddHeader(string Nombre, string Valor)
        {
            if (HttpClient.DefaultRequestHeaders.Contains(Nombre)) HttpClient.DefaultRequestHeaders.Remove(Nombre);
            HttpClient.DefaultRequestHeaders.Add(Nombre, Valor);
        }

        /// <summary>
        /// decode AUTH JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private (string json, string error) Decode(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                return (decoder.Decode(token), "");


            }
            catch (TokenExpiredException)
            {
                return ("", Constants.ERR_EXPIRED_JWT_TOKEN);
            }
            catch (SignatureVerificationException)
            {
                return ("", Constants.ERR_INVALID_SIGNATURE_JWT_TOKEN);
            }
        }

        /// <summary>
        /// Try to login with the configuration at this.Config
        /// </summary>
        /// <returns>Valid JWT on success, null otherwise</returns>
        public async Task<PikaAPIResult<string>> Login()
        {

            PikaAPIResult<string> result = new PikaAPIResult<string>();
            result.Payload = null;
            InSession = false;

            if (!this.Config.Valid())
            {
                result.ErrorCode = Constants.ERR_INVALID_CONFIGURATION;
            }
            else
            {
                var disco = await HttpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = Config.URLIdentity,
                    Policy = { RequireHttps = false, AllowHttpOnLoopback = true }
                });

                if (disco.IsError)
                {
                    result.ErrorCode = Constants.ERR_IDENTITY_DISCOVERY_FAIL;
                    result.Error = $"{disco.ErrorType} {disco.Error}\r\n{disco.Exception}";

                }
                else
                {
                    Token = await HttpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = Config.ClientId,
                        GrantType = "password",
                        ClientSecret = Config.APISecret,
                        UserName = Config.Username,
                        Password = Config.Password,
                        Scope = Config.APIScope
                    });

                    if (Token.IsError)
                    {
                        result.ErrorCode = Constants.ERR_IDENTITY_LOGIN_FAIL;
                        result.Error = $"{Token.ErrorType} {Token.ErrorDescription}\r\n{Token.Exception}";
                    }
                    else
                    {
                        var (json, error) = Decode(Token.AccessToken);
                        if (!string.IsNullOrEmpty(error))
                        {
                            result.ErrorCode = error;
                        }
                        else
                        {
                            InSession = true;
                            UpdateJWTAt = DateTime.Now.AddSeconds(Token.ExpiresIn - 180);
                            DecodedJWT = json;
                            result.Success = true;

                            apiClient = new API.Client(HttpClient)
                            {
                                BaseUrl = Config.URLAPI
                            };

                        }

                    }

                }
            }

            return result;
        }

        /// <summary>
        /// Get a list o valid domains for the in session user
        /// </summary>
        /// <returns></returns>
        public async Task<PikaAPIResult<List<DomainAccess>>> GetUserDomainAccess()
        {

            PikaAPIResult<List<DomainAccess>> result = new PikaAPIResult<List<DomainAccess>>
            {
                Payload = null
            };

            if (InSession) {
                
                domainAccesses = new List<DomainAccess>();
                VerificarHeaders();
               
                dynamic d = JObject.Parse(DecodedJWT);

                foreach (string s in d.role)
                {
                    if (s.IndexOf('-') > 0)
                    {
                        var valores = s.Split('-');


                        var domain = apiClient.ApiVOrgDominioGetAsync(valores[0], Constants.APIVERSION);
                        var ou = apiClient.ApiVOrgUnidadOrganizacionalGetAsync(valores[1], Constants.APIVERSION);

                        List<Task> ltask = new List<Task>() { domain, ou };
                        Task.WaitAll(ltask.ToArray());

                        domainAccesses.Add(new DomainAccess()
                        {
                            Domain = domain.Result.Nombre,
                            OrgUnit = ou.Result.Nombre,
                            DomainId = valores[0],
                            OrgUnitId = valores[1],
                            Type = valores[2] == "admin" ? UserType.Admin : UserType.Usuer
                        }); ;

                        
                    }
                }

                if(domainAccesses.Count>0)
                {
                    currentDomainAccess = domainAccesses[0];
                }
                result.Payload = domainAccesses;
                result.Success = true;
            }
            else
            {
                result.ErrorCode = Constants.ERR_NOT_IN_SESSION;
            }

            return result;
        }

    }
}
