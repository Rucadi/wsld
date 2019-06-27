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

        public static string RunProgramGetOutput(string commandToExecute)
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

               string output = proc.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                return output;
                
            }

        }


       public static List<string> getInstalledDistros()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c wsl -l"; // Note the /c command (*)
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();

            string stdoutput = process.StandardOutput.ReadToEnd();
            string processed_output = "";
            foreach (var ch in stdoutput)
                if (ch != 0) processed_output += ch;
            

            var output = processed_output.Split('\n');
            List<string> distro_list = new List<string>();
            for (int i = 1; i < output.Length; ++i)
            {
                var dsplit = output[i].Split(' ');
                distro_list.Add(dsplit[0]);
            }

            return distro_list;
        }



        public static bool DockerLogin(string distro, string username, string password)
        {
            Process process = new Process();
            process.StartInfo.FileName = "wsl.exe";
            process.StartInfo.Arguments = " -d "+ distro+ " service docker start";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
           // process.StartInfo.RedirectStandardInput = true;

            process.Start();
            process.WaitForExit();
            process.StartInfo.Arguments = " -d " + distro + " docker login --username " + username + " --password " + password;
            process.Start();
            process.WaitForExit();

            string rte =  process.StandardOutput.ReadToEnd();
            return rte.Contains("Login Succeeded");
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
