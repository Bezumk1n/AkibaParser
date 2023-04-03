using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiba.Models
{
    internal class ComicstreetItemInformation
    {
        public string Url { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public IEnumerable<string> Category { get; set; }
    }
}
