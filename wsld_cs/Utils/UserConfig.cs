using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsld.Utils;
using wsld_cs.Processes;

namespace wsld_cs
{
    public static class UserConfig
    {

        public static string default_distro;
        public static string session_id = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();


        public static string _dockerfile = "Dockerfile";
        public static string dockerfile_path
        {
            get {
                return _dockerfile;
            }
            set {
                if (value != null && value.Length > 0)
                    _dockerfile = value;
            }
        }

        public static string PWD = Directory.GetCurrentDirectory();
        public static string wsl_PWD = Commands.wslpath(PWD);



        public static string repository;
        public static string image;
        public static string tag;
        public static string repo_image_tag {
            get
            {
                return repository + "/" + image + ":"+tag;
            }
            set
            {
                var rdt = Utils.Get_Repo_Dist_Tag(value);
                repository = rdt[0];
                image      = rdt[1];
                tag        = rdt[2];
            }
        }
        public static string repo_image
        {
            get { return repository + "/" + image; }
        }


        public static string wsld_distro_name;
        public static int wsld_version;

        /*WSLD DEFAULT ONLY PARAMETERS*/

            /*WINDOWS PATHS*/
            public static string windows_temp_path  = Path.GetTempPath();
            public static string windows_image_path = Path.Combine(windows_temp_path,"images");
            private static string installation_dir  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"wsld");
            public static string image_install_dir
            {
                get {
                    return Path.Combine(installation_dir,wsld_distro_name);
                }
                set { 
                    if(value != null && value.Length > 0)
                        installation_dir = Path.GetFullPath(value);
                }
            }
       
 
            /*WINDOWS PATHS LINUX FORMAT*/
            public static string wsl_windows_temp_path{
                get {
                    return Commands.wslpath(windows_temp_path);
                }
            }
            public static string wsl_windows_image_path
            {
                get
                {
                    return Commands.wslpath(windows_image_path);
                }
            }
            public static string w_rootfs_path
            {
                get
                {
                    return Path.Combine(windows_image_path, rootfs_name);
                }
            }
            public static string w_tmp_rootfs_path
            {
                get
                {
                    return windows_temp_path + tmp_rootfs_name;
                }
            }


            /*NAMING*/
            public static string tmp_rootfs_name
            {
                get
                {
                    return image + "_" + session_id + "_temp_rootfs.tar.gz";
                }
            }

            public static string rootfs_name
            {
                get {
                    return wsld_distro_name + "_" + session_id + "_rootfs.tar.gz";
                }
            }


            /*LINUX PATHS*/
            public static string linux_temporary_folder
            {
                get
                {
                    return "/tmp/wsld/" + wsld_distro_name + session_id;
                }
            }

        /*USER AND PASSWORD*/

            public static string username;
            public static string userpassword;
        public static bool createUser;
            public static void setImageUserPasswordConfig(string user, string passw, bool create)
        {

            createUser = create;
            username = user;
            userpassword = passw;
        }
            public static void generateConfigs(string rit, string iid, string d_name, int version)
            {

                repo_image_tag = rit;
                image_install_dir = iid;
                wsld_distro_name = d_name;
                wsld_version = version;
            }

        /*END OF DEFAULT ONLY PARAMETERS*/

        /*DOCKER RELATED*/

        public static bool isLoggedToDocker = false;

    }
}
