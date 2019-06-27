using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace wsld_cs.Dockerio
{
    class HttpRequests
    {
        static HttpClient client = new HttpClient();


       public static void ClientSetBearerAndType(string bearer, string type)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(type));
        }

       public static async Task<JObject> GetResponseJson(string uri)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                JObject responseBody = JObject.Parse(await response.Content.ReadAsStringAsync());
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                return responseBody;
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(1);
            }
            return null;
        }

        public static async Task<string> GetResponse(string uri)
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
            catch (HttpRequestException)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(1);
            }
            return "";
        }


        public static async Task<byte[]> GetResponseBytes(string uri)
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
            catch (HttpRequestException)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(1);
            }
            return responseBody;
        }


        const string docker_pull_api_url = "http://auth.docker.io/token?service=registry.docker.io&scope=repository:";
        const string docker_register_url = "https://registry-1.docker.io/v2/";


        public static void SetupAuth()
        {
            var res = GetResponseJson(docker_pull_api_url + UserConfig.repo_image + ":pull").Result;
            ClientSetBearerAndType(res["access_token"].ToString(), "application/vnd.docker.distribution.manifest.v2+json");

        }

        public static JToken AskForLayers()
        {
            SetupAuth();
            var url = docker_register_url + UserConfig.repo_image + "/manifests/" + UserConfig.tag;
            return GetResponseJson(url).Result["layers"];
        }

        public static JObject GetBlobs(string config)
        {
            string url = docker_register_url + UserConfig.repo_image + "/blobs/" + config;
            return GetResponseJson(url).Result;
        }

        public static byte[] DownloadLayer(string digest)
        {
            string url = docker_register_url + UserConfig.repo_image + "/blobs/" + digest;
            return GetResponseBytes(url).Result;

        }


    }
}
