using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.NetCore.Client
{
    public interface IPikaSession
    {
        /// <summary>
        /// API configuration including user login information
        /// </summary>
        PikaAPIConfiguration APIConfig { get; }

        /// <summary>
        /// Set API configuration
        /// </summary>
        /// <param name="Config"></param>
        void SetConfig(PikaAPIConfiguration Config);

        /// <summary>
        /// Login to the aplication
        /// </summary>
        /// <returns></returns>
        Task<PikaAPIResult<string>> Login();

        /// <summary>
        /// Get domains assigned to the user
        /// </summary>
        /// <returns></returns>
        Task<PikaAPIResult<List<DomainAccess>>> GetUserDomainAccess();
        
    }
}
