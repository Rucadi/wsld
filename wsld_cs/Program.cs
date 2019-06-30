using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Net.Http;
using wsld_cs.wsl;
using System.IO;
using wsld_cs.Processes;
using wsld_cs.Linux;
using System.Security;
using wsld.Params;
using wsld.Utils;
using System.Runtime.InteropServices;
using wsld.Dockerio;

namespace wsld_cs
{
   

    class Program
    {

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            System.Environment.Exit(1);
            return true;
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Commands.SetDefaultDistro(UserConfig.default_distro);
        }



        static void DockerTree(string[] args)
        {
            string firstArg = args.Length > 0 ? args[0] : "";

            if(firstArg.Equals("login"))
            {
                Parser.Default.ParseArguments<DockerLoginOptions>(args).WithParsed(options => {
                    if (Docker.DockerLogin(options.User, options.Password))
                        Console.WriteLine("Succeed!");
                    else Console.WriteLine("User or Password incorrect");
                });

                return;
            }
            else if (!UserConfig.isLoggedToDocker)
            {
                if(Docker.DockerLogin(null, null))
                    Console.WriteLine("Login Succeed!, rerun the command!");
                else Console.WriteLine("User or Password incorrect");
                return;
            }


            if (firstArg.Equals("upload"))
            {
                Parser.Default.ParseArguments<DockerUploadOptions>(args).WithParsed(options => {
                    UserConfig.generateConfigs(options.Dockerimage, "", options.Distroname, 0);

                    Console.WriteLine(DockerInterop.wslToDockerHub());
                });

                return;
            }

            Console.WriteLine("Command must be used followed by login or upload");
            Console.WriteLine("Example:");
            Console.WriteLine("wsld docker login");
            Console.WriteLine("wsld docker upload");


        }


        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            if (!Wsl.CheckWsldImage())
            {
                Docker.InstallWsldImage();
                return;
            }

            string firstArg = args.Length > 0 ? args[0] : "";

            UserConfig.default_distro = Commands.GetDefaultDistro();
            UserConfig.isLoggedToDocker = Commands.IsLoggedToDocker();

            switch (firstArg)
            {
                case "docker":
                    DockerTree(args.Skip(1).ToArray());
                    break;


                default:
                        var cmdOptions = Parser.Default.ParseArguments<DefaultOptions>(args);
                        cmdOptions.WithParsed(options => { ParsedMain(options); });
                    break;
            }
           
            

  
        }

        private static void ParsedMain(DefaultOptions options)
        {
            UserConfig.generateConfigs(options.Dockerimage, options.InstallDir, options.Distroname, options.Version);
            DockerInterop.DownloadImageUsingDocker();
            Wsl.InstallImage();
        }

    }
}
