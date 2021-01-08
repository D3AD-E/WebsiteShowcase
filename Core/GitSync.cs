using HtmlAgilityPack;
using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models;

namespace Website.Core
{
    public class GitSync
    {
        private const string GitLink = "https://github.com/D3AD-E?tab=repositories";

        private const string GitRawLink = "https://raw.githubusercontent.com/D3AD-E/{0}/master/README.md";

        private const string GitRepoLink = "https://github.com/D3AD-E/{0}";

        private HtmlWeb Web;

        public GitSync()
        {
            Web = new HtmlWeb();
        }

        private async Task<HtmlDocument> DownloadInternal(string crawlUrl)
        {

            return await Web.LoadFromWebAsync(crawlUrl);

            throw new InvalidOperationException("Can not load html from given source.");
        }

        public async Task<IEnumerable<ProjectModel>> GetProjects()
        {
            try
            {
                var htmlDocument = await DownloadInternal(GitLink);

                var projectsNodesList = htmlDocument.DocumentNode
                                   .Descendants("ul")
                                   .Where(cl => !string.IsNullOrEmpty(cl.GetAttributeValue("data-filterable-for", null))).FirstOrDefault().ChildNodes
                                   .Where(li =>string.Compare(li.Name,"li")==0);
                                   

                var projects = new List<ProjectModel>();

                foreach (var node in projectsNodesList)
                {
                    var modelHtmlNode = node.Descendants("div").FirstOrDefault();
                    var model = new ProjectModel();
                    model.Name = modelHtmlNode.Descendants("a").FirstOrDefault().InnerText.Trim();

                    model.GithubLink = string.Format(GitRepoLink, model.Name);
                    model.DescriptionShort = modelHtmlNode.Descendants("p").FirstOrDefault().InnerText.Trim();
                    model.LanguageTag = modelHtmlNode.Descendants("span").Where(sp => !string.IsNullOrEmpty(sp.GetAttributeValue("itemprop", "programmingLanguage"))).FirstOrDefault().InnerText.Trim();
                    var readme = await Web.LoadFromWebAsync(string.Format(GitRawLink, model.Name));

                    model.Description = Markdown.ToHtml(readme.DocumentNode.OuterHtml);

                    projects.Add(model);
                }

                return projects;
            }
            catch (Exception exception)
            {
                return Enumerable.Empty<ProjectModel>();
            }
        }
    }
}
