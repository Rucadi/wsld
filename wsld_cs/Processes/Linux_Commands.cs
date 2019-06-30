using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsld_cs.Processes;

namespace wsld_cs.Linux
{
    class Linux_Commands
    {

        static public string StartDockerService()
        {
            return "service docker start";
        }

        static public string DockerPull(string rit)
        {
            return "docker pull " + rit;
        }

        static public string DockerSave(string rit, string path)
        {
            return "docker save " + rit + " -o " + path;
        }

        static public string UnTar(string path)
        {
            return "tar xf "+path;
        }

        static public string UnTarAllFilesThatMatchesIntoFolder(string pattern, string searchPath, string outPath)
        {
            return "find " + searchPath + " -type f -name  \"" + pattern + "\" -exec tar xf {} -C " + outPath + " \\; ";
        }

        static public string Change_directory(string path)
        {
            return "cd " + path;
        }

        static public string EraseFile(string name)
        {
            return "rm " + name;
        }
        static public string Create_directory_tree(string path)
        {
            return "mkdir -p " + path;
        }

        static public string CopyFile(string from, string to)
        {
            return "cp " + from + " " + to;
        }
        static public string MoveFile(string from, string to)
        {
            return "mv " + from + " " + to;
        }
        static public string Untar_rootfs_joined(string name, string untar_path)
        {
            return "tar xfi " + name + " -C " + untar_path + "/ --exclude=/mnt --exclude=/dev --exclude=/proc --exclude=/tmp || :";
        }

        static public string Tar_rootfs(string distro_name, string appendix)
        {
           return "tar cf " + distro_name + "_" + appendix + "_rootfs.tar.gz * || :";
        }


        static public string EraseDirectory(string path)
        {
            return "chmod -R 777 " + path + "&&  rm -rf " + path;
        }


        static public string GenerateCommand(string[] commands)
        {
            var command = "";
            foreach (var cmd in commands)
            {
                command += (cmd + ";");
            }
            return command;
        }


    


        public static void GenerateRootfsTar()
        {
            string session_id                = UserConfig.session_id;
            string win10_temporary_folder    = UserConfig.wsl_windows_temp_path;
            string win10_imagepath           = UserConfig.wsl_windows_image_path;
            string linux_temporary_folder    = UserConfig.linux_temporary_folder;
            string tmp_rootfs_name           = UserConfig.tmp_rootfs_name;
            string distro_name               = UserConfig.wsld_distro_name;

            string[] commands2 =
            {
                   Change_directory(win10_temporary_folder),
                   Create_directory_tree(linux_temporary_folder),
                   CopyFile(tmp_rootfs_name, linux_temporary_folder),
                   Change_directory(linux_temporary_folder),
                   Untar_rootfs_joined(tmp_rootfs_name, linux_temporary_folder),
                   EraseFile(tmp_rootfs_name),
                   Tar_rootfs(distro_name, session_id),
                   Create_directory_tree(win10_imagepath),
                   CopyFile(UserConfig.rootfs_name, win10_imagepath),
                   EraseDirectory(linux_temporary_folder),
                   Change_directory(win10_temporary_folder),
                   EraseFile(tmp_rootfs_name)

             };

            var command = GenerateCommand(commands2);
            Console.WriteLine("Generating tar...");
            Commands.BashRunCommand_stdout(command);
            Console.WriteLine("Generated.");

        }

    }
}
