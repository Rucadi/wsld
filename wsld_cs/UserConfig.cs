using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wsld_cs.Processes;

namespace wsld_cs
{
    public static class UserConfig
    {
        //public static string windows_temp_path = "C:/temp/";
        //public static string windows_image_path = "C:/temp/images";
        //public static string install_dir = "C:/wsld/";

        public static string windows_temp_path = Path.GetTempPath();
        public static string windows_image_path = Path.Combine(windows_temp_path,"images");

        private static string installation_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"wsld");
        public static string install_dir
        {
            get {
                return installation_dir;
            }
            set { 
                if(value.Length > 0)
                    installation_dir = Path.GetFullPath(value);
            }
        }
        


        public static string linux_windows_temp_path{
            get {
                return Commands.wslpath(windows_temp_path);
            }
        }
        public static string linux_windows_image_path
        {
            get
            {
                return Commands.wslpath(windows_image_path);
            }
        } 


        public static string session_id = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        //Commands.wslpath(UserConfig.windows_temp_path)



    }
}
