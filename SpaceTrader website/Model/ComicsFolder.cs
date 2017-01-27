using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;

namespace SpaceTrader.Model
{
    public class ComicsFolder
    {
        private List<Chapter> chapters;
        private List<Page> pages;

        public IEnumerable<Chapter> Chapters => chapters;
        public IEnumerable<Page> AllPages => pages;

        private List<Character> characters;
        public IEnumerable<Character> Characters => characters;

        public Uri ImageRootUrl { get; }
        
        public int PageIndex(Page p) => pages.IndexOf(p);
        public Chapter PageChapter(Page p) => chapters.Where(c => c.Pages.Contains(p))
            .FirstOrDefault();

        public Page PageByIndex(int index) => index < 0? 
                pages.LastOrDefault() :
                pages.Skip(index).FirstOrDefault();
        
        public ComicsFolder(string folderName)
        {
            var root = ConfigurationManager.AppSettings["SpaceTrader.ComicsPath"];
            var folderPath = Path.Combine(root, folderName);

            chapters = Directory.GetDirectories(folderPath)
                .Select(dir =>
                {
                    try
                    {
                        var manifest = Path.Combine(dir, "chapter.json");

                        return new Chapter(manifest);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                })
                .Where(chapter => chapter != null)
                .OrderBy(c => c.Ordinal)
                .ToList();

            pages = chapters.SelectMany(c =>
                    c.Pages.Select(p => new { c, p }))
                .OrderBy(val => val.c.Ordinal)
                .ThenBy(val => val.p.PageNumber)
                .Select(val => val.p)
                .ToList();

            var rootUrl = new Uri(ConfigurationManager.AppSettings["SpaceTrader.ComicsRootUrl"]);
            ImageRootUrl = new Uri(rootUrl, folderName + Path.DirectorySeparatorChar);

            try
            {
                var charactersFile = Path.Combine(folderPath, "characters.json");

                var charactersRoot = new Uri(rootUrl, folderName + Path.DirectorySeparatorChar);
                characters = Character.ReadCharacters(charactersRoot, charactersFile)
                    .ToList();
            }
            catch (IOException)
            {
                characters = new List<Character>(0);
            }
        }
    }
}