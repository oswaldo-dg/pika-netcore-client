using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.NetCore.Client
{
    public class PikaAPIResult<T> where T : class
    { 
        public PikaAPIResult()
        {
            Success = false;
            Error = null;
            ErrorCode = null;
        }

        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }
        public T Payload { get; set; }
    }


    public class PikaAPICommand
    {
        public PikaAPICommand()
        {
            Success = false;
            Error = null;
            ErrorCode = null;
        }

        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string Error { get; set; }
     }
}
