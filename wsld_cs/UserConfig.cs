using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld_cs
{
    public static class UserConfig
    {
        public static string Temp_path = "C:/temp/";
        public static string rootfs_path = "C:/temp/";




        public static string Path_win_to_wsl(string win_path)
        {
            string new_path = "/mnt/" + win_path.Substring(0, 1).ToLower() + "/";
            win_path = win_path.Substring(3, win_path.Length-3);
            foreach(var let in win_path)
            {
                if (let == '\\') new_path += "/";
                else new_path += let;
            }

            return new_path;
        }




    }
}
