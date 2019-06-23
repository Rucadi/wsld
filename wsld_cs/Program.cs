using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Net.Http;

namespace wsld_cs
{
   

    class Program
    {

        public class CmdOptions
        {
            [Option('f', "file", Required = false, HelpText = "Path of the Dockerfile.")]
            public string Dockername { get; set; }

            [Option('t', "tag", Required = false, HelpText = "Name and optionally a tag in the ‘name:tag’ format.")]
            public string Tag { get; set; }

            [Option('p', "pull", Required = false, HelpText = "Always attempt to pull a newer version of the image.")]
            public bool Pull { get; set; }

            [Option('q', "quiet", Required = false, HelpText = "Suppress the build output and print image ID on success.")]
            public bool Quiet { get; set; }

            [Option('b', "build-arg", Required = false, HelpText = "Suppress the build output and print image ID on success.")]
            public string BuldARG { get; set; }

            [Option('q', "add-host", Required = false, HelpText = "Suppress the build output and print image ID on success.")]
            public string Hosts { get; set; }

        }
        static void Main(string[] args)
        {
            // or (2) build and configure instance
            var cmdOptions = Parser.Default.ParseArguments<CmdOptions>(args);
            cmdOptions.WithParsed(
                options => {
                    ParsedMain(options);
                });


            Console.ReadKey();

            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }

        private static void ParsedMain(CmdOptions options)
        {
            Console.WriteLine($"Name used {options.Dockername}");
            Console.WriteLine($"Name used {options.Tag}");

            Docker dr = new Docker();
            dr.DebugTest();

        }
    }
}
