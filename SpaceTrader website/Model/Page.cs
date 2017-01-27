using System.Text.RegularExpressions;

namespace SpaceTrader.Model
{
    public class Page
    {
        public string ImagePath { get; }
        public int PageNumber { get; }

        private Page(string imagePath, int pageNumber)
        {
            ImagePath = imagePath;
            PageNumber = pageNumber;
        }

        public static bool TryCreatePage(string imagePath, out Page page)
        {
            page = null;

            var nameMatch = Regex.Match(imagePath, @".+?([0-9]+)\.(png|jpg)");
            if (!nameMatch.Success)
            {
                return false;
            }

            int pageNumber;
            if (!int.TryParse(nameMatch.Groups[1].Value, out pageNumber))
            {
                return false;
            }

            page = new Page(imagePath, pageNumber);
            return true;
        }
    }
}