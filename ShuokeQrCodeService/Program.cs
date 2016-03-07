using System;
using System.ServiceProcess;

namespace ShuokeQrCodeService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                new Service1().Start();
                Console.WriteLine(" Press anyKey to stop!");

                Console.Read();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
