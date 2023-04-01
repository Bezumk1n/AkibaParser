using Akiba.Services;
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
            FileService.SaveToFile(str, prefix);
        }
    }
}
