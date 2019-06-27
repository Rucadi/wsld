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

        static public string Untar_rootfs_joined(string name, string untar_path)
        {
            return "tar xfi " + name + " -C " + untar_path + "/ --exclude=/mnt --exclude=/dev --exclude=/proc --exclude=/tmp || :";
        }

        static public string Tar_rootfs(string distro_name, string appendix)
        {
           return "tar cf " + distro_name + "_" + appendix + "_rootfs.tar.gz * || :";
        }

        static public string RootfsName(string distro_name)
        {
            return distro_name + "_" + UserConfig.session_id + "_rootfs.tar.gz";
        }

        static public string TemporalRootfsName(string distro_name)
        {
            return distro_name + "_" + UserConfig.session_id + "_temp_rootfs.tar.gz";
        }
        static public string EraseDirectory(string path)
        {
            return "chmod -R 777 " + path + "&&  rm -rf " + path;
        }


        static public string GenerateCommand(string[] commands)
        {
            var command = "bash -c \"";

            foreach (var cmd in commands)
            {
                command += (cmd + " && ");
            }
            command += ": \"";

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
            Console.WriteLine(command);
            Console.WriteLine("Generating tar...");
            Commands.RunProgram(command);
            Console.WriteLine("Generated.");

        }

    }
}
