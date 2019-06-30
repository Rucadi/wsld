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



        static bool DockerUpload(string[] args)
        {
            string firstArg = args.Length > 0 ? args[0] : "";

            Parser.Default.ParseArguments<DockerUploadOptions>(args).WithParsed(options => {
                UserConfig.generateConfigs(options.Dockerimage, "", options.Distroname, 0);

                Console.WriteLine(DockerInterop.wslToDockerHub());
            });

            return true;

        }


        static bool DockerBuild(string[] args)
        {
            string firstArg = args.Length > 0 ? args[0] : "";

            Parser.Default.ParseArguments<DockerfileOptions>(args).WithParsed(options => {
                UserConfig.generateConfigs("wsld/wsld_dockerfile:image", "", options.Distroname, 0);
                UserConfig.dockerfile_path = options.Dockerfile;

                Console.WriteLine("Building image...");


                if (DockerInterop.BuildDockerfile(options.remote))
                        Console.WriteLine("Image created correctly... installing...");
                else
                {
                    Console.WriteLine("Image generation failed...");
                }
                DockerInterop.DownloadImageUsingDocker();
                Wsl.InstallImage();
            });

            return true;

        }

        static bool DockerLogin(string[] args)
        {
            string firstArg = args.Length > 0 ? args[0] : "";

            bool retval = false;
            Parser.Default.ParseArguments<DockerLoginOptions>(args).WithParsed(options => {
                if (Docker.DockerLogin(options.User, options.Password))
                {
                    Console.WriteLine("Succeed!");
                    retval = true;
                }
                else {
                    Console.WriteLine("User or Password incorrect");
                }
            });
            return retval;
        }

        static bool DockerLogin_required()
        {
            if (Docker.DockerLogin(null, null))
            {
                Console.WriteLine("Login Succeed!, rerun the command!");
                return true;
            }

            Console.WriteLine("User or Password incorrect");
            return false;
        }

        static bool DockerTree(string[] args)
        {
            string firstArg = args.Length > 0 ? args[0] : "";



            if (firstArg.Equals("login")) return DockerLogin(args);
            if (!UserConfig.isLoggedToDocker) return DockerLogin_required();
            if (firstArg.Equals("upload")) return DockerUpload(args);
            if (firstArg.Equals("build")) return DockerBuild(args);

            Console.WriteLine("Command must be used followed by login or upload");
            Console.WriteLine("Example:");
            Console.WriteLine("wsld docker login");
            Console.WriteLine("wsld docker upload");

            return false;

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
                        cmdOptions.WithParsed(options => { DefaultOptionsMain(options); });
                    break;
            }
           
            

  
        }


        private static void DefaultOptionsMain(DefaultOptions options)
        {
            UserConfig.generateConfigs(options.Dockerimage, options.InstallDir, options.Distroname, options.Version);
            DockerInterop.DownloadImageUsingDocker();
            Wsl.InstallImage();
        }

    }
}
