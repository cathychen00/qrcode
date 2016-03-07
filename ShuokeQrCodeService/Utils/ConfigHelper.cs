using System.Configuration;

namespace ShuokeQrCodeService.Utils
{
    public class ConfigHelper
    {
        public static string RabbitMqServer = ConfigurationManager.AppSettings["RabbitMqServer"];
        public static string MqName = ConfigurationManager.AppSettings["MqName"];
    }
}
