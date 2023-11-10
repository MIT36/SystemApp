using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService
{
    static class AppHelper
    {
        public static string GetAppName() => "SystemApp";
        public static string GetExeApp() => $"{GetAppName()}.exe";
        public static string GetPathExeApp() => Path.Combine(Environment.SystemDirectory, GetExeApp());


        private static bool IsInstalled(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
        }

        private static bool IsRunning(string serviceName)
        {
            using (ServiceController controller = new ServiceController("YourServiceName"))
            {
                if (!IsInstalled(serviceName)) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(Service).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        public static void InstallService(string serviceName)
        {
            if (IsInstalled(serviceName)) return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void UninstallService(string serviceName)
        {
            if (!IsInstalled(serviceName)) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void StartService(string serviceName)
        {
            if (!IsInstalled(serviceName)) return;

            using (ServiceController controller =
                new ServiceController(serviceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running,
                            TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void StopService(string serviceName)
        {
            if (!IsInstalled(serviceName)) return;
            using (ServiceController controller =
                new ServiceController(serviceName))
            {
                try
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped,
                             TimeSpan.FromSeconds(10));
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
