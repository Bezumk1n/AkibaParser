﻿using Akiba.Extensions;
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
            StartParsers();
            Console.ReadLine();
        }

        private async static void StartParsers()
        {
            var akibaParser = new AkibaParserService();
            await akibaParser.Parse();
        }
    }
}