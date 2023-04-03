using Akiba.Extensions;
using Akiba.Models;
using Akiba.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Akiba
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Кого парсим?");
            Console.WriteLine("1 - Akiba");
            Console.WriteLine("2 - Comicstreet");

            var choice = Console.ReadLine();
            if (choice.ToLower() == "akiba" || choice == "1")
            {
                var akibaParser = new AkibaParserService();
                akibaParser.Parse();
            }
            if (choice.ToLower() == "comicstreet" || choice == "2")
            {
                var comicstreetParser = new ComicstreetParserService();
                comicstreetParser.Parse();
            }
            AwaitCommand();
        }

        private static void AwaitCommand()
        {
            Console.WriteLine("Для завершения процесса введите 'Stop'");
            var command = Console.ReadLine();
            if (command.ToLower() != "stop")
            {
                AwaitCommand();
            }
        }
    }
}
