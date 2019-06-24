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
using cmd;
using wsld_cs.Dockerio;
using wsld_cs.Processes;
using wsld_cs.Files;
using wsld_cs.Linux;

namespace wsld_cs
{
    public class Docker
    {
        const string docker_pull_api_url = "http://auth.docker.io/token?service=registry.docker.io&scope=repository:";
        const string docker_register_url = "https://registry-1.docker.io/v2/";


        private static void SetupAuth(string repository)
        {
            //Console.WriteLine(docker_pull_api_url + repository + ":pull");
            var res = HttpRequests.GetResponseJson(docker_pull_api_url + repository + ":pull").Result;
            HttpRequests.ClientSetBearerAndType(res["access_token"].ToString(), "application/vnd.docker.distribution.manifest.v2+json");

        }

        private static JObject AskForLayers(string repository, string tag )
        {

            var url = docker_register_url + repository +"/manifests/"+tag;
            //Console.WriteLine(url);
            return HttpRequests.GetResponseJson(url).Result;
        }

        private static JObject GetBlobs(string config, string repository)
        {
            string url = docker_register_url + repository+"/blobs/" + config;
            //Console.WriteLine(url);
            return HttpRequests.GetResponseJson(url).Result;
        }

        private static byte[] DownloadLayer(string digest, string repository)
        {
            string url = docker_register_url + repository + "/blobs/" + digest;
            return HttpRequests.GetResponseBytes(url).Result;

        }


 

        private static  void GenerateTar(string linux_temporary_folder, string tmp_rootfs_name, string distro_name)
        {
            string session_id = UserConfig.session_id;
            string win10_temporary_folder = UserConfig.linux_windows_temp_path;
            string win10_imagepath = UserConfig.linux_windows_image_path;

            string[] commands2 =
            {
                   Linux_Commands.Change_directory(win10_temporary_folder),
                   Linux_Commands.Create_directory_tree(linux_temporary_folder),
                   Linux_Commands.CopyFile(tmp_rootfs_name, linux_temporary_folder),
                   Linux_Commands.Change_directory(linux_temporary_folder),
                   Linux_Commands.Untar_rootfs_joined(tmp_rootfs_name, linux_temporary_folder),
                   Linux_Commands.EraseFile(tmp_rootfs_name),
                   Linux_Commands.Tar_rootfs(distro_name, session_id),
                   Linux_Commands.Create_directory_tree(win10_imagepath),
                   Linux_Commands.CopyFile(Linux_Commands.RootfsName(distro_name, session_id), win10_imagepath),
                   Linux_Commands.EraseDirectory(linux_temporary_folder),
                   Linux_Commands.Change_directory(win10_temporary_folder),
                   Linux_Commands.EraseFile(tmp_rootfs_name)

             };

            var command = Linux_Commands.GenerateCommand(commands2);
           // Console.WriteLine(command);
            Commands.RunProgram(command);
        }


        //string config = get_layers["config"]["digest"].ToString();
        //var blobs = GetBlobs(config, repository);
        public static string DownloadAndGenerateImage(string repo, string image, string tag, string distro_name)
        {
            string session_id = UserConfig.session_id;
            string repository = repo + "/" + image;


            SetupAuth(repository);


            string temp_rootfs_name = Linux_Commands.TemporalRootfsName(distro_name, session_id);
            string win10_temp_file_path = UserConfig.windows_temp_path + temp_rootfs_name;



            var layers = AskForLayers(repository, tag)["layers"];

            Console.WriteLine("Image with " + layers.Count() + " layers");
            Console.WriteLine("Downloading...");
            int i = 1;
            foreach (var layer in layers)
            {
                Fops.WriteToFileAppend(DownloadLayer(layer["digest"].ToString(), repository), win10_temp_file_path);
                Console.WriteLine("Downloaded " + i++ + " of " + layers.Count());
            }
            Console.WriteLine("Downloaded.");

            var linux_temporary_folder = "/tmp/wsld/" + distro_name + session_id;
            Console.WriteLine("Generating tar...");
            GenerateTar(linux_temporary_folder, temp_rootfs_name, distro_name);
            Console.WriteLine("Generated.");
            File.Delete(win10_temp_file_path);

            return  Path.Combine(UserConfig.windows_image_path,Linux_Commands.RootfsName(distro_name, session_id));
        }





    }
}
