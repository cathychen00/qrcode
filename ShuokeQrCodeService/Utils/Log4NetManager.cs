using System;
using System.IO;
using log4net;
using log4net.Config;

namespace RuntimeLib.Utils
{
    public class Log4netManager
    {
        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/log4net.config";

        /// <summary>
        ///     ��̬���캯�������״�ʹ��log4netǰ������log4net����
        /// </summary>
        static Log4netManager()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        /// <summary>
        ///     ����name��ȡlogger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }

        public static ILog GetLogger(Type name)
        {
            return LogManager.GetLogger(name);
        }
    }
}