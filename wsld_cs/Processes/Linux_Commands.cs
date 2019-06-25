using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static public string RootfsName(string distro_name, string appendix)
        {
            return distro_name + "_" + appendix + "_rootfs.tar.gz";
        }

        static public string TemporalRootfsName(string distro_name, string appendix)
        {
            return distro_name + "_" + appendix + "_temp_rootfs.tar.gz";
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
    }
}
