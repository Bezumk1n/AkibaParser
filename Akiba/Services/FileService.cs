using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiba.Services
{
    internal class FileService
    {
        public static void SaveToFile(string str, string fileName = "")
        {
            try
            {
                var fileToWrite = $"{Environment.CurrentDirectory}/{fileName}.xls";
                File.WriteAllText(fileToWrite, str, Encoding.UTF8);
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка записи в файл");
            }
        }
    }
}
