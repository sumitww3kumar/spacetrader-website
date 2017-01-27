using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpaceTrader.Model
{
    public class Chapter
    {
        public int Ordinal { get; }
        public string Title { get; }

        private List<Page> pages;
        public IEnumerable<Page> Pages => pages;

        public Chapter(string manifestPath)
        {
            using (var reader = File.OpenText(manifestPath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var manifest = (JObject)JToken.ReadFrom(jsonReader);

                Title = (string)manifest["title"];
                Ordinal = (int)manifest["ordinal"];
            }

            var chapterRoot = Path.GetDirectoryName(manifestPath) + Path.DirectorySeparatorChar;
            var folderRoot = Path.Combine(chapterRoot, "..");

            pages = Directory.EnumerateFiles(chapterRoot, "*.png")
                .Union(Directory.EnumerateFiles(chapterRoot, "*.jpg"))
                .Select(file =>
                {
                    //get path relative to root
                    var relativePath = new Uri(folderRoot).MakeRelativeUri(new Uri(file));

                    Page newPage;
                    return Page.TryCreatePage(relativePath.ToString(), out newPage) ? newPage : null;
                })
                .Where(p => p != null)
                .ToList();
        }
    }
}