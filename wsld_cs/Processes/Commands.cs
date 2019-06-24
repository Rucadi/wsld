using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld_cs.Processes
{
    class Commands
    {
        public static void RunProgram(string commandToExecute)
        {
            // Execute wsl command:
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                }
            })
            {
                proc.Start();
                proc.StandardInput.WriteLine(commandToExecute);
                proc.StandardInput.Flush();
                proc.StandardInput.Close();
                proc.WaitForExit(); // wait up to 5 seconds for command to execute
            }
        }


        public static string wslpath(string WinPath)
        {
            string[] sep = WinPath.Split('\\');
            string wspath = "/mnt/";
            sep[0] = sep[0].Substring(0, 1).ToLower();

            string separator = "/";

            foreach (var pdir in sep)
            {
                wspath += pdir;
                wspath += separator;
            }


            return wspath;


        }
    }
}
