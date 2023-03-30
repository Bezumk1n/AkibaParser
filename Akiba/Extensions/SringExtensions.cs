using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiba.Extensions
{
    internal static class SringExtensions
    {
        public static string GetValue(this string source, string descriptionIdentifier, string endIdentifier)
        {
            var result = string.Empty;
            var descriptionStartIndex = source.IndexOf(descriptionIdentifier);
            if (descriptionStartIndex != -1)
            {
                var descriptionEndIndex = source.IndexOf(endIdentifier, descriptionStartIndex);
                if (descriptionEndIndex != -1)
                {
                    result = source.Substring(descriptionStartIndex + descriptionIdentifier.Length, descriptionEndIndex - (descriptionStartIndex + descriptionIdentifier.Length)).Trim();
                }
            }
            return result;
        }
        public static void SaveToFile(this string str, string prefix = "")
        {
            try
            {
                var fileToWrite = $"{Environment.CurrentDirectory}/{prefix}ParsedLinks_{DateTime.Now}.xls";
                File.WriteAllText(fileToWrite, str, Encoding.UTF8);
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка записи в файл");
            }
        }
    }
}
