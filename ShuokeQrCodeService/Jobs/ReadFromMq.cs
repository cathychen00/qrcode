using System;
using System.Threading;
using log4net;
using RuntimeLib.Utils;
using ShuokeQrCodeService.Dto;
using ShuokeQrCodeService.Utils;

namespace ShuokeQrCodeService.Jobs
{
    public class ReadFromMq
    {
        private static ILog logger = Log4netManager.GetLogger("toClient");
        public static void Start()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Read();

                    Thread.Sleep(1 * 1000);
                }
            });

            t.Start();
        }

        private static void Read()
        {
            try
            {
                var mqserver = ConfigHelper.RabbitMqServer;
                var mqname = ConfigHelper.MqName;

                MqUtils.GetRabbitMq<BlogArticleDto>(mqserver, mqname, action: Action);
            }
            catch (Exception e)
            {
                logger.Error(e);

                throw;
            }
           
        }


        static readonly Action<BlogArticleDto> Action = (t) =>
        {
            try
            {
                var result = new FilterImage().FilterContentQRcode(t);
#warning process qrcode result
                if (result != null)
                {
                    foreach (var qrCodeInfo in result)
                    {
                        logger.Debug(qrCodeInfo.ImgUrl);
                        logger.Debug(qrCodeInfo.DecodeInfo);
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        };

    }


}
