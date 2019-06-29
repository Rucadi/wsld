using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsld_cs;
using wsld_cs.Linux;
using wsld_cs.Processes;

namespace wsld.Dockerio
{
    class DockerInterop
    {
        public static string wslToDockerHub()
        {

            string wsl_tmp = UserConfig.linux_temporary_folder;

            string[] command_1 ={
                 Linux_Commands.Create_directory_tree(wsl_tmp),
                 Linux_Commands.Change_directory(wsl_tmp),
                 "wsl.exe --export "+UserConfig.wsld_distro_name+" import_docker.tar.gz"
            };

            Commands.BashRunCommand(Linux_Commands.GenerateCommand(command_1));

            if (!Commands.CheckIfFileExists(wsl_tmp + "/import_docker.tar.gz")) return "ERROR: The image could not be exported from WSL";

            string marker = "@@@@@@@@@@";

            string[] command_2 =
                {
                 Linux_Commands.Change_directory(wsl_tmp),
                 "service docker start",
                 "docker import import_docker.tar.gz " + UserConfig.repo_image_tag,
                 "echo "+marker,
                 "docker push " + UserConfig.repo_image_tag,
                 "echo "+marker,
                 "docker rmi $(docker images -q)",
                 "rm -rf "+wsl_tmp
                };

            var ret = Commands.BashRunCommand(Linux_Commands.GenerateCommand(command_2));


            int pFrom = ret.IndexOf(marker) + marker.Length;
            int pTo = ret.LastIndexOf(marker);
            string result = ret.Substring(pFrom, pTo - pFrom);

            if (result.Contains("Pushed") || result.Contains("Mounted")  || result.Contains("exists"))
                return "Success.";


            return "Couldn't push the image to the repo";


        }


    }
}
