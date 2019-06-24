using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Net.Http;
using wsld_cs.wsl;
using System.IO;

namespace wsld_cs
{
   

    class Program
    {

        public class CmdOptions
        {
            [Option('o', "directory", Required = false, HelpText = "Directory to install.")]
            public string InstallDir { get; set; }

            [Option('i', "image", Required = true, HelpText = "Docker Image to Install.")]
            public string Dockerimage { get; set; }

            [Option('d', "distroname", Required = true, HelpText = "Name to give to the new distro")]
            public string Distroname { get; set; }

            [Option('v', "version", Required = false, HelpText = "Version for the new distro, the default is the wsl default, set 1 to WSL1, 2 to WSL2.")]
            public int Version { get; set; }
        }
        static void Main(string[] args)
        {
            var cmdOptions = Parser.Default.ParseArguments<CmdOptions>(args);
            cmdOptions.WithParsed(
                options => {
                    ParsedMain(options);
                });
        }




        private static string[] Get_Repo_Dist_Tag(string all)
        {

            string[] repository_distrotag = all.Split('/');
            int rlen = repository_distrotag.Length;
            string repository = rlen > 1 ? repository_distrotag[0] : "library";
            string distrotag = rlen > 1 ? repository_distrotag[1] : repository_distrotag[0];


            string[] distro_tag = distrotag.Split(':');
            int dlen = distro_tag.Length;

            string distro = distro_tag[0];
            string tag = dlen > 1 ? distro_tag[1] : "latest";


            string[] ret = { repository, distro, tag };
            return  ret ;

        }

        private static void ParsedMain(CmdOptions options)
        {
            string img = options.Dockerimage;
            var rdt = Get_Repo_Dist_Tag(img);

            string repo = rdt[0];
            string distroname = rdt[1];
            string tag = rdt[2];

            if(options.InstallDir!=null)
            UserConfig.install_dir = options.InstallDir;
            

            string install_tar_path = Docker.DownloadAndGenerateImage(repo, distroname, tag, options.Distroname);
            Wsl.InstallImage(install_tar_path, Path.Combine(UserConfig.install_dir,options.Distroname), options.Distroname, options.Version);

        }
    }
}
