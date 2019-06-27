using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld.Utils
{
    class Utils
    {

        public static string ReadLineMasked(char mask = '*')
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    sb.Append(keyInfo.KeyChar);
                    Console.Write(mask);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);

                    if (Console.CursorLeft == 0)
                    {
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);
                    }
                    else Console.Write("\b \b");
                }
            }
            Console.WriteLine();
            return sb.ToString();
        }

        public static string[] Get_Repo_Dist_Tag(string all)
        {

            string[] repository_distrotag = all.Split('/');
            int rlen = repository_distrotag.Length;
            string repository = rlen > 1 ? repository_distrotag[0] : "library";
            string distrotag = rlen > 1 ? repository_distrotag[1] : repository_distrotag[0];


            string[] distro_tag = distrotag.Split(':');
            int dlen = distro_tag.Length;

            string distro = distro_tag[0];
            string tag = dlen > 1 ? distro_tag[1] : "latest";


            string[] ret = { repository, distro, tag };
            return ret;

        }

    }
}
