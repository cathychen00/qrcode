using System;
using System.Text;
using log4net;
using RabbitMQ.Client;

namespace ShuokeQrCodeService.Utils
{
    public class MqUtils
    {
        static ILog log = LogManager.GetLogger("qrcode");
        public static void GetRabbitMq<T>(string rabbitMqServerngs, string mqname, Action<T> action)
        {

            try
            {
                var cf = new ConnectionFactory { Uri = rabbitMqServerngs };

                using (var conn = cf.CreateConnection())
                using (var model = conn.CreateModel())
                {

                    while (true)
                    {
                        var res = model.BasicGet(mqname, false);

                        if (res != null)
                        {
                            try
                            {
                                var str = Encoding.UTF8.GetString(res.Body).Trim();
                                log.Info(str.Trim());

                                var cmodel = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);

                                action(cmodel);

                                model.BasicAck(res.DeliveryTag, false);
                            }
                            catch (Exception e)
                            {

                                log.Error(DateTime.Now + " " + e.Message, e);
                                model.BasicAck(res.DeliveryTag, false);
                            }
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

        }


        public static void GetOneMessage<T>(string rabbitMqServerngs, string mqname, Action<T> action)
        {

            try
            {
                var cf = new ConnectionFactory { Uri = rabbitMqServerngs };

                using (var conn = cf.CreateConnection())
                using (var model = conn.CreateModel())
                {
                    var res = model.BasicGet(mqname, false);

                    if (res != null)
                    {
                        try
                        {
                            var str = Encoding.UTF8.GetString(res.Body).Trim();
                            log.Info(str.Trim());

                            var cmodel = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);

                            action(cmodel);

                            model.BasicAck(res.DeliveryTag, false);
                        }
                        catch (Exception e)
                        {

                            log.Error(DateTime.Now + " " + e.Message, e);
                            model.BasicAck(res.DeliveryTag, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

        }




        public static void SendMessage<T>(T obj, string path, string queue, string exchange = "defaultexchange", string routingkey = "defaultroutingkey")
        {

            var cf = new ConnectionFactory { Uri = path };

            try
            {
                using (var conn = cf.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

                    channel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
                    channel.QueueDeclare(queue, true, false, false, null);
                    channel.QueueBind(queue, exchange, routingkey, null);
                    channel.BasicPublish(exchange,
                        routingkey,
                        null,
                        Encoding.UTF8.GetBytes(message));

                    log.InfoFormat(" [x] Sent {0}", message);

                }
            }
            catch (Exception ex)
            {
                log.Error("MQ Send Error:", ex);
            }
        }
    }
}
