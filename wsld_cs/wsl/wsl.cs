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

        public static void InstallImage(string tar_path, string install_path, string distroname, int version)
        {

            Console.WriteLine("Installing image:  " + distroname);
            Console.WriteLine("Path: " + install_path);
            System.IO.Directory.CreateDirectory(install_path);
            //--import <Distro> <UbicaciónDeInstalación> <NombreDeArchivo> --version <1,2>
            if (version==1 || version==2)
                Commands.RunProgram("wsl --import " + distroname + " " + install_path + " " + tar_path + " --version " + version);
            else
                Commands.RunProgram("wsl --import " + distroname + " " + install_path + " " + tar_path);

            Console.WriteLine("Done!");
        }
    }
}
