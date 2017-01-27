using SpaceTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceTrader.Views
{
    public class FolderViewModel
    {
        public int PageCount { get; }
        public Uri RootURI { get; }

        public FolderViewModel(ComicsFolder folder)
        {
            PageCount = folder.AllPages.Count();
            RootURI = folder.ImageRootUrl;
        }
    }

    public class ChapterViewModel
    {
        public int PageCount { get; }
        public int Ordinal { get; }
        public string Title { get; }
        public int Number => Ordinal + 1;

        public ChapterViewModel(Chapter chapter)
        {
            Title = chapter.Title;
            PageCount = chapter.Pages.Count();
            Ordinal = chapter.Ordinal;
        }
    }

    public class PageViewModel
    {
        public Uri ImagePath { get; }

        public int PageNumber { get; }
        public int PageIndex { get; }

        public FolderViewModel Folder { get; }
        public ChapterViewModel Chapter { get; }

        public bool First => PageIndex == 0;
        public bool Last => PageIndex == Folder.PageCount - 1;

        public PageViewModel(ComicsFolder folder, Page page)
        {
            Folder = new FolderViewModel(folder);

            var chapter = folder.PageChapter(page);
            Chapter = new ChapterViewModel(chapter);
            
            ImagePath = new Uri(folder.ImageRootUrl, page.ImagePath);
            PageNumber = page.PageNumber + 1;

            PageIndex = folder.PageIndex(page);
        }
    }
}