using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallService
{
    class Program
    {
        static string GetExeFile(string name) => $"{name}.exe";

        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                string workerService = GetExeFile(nameof(WorkerService));
                string systemApp = GetExeFile(nameof(SystemApp));

                ElevateAccess(args);

                Console.WriteLine($"{args?.Length}");
                if(args?.Length > 0 && (args[0] == "-u" || args[0] == "/u" || args[0] == "-uninstall" || args[0] == "/uninstall"))
                {
                    //UnInstallAndStop(workerService, systemApp);
                    Console.WriteLine("UNINSTALL");
                }
                else
                {
                    //CopyFiles(workerService, systemApp);
                    //InstallAndRun(workerService);
                    Console.WriteLine("Install...");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
                Console.WriteLine("Press any key for exit...");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            Console.WriteLine("Press any key for exit...");
            Console.ReadKey();
        }

        static void ElevateAccess(string[] args)
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                var processArgs = string.Join(" ", args);
                var elevated = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, processArgs)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(elevated);
                }
                catch
                {
                    throw new Exception("Administrator rights required!");
                }
                Environment.Exit(-1);
            }
        }

        static void CopyFiles(string service, string app)
        {
            if (!File.Exists(service) && !File.Exists(app))
            {
                throw new Exception("An error occured! No required files!");
            }

            Console.WriteLine("Copying files...");

            File.Copy(service, Path.Combine(Environment.SystemDirectory, service), true);
            File.Copy(app, Path.Combine(Environment.SystemDirectory, app), true);

            Console.WriteLine("Copying completed successfully!");
        }

        static void InstallAndRun(string workerService)
        {
            //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe "C:\WorkerService\WorkerService\bin\x64\Release\WorkerService.exe"
            ExtProcess.StartAndWaitForExit(Path.Combine(Environment.SystemDirectory, workerService),
                    "/install",
                    out var output, out var exitCode);
            //logger.LogInformation($"cmd Success");
            if (exitCode != 0)
                throw new Exception(output);

            Console.WriteLine("Service installation is complete! The service has started!");
        }

        static void UnInstallAndStop(string workerService, string systemApp)
        {
            ExtProcess.StartAndWaitForExit(Path.Combine(Environment.SystemDirectory, workerService),
                    "/uninstall",
                    out var output, out var exitCode);
            //logger.LogInformation($"cmd Success");
            if (exitCode != 0)
                throw new Exception(output);

            Console.WriteLine("The service has been stopped and deleted!");
            var fullPathService = Path.Combine(Environment.SystemDirectory, workerService);
            var fullPathApp = Path.Combine(Environment.SystemDirectory, systemApp);
            if (File.Exists(fullPathService))
            {
                File.Delete(fullPathService);
                Console.WriteLine($"System file: '{workerService}' deleted!");
            }
            if (File.Exists(fullPathApp))
            {
                File.Delete(fullPathApp);
                Console.WriteLine($"System file: '{systemApp}' deleted!");
            }
        }
        
    }
}
