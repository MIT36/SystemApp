using System.Diagnostics;
using System.IO;

namespace InstallService
{
    public static class ExtProcess
    {
        private static Process CreateProcessAndStart(string path, string args)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            proc.Start();

            return proc;
        }

        public static void StartAndWaitForExit(string path, string args, out string output, out int exitCode)
        {
            var process = CreateProcessAndStart(path, args);
            output = process.StandardOutput.ReadToEnd();
            exitCode = process.ExitCode;
            process.WaitForExit();
        }
    }
}
