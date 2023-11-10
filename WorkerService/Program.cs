using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {
            var service = new Service();
            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    service
                };
                ServiceBase.Run(ServicesToRun);
            }
            else if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "/install":
                        AppHelper.InstallService(service.ServiceName);
                        AppHelper.StartService(service.ServiceName);
                        break;
                    case "/uninstall":
                        AppHelper.StopService(service.ServiceName);
                        AppHelper.UninstallService(service.ServiceName);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
