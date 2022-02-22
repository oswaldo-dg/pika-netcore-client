using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.NetCore.Client
{
    public class Constants
    {
        /// <summary>
        /// PIKA API version 
        /// </summary>
        public const string APIVERSION = "1.0";

        /// <summary>
        ///  Identity and auth constants
        /// </summary>
        public const string ERR_INVALID_CONFIGURATION = "ERR_INVALID_CONFIGURATION";
        public const string ERR_IDENTITY_DISCOVERY_FAIL = "ERR_IDENTITY_DISCOVERY_FAIL";
        public const string ERR_IDENTITY_LOGIN_FAIL = "ERR_IDENTITY_LOGIN_FAIL";
        public const string ERR_EXPIRED_JWT_TOKEN = "ERR_EXPIRED_JWT_TOKEN";
        public const string ERR_INVALID_SIGNATURE_JWT_TOKEN = "ERR_INVALID_SIGNATURE_JWT_TOKEN ";



        public const string ERR_NOT_IN_SESSION = "ERR_NOT_IN_SESSION";

        public const string ERR_UNKNOWN = "ERR_UNKNOWN";
        public const string ERR_NOT_FOUND = "ERR_NOT_FOUND";
        public const string ERR_NOT_CREATED = "ERR_NOT_CREATED";
        public const string ERR_MULTIPLE_RESULTS = "ERR_MULTIPLE_RESULTS";
        public const string ERR_FILE_NOT_FOUND = "ERR_FILE_NOT_FOUND";
    }

    public class Metadata
    {
        public const string tString = "string";
        public const string tIndexedString = "istring";
        public const string tDouble = "double";
        public const string tBoolean = "bool";
        public const string tInt32 = "int";
        public const string tInt64 = "long";
        public const string tDateTime = "datetime";
        public const string tDate = "date";
        public const string tTime = "time";
        public const string tBinaryData = "bin";
        public const string tList = "list";
    }

}
