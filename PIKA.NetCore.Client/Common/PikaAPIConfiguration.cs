namespace PIKA.NetCore.Client
{
    public class PikaAPIConfiguration
    {
        public string URLIdentity { get; set; }
        public string URLAPI { get; set; }
        public string APISecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string APIScope { get; set; }
        public string ClientId { get; set; }

        public bool Valid()
        {
            
            return !string.IsNullOrEmpty(URLIdentity) &&
                !string.IsNullOrEmpty(URLAPI) &&
                !string.IsNullOrEmpty(APISecret) &&
                !string.IsNullOrEmpty(Username) &&
                !string.IsNullOrEmpty(APIScope) &&
                !string.IsNullOrEmpty(ClientId) &&
                !string.IsNullOrEmpty(Password);

        }

    }
}
