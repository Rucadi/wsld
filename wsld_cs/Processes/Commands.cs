using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld_cs.Processes
{
    static class Commands
    {
        public static void SetDefaultDistro(string distro)
        {
            RunProgram("wsl --set-default " + distro);
        }

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
                proc.WaitForExit();
            }
        }

        public static string RunProgramGetOutput(string commandToExecute)
        {
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

        public static string RemoveLast(this string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }

        public static string BashRunCommand(string command)
        {
            SetDefaultDistro("wsld");

            Process process = new Process();
            process.StartInfo.FileName = "bash.exe";
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            process.StandardInput.Write(command);
            process.StandardInput.Close();
            process.WaitForExit();

            SetDefaultDistro(UserConfig.default_distro);
            var result = process.StandardOutput.ReadToEnd();
            result += process.StandardError.ReadToEnd();
            return RemoveLast(result, "\n");
        }


        public static bool DockerLogin(string username, string password)
        {
            string command = "service docker start ||  docker login --username " + username + " --password " + password;
            var res =  BashRunCommand(command);
            return res.Contains("Login Succeeded");
        }

        public static bool CheckIfFileExists(string linux_file_path)
        {
            string commandString = "if test -f '"+linux_file_path+"'; then echo 'ok'; else echo 'ko'; fi";
            return BashRunCommand(commandString).Equals("ok"); 
        }


        public static bool IsLoggedToDocker()
        {
            return CheckIfFileExists("/root/.docker/config.json");
        }

        

        public static string GetDefaultDistro()
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
 
           return output[1].Split(' ')[0];
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
