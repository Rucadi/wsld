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

namespace wsld_cs
{
   

    class Program
    {


        static void Main(string[] args)
        {
            if (!Wsl.CheckWsldImage())
            {
                Docker.InstallWsldImage();
                return;
            }

            string firstArg = args.Length > 0 ? args[0] : "";

           
            switch(firstArg)
            {
                case "docker":
                    args = args.Skip(1).ToArray();
                    var doptions = Parser.Default.ParseArguments<DockerOptions>(args);
                    doptions.WithParsed(options => { ParsedDockerMain(options); });

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
            Docker.DownloadAndGenerateImage();
            Wsl.InstallImage();
        }


        private static void ParsedDockerMain(DockerOptions options)
        {
            if (options.login)
                if (Docker.DockerLogin())
                    Console.WriteLine("Succeed!");
                else Console.WriteLine("User or Password incorrect");
            
        }
    }
}
