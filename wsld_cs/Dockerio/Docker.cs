using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using wsld_cs.Dockerio;
using wsld_cs.Processes;
using wsld_cs.Files;
using wsld_cs.Linux;
using wsld_cs.wsl;
using wsld.Utils;

namespace wsld_cs
{
    public class Docker
    {
        public static int DownloadImage( )
        {

            var layers = HttpRequests.AskForLayers();

            Console.WriteLine("Image with " + layers.Count() + " layers");
            Console.WriteLine("Downloading...");

            for (int i = 0; i < layers.Count(); ++i)
            {
                var layer = layers.ElementAt(i);
                Fops.WriteToFileAppend(HttpRequests.DownloadLayer(layer["digest"].ToString()), UserConfig.w_tmp_rootfs_path);
                Console.WriteLine("Downloaded " + i + 1 + " of " + layers.Count()+ " on: "+ UserConfig.w_tmp_rootfs_path);
            }

            Console.WriteLine("Finished! All layers downloaded.");
            return layers.Count();
        }



        public static void DownloadAndGenerateImage()
        {
            string distro_name  = UserConfig.wsld_distro_name;
            int count = DownloadImage();
            if(count > 1)
            {
                Linux_Commands.GenerateRootfsTar();
                File.Delete(UserConfig.w_tmp_rootfs_path);
            }
            else
            {
                File.Move(UserConfig.w_tmp_rootfs_path, UserConfig.w_rootfs_path);
            }
        }



        public static bool DockerLogin()
        {
            Console.WriteLine("\nFor using this feature you must login to Docker");
            Console.WriteLine();
            Console.WriteLine("username: ");
            string user = Console.ReadLine();
            Console.WriteLine("password: ");
            var password = Utils.ReadLineMasked();
            return Commands.DockerLogin("wsld", user, password);
        }


        public static void InstallWsldImage()
        {
            Console.WriteLine("WSLD Image will be installed.");
            Console.WriteLine("This image is needed to perform all the operations of this tool");
            Console.WriteLine("Touching the contents of the image is not recomended.");
            UserConfig.generateConfigs("rucadi/wsld:wsld", "", "wsld", 0);
            DownloadAndGenerateImage();
            Wsl.InstallImage();

        }

    }
}
