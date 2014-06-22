using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace imc
{
    public class Log
    {
        public static void SaveLog(string log)
        {
            try
            {
                FileStream fs = new FileStream(Program.FILE_NAME, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(log);
                sw.Close();
                fs.Close();
            }
            catch
            {
                Console.WriteLine("写入日志文件失败");
            }
        }
    }
}
