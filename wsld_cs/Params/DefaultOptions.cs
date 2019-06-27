using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld.Params
{
    class DefaultOptions
    {
        [Option('o', "directory", Required = false, HelpText = "Directory to install.")]
        public string InstallDir { get; set; }

        [Option('i', "image", Required = true, HelpText = "Docker Image to Install.")]
        public string Dockerimage { get; set; }

        [Option('d', "distroname", Required = true, HelpText = "Name to give to the new distro")]
        public string Distroname { get; set; }

        [Option('v', "version", Required = false, HelpText = "Version for the new distro, the default is the wsl default, set 1 to WSL1, 2 to WSL2.")]
        public int Version { get; set; }
        
    }

    class DockerOptions
    {
        [Option("login", Required = true, HelpText = "Link wlsd with your docker account!")]
        public bool login { get; set; }

    }
}
