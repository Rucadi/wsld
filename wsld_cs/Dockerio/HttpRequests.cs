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
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(0);
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
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(0);
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
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find the image. exiting...");
                Environment.Exit(0);
            }
            return responseBody;
        }


    }
}
