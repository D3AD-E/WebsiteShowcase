using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class ApplicationModel
    {
        public string Name { get; set; }
        public string Desciption { get; set; }

        public string LanguageTag { get; set; }

        public List<string> Images { get; set; }

    }
}
