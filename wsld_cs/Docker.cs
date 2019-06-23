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

namespace wsld_cs
{
    public class Docker
    {
        const string docker_pull_api_url = "http://auth.docker.io/token?service=registry.docker.io&scope=repository:";
        const string docker_register_url = "https://registry-1.docker.io/v2/";
        static  HttpClient client;



       public  Docker()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            client = new HttpClient(handler);
        }

        static async Task<string> GetResponse(string uri)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return "";
        }

        static async Task<byte[]> GetResponseBytes(string uri)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions
            byte[] responseBody = { };
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                 responseBody = await response.Content.ReadAsByteArrayAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return responseBody;
        }


        public void SetupAuth(string repository)
        {
            Console.WriteLine(docker_pull_api_url + repository + ":pull");
            string access_token = JObject.Parse(GetResponse(docker_pull_api_url + repository + ":pull").Result)["access_token"].ToString();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.docker.distribution.manifest.v2+json"));
        }

        public JObject AskForLayers(string repository, string tag )
        {

            var url = docker_register_url + repository +"/manifests/"+tag;
            Console.WriteLine(url);
            return JObject.Parse(GetResponse(url).Result);
        }

        public JObject GetBlobs(string config, string repository)
        {
            string url = docker_register_url + repository+"/blobs/" + config;
            Console.WriteLine(url);
            return JObject.Parse(GetResponse(url).Result);
        }

        public byte[] DownloadLayer(string digest, string repository)
        {
            string url = docker_register_url + repository + "/blobs/" + digest;
            return GetResponseBytes(url).Result;

        }


        public void WriteToFile(byte[] binary, string path)
        {
            Stream writingStream = new FileStream(path, FileMode.Append);
            writingStream.Write(binary, 0, binary.Length);
            writingStream.Close();
        }

        public void GenerateTar(string temp_path, string temp_name, string copyback_path)
        {

            string[] commands =
                {
                    "cd "+UserConfig.Path_win_to_wsl(UserConfig.rootfs_path),
                    "mkdir -p "+temp_path,
                    "rm -rf "+temp_path+"*",
                    "cp ./"+temp_name+" "+temp_path,
                    "cd "+temp_path,
                    "tar xfi "+temp_name+" --directory "+temp_path,
                    "tar cf rootfs.tar.gz *",
                    "cp rootfs.tar.gz "+copyback_path,
                    "rm -rf *"
                };

            var command = "bash -c \"";

            foreach ( var cmd in commands)
            {
                command += (cmd + " && ");
            }

            command += "echo 'img created'\"";

            RunProgram(command);
        }


        public void RunProgram(string commandToExecute)
        {
            // Execute wsl command:
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                }
            })
            {
                proc.Start();
                proc.StandardInput.WriteLine(commandToExecute);
                proc.StandardInput.Flush();
                proc.StandardInput.Close();
                proc.WaitForExit(); // wait up to 5 seconds for command to execute
            }
        }

        public void DebugTest()
        {
            //string defaultrepo = "library";


            string repo = "tianon";
            string imagetag= "qemu";


            string repository = repo + "/" + imagetag;



            SetupAuth(repository);

            var get_layers = AskForLayers(repository, "latest");

            var layers = get_layers["layers"];

            string config = get_layers["config"]["digest"].ToString();
            var blobs = GetBlobs(config, repository);


            string temp_file_path = UserConfig.rootfs_path + "temp_rootfs.tar.gz";
            foreach (var layer in layers)
            {
                WriteToFile(DownloadLayer(layer["digest"].ToString(), repository), temp_file_path);
            }
        
            GenerateTar("/tmp/wsld/", "temp_rootfs.tar.gz", "/mnt/c/temp/");
            File.Delete(temp_file_path);

        }





    }
}
