namespace DIP.SoapServices
{
    /// <summary>
    /// Configuration for all SOAP services used in the application
    /// </summary>
    public class SoapServicesConfiguration
    {
        /// <summary>
        /// NOC Service configuration (StgDipService)
        /// </summary>
        public NOCServiceConfig NOCService { get; set; }
    }

    /// <summary>
    /// Configuration for NOC SOAP Service
    /// WSDL: https://dbconnecttest.dipark.com/test/NocService.svc?wsdl
    /// Version: StgDipService (Staging)
    /// </summary>
    public class NOCServiceConfig
    {
        /// <summary>
        /// SOAP service endpoint URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Application key for service authentication
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// Timeout in seconds for service calls (default: 30)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Maximum buffer size for messages (default: int.MaxValue)
        /// </summary>
        public int MaxBufferSize { get; set; } = int.MaxValue;

        /// <summary>
        /// Maximum received message size (default: int.MaxValue)
        /// </summary>
        public long MaxReceivedMessageSize { get; set; } = int.MaxValue;
    }
}
