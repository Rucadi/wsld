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
using wsld.Dockerio;

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
                Console.WriteLine("Downloaded " + (i + 1) + " of " + layers.Count());
            }

            Console.WriteLine("Finished! All layers downloaded.");
            return layers.Count();
        }







        public static bool DockerLogin()
        {
            return DockerLogin(null, null);
        }

        public static bool DockerLogin(string user = null, string password = null)
        {
            if(user!=null && password!=null)
            {
                return Commands.DockerLogin(user, password);
            }

            Console.WriteLine("\nFor using this feature you must login to Docker");
            Console.WriteLine();
            if (user == null)
            {
                Console.WriteLine("username: ");
                user = Console.ReadLine();
            }else
            {
                Console.WriteLine("username: \n"+user+"\n");
            }

            if (password == null)
            {
                Console.WriteLine("password: ");
                password = Utils.ReadLineMasked();
            }
            else
            {
                Console.WriteLine("password: ******");
            }
            return Commands.DockerLogin(user, password);
        }




        public static void InstallWsldImage()
        {
            Console.WriteLine("WSLD Image will be installed.");
            Console.WriteLine("This image is needed to perform all the operations of this tool");
            Console.WriteLine("Touching the contents of the image is not recommended.");
            UserConfig.generateConfigs("rucadi/wsld:wsld", "", "wsld", 0);
            DownloadImage();
            File.Move(UserConfig.w_tmp_rootfs_path, UserConfig.w_rootfs_path);
            Wsl.InstallImage();
        }

    }
}
