using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsld_cs.Files
{
    class Fops
    {
        public static void WriteToFileAppend(byte[] binary, string path)
        {
            Stream writingStream = new FileStream(path, FileMode.Append);
            writingStream.Write(binary, 0, binary.Length);
            writingStream.Close();
        }
    }
}
