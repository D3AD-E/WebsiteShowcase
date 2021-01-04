using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DescriptionShort { get; set; }
        public string Description { get; set; }
        public string LanguageTag { get; set; }
        public string ImageLink { get; set; }
        public string GithubLink { get; set; }
    }
}
