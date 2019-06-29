using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsld_cs.Processes;

namespace wsld_cs.wsl
{
    class Wsl
    {

        public static void InstallImage()
        {

            Console.WriteLine("Installing image:  " + UserConfig.wsld_distro_name);
            Console.WriteLine("Path: " + UserConfig.image_install_dir);
            System.IO.Directory.CreateDirectory(UserConfig.image_install_dir);

            //--import <Distro> <UbicaciónDeInstalación> <NombreDeArchivo> --version <1,2>
            if (UserConfig.wsld_version == 1 || UserConfig.wsld_version == 2)
                Commands.RunProgram("wsl --import " + UserConfig.wsld_distro_name + " " + UserConfig.image_install_dir + " " + UserConfig.w_rootfs_path + " --version " + UserConfig.wsld_version);
            else
                Commands.RunProgram("wsl --import " + UserConfig.wsld_distro_name + " " + UserConfig.image_install_dir + " " + UserConfig.w_rootfs_path);

            Console.WriteLine("Done!");
        }


        public static bool CheckWsldImage()
        {
            var distrolist = Commands.getInstalledDistros();
            foreach (string distro in distrolist)
            {
                if (distro.Split((char)13)[0].Equals("wsld")) return true;
             }
            return false;
        }
    }
}
