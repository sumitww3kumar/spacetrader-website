namespace SpaceTrader.Views

open System
open System.Web
open SpaceTrader.Comics

type FolderViewModel(folder: ComicsFolder) =
    member self.PageCount = List.length folder.AllPages
    member self.RootURI = folder.ImageRootUrl

type ChapterViewModel(chapter: Chapter) =
    member self.PageCount = List.length chapter.Pages
    member self.Ordinal = chapter.Ordinal
    member self.Title = chapter.Title
    member self.Number = self.Ordinal + 1

type PageViewModel(folder: ComicsFolder, page: Page) =
    member self.ImagePath = Uri(folder.ImageRootUrl, page.ImagePath)
    
    member self.PageNumber = page.PageNumber + 1
    member self.PageIndex = folder.PageIndex page

    member self.Folder = FolderViewModel(folder)
    member self.Chapter = ChapterViewModel(folder.PageChapter(page).Value)

    member self.First = self.PageIndex = 0
    member self.Last = self.PageIndex + 1 = self.Folder.PageCount

type CharacterViewModel(character: Character) =
    member self.Name = character.Name
    member self.BioLines = character.BioLines
    member self.ImageURI = character.ImageURI

module PageView =
    let ShowPage (folder: ComicsFolder) index = 
        match folder.PageByIndex index with
            | Some(page) -> PageViewModel(folder, page)
            | None -> raise(HttpException(404, "Not found"))