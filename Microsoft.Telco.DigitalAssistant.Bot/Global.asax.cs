namespace Microsoft.Telco.DigitalAssistant.Bot
{
    using Microsoft.ApplicationInsights;
    using Microsoft.Telco.DigitalAssistant.Logging;
    using System.Web.Http;


    public class WebApiApplication : System.Web.HttpApplication
    {
        public static TelemetryClient Telemetry { get; } = new TelemetryClient();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            LoggingManager.Initialize();

        }
    }
}
