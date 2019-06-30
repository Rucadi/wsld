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
                 Linux_Commands.EraseDirectory(wsl_tmp)
            };

            var ret = Commands.BashRunCommand(Linux_Commands.GenerateCommand(command_2));


            int pFrom = ret.IndexOf(marker) + marker.Length;
            int pTo = ret.LastIndexOf(marker);
            string result = ret.Substring(pFrom, pTo - pFrom);

            if (result.Contains("Pushed") || result.Contains("Mounted")  || result.Contains("exists"))
                return "Success.";


            return "Couldn't push the image to the repo";


        }


        public static bool DownloadImageUsingDocker()
        {
            string linux_temporary_folder = UserConfig.linux_temporary_folder;
            string temp_linux_path_extracted = linux_temporary_folder+"/image";
            string win10_imagepath = UserConfig.wsl_windows_image_path;

            string[] commands =
            {
                 Linux_Commands.Create_directory_tree(win10_imagepath),
                 Linux_Commands.Create_directory_tree(linux_temporary_folder),
                 Linux_Commands.Create_directory_tree(temp_linux_path_extracted),
                 Linux_Commands.Change_directory(linux_temporary_folder),
                 Linux_Commands.StartDockerService(),
                 Linux_Commands.DockerPull(UserConfig.repo_image_tag)
            };

            var command = Linux_Commands.GenerateCommand(commands);
            var res = Commands.BashRunCommand(command);
            if (res.Contains("docker login"))
            {
                Console.WriteLine("repository does not exist or may require 'wsld docker login'");
                Commands.BashRunCommand(Linux_Commands.EraseDirectory(linux_temporary_folder));
                System.Environment.Exit(0);
            }

            string[] commands2 =
            {
                 Linux_Commands.Change_directory(linux_temporary_folder),
                 Linux_Commands.DockerSave(UserConfig.repo_image,"docker"),
                 Linux_Commands.UnTar("docker"),
                 Linux_Commands.UnTarAllFilesThatMatchesIntoFolder("*.tar",linux_temporary_folder,temp_linux_path_extracted),
                 Linux_Commands.Change_directory(temp_linux_path_extracted),
                 Linux_Commands.Tar_rootfs(UserConfig.wsld_distro_name, UserConfig.session_id),
                 Linux_Commands.MoveFile(UserConfig.rootfs_name,win10_imagepath),
                 Linux_Commands.EraseDirectory(linux_temporary_folder)
            };
            var command2 = Linux_Commands.GenerateCommand(commands2);
            Console.WriteLine("Generating tar from docker...");
            Commands.BashRunCommand(command2);

            Console.WriteLine("Generated.");

            return true;

        }

    }
}
