namespace SpaceTrader.Comics

open System.Configuration
open System.IO
open System
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.Text.RegularExpressions

type Page = { 
    ImagePath: string; 
    PageNumber: int32 
}

type Chapter = { 
    Title: string;
    Ordinal: int32;
    Pages: list<Page>
}

type Character = {
    Name: string;
    BioLines: list<string>;
    ImageURI: Uri
}

type ComicsFolder(rootUrl, chapters, pages, characters) =
    member self.ImageRootUrl: Uri = rootUrl

    member self.Chapters: list<Chapter> = chapters
    member self.AllPages: list<Page> = pages
    member self.Characters: list<Character> = characters 

    member self.PageByIndex index =
        if index < 0 then
            pages |> Seq.tryLast
        else
            pages |> Seq.skip(index)
                  |> Seq.tryHead

    member self.PageIndex (page: Page) =
        match pages |> List.tryFindIndex (fun p -> obj.Equals(p, page)) with
            | Some(index) -> index
            | None -> -1

    member self.PageChapter page =
        chapters |> Seq.where(fun ch -> ch.Pages |> List.contains(page))
                 |> Seq.tryHead

module LoadFolder =
    let private appSetting (name: string) = ConfigurationManager.AppSettings.Item(name)

    let private ReadPage imagePath =
        let nameMatch = Regex.Match(imagePath, @".+?([0-9]+)\.(png|jpg)")
        if nameMatch.Success then
            let mutable pageNum = 0
            
            if Int32.TryParse(nameMatch.Groups.Item(1).Value, &pageNum) then
                Some({ ImagePath = imagePath; 
                       PageNumber = pageNum })
            else
                None
        else
            None

    let private RelativePath filePath relativeTo = 
        let rootUri = Uri(relativeTo)
        rootUri.MakeRelativeUri(Uri(filePath))

    let private ReadChapter manifestPath =
        let chapterRoot = Path.GetDirectoryName(manifestPath) + string(Path.DirectorySeparatorChar)
        let folderRoot = Path.Combine(chapterRoot, "..")

        let relativeToFolder filePath = RelativePath filePath folderRoot

        let filesInChapter ext = Directory.EnumerateFiles(chapterRoot, ext)

        try 
            use reader = File.OpenText(manifestPath)
            use jsonReader = new JsonTextReader(reader)
            
            let manifest = JToken.ReadFrom(jsonReader) :?> JObject
            
            let ordinal = int32(manifest.Item("ordinal"))
            let title = string(manifest.Item("title"))
            
            let pages = Seq.concat([filesInChapter("*.png"); 
                                        filesInChapter("*.jpg")])
                            |> Seq.map(relativeToFolder)
                            |> Seq.map(fun pageUri -> ReadPage(string(pageUri)))
                            |> Seq.choose id
                            |> Seq.toList
            
            Some({ Title = title;
                   Ordinal = ordinal;
                   Pages = pages })
        with
            | :? IOException -> None

    let private ReadCharacter (rootUri: Uri) (prop: JProperty) =
        let charInfo = prop.Value :?> JObject

        let bioLines = match charInfo.Item("bio") with
                        | null -> Seq.empty
                        | vals -> vals.Values()
                        |> Seq.map(string)
                        |> Seq.toList

        { Name = prop.Name;
          BioLines = bioLines;
          ImageURI = Uri(rootUri, prop.Name + ".png"); }

    let private ReadCharacters rootUri charactersFile =
        try
            use reader = File.OpenText(charactersFile)
            use jsonReader = new JsonTextReader(reader)
            
            let characters = JToken.ReadFrom(jsonReader) :?> JObject
            let readCharacterFromRoot = ReadCharacter rootUri

            characters.Properties() |> Seq.map(fun charProp -> readCharacterFromRoot(charProp))
                                    |> Seq.toList                    
        with
            | :? IOException -> List.empty

    type private ChapterPage = { Chapter: Chapter; Page: Page }

    let OpenFolder folderName = 
        let root = appSetting("SpaceTrader.ComicsPath")
        let staticRoot = appSetting("SpaceTrader.ComicsRootUrl")
        let staticRootUrl = Uri(Uri(staticRoot), folderName + string(Path.DirectorySeparatorChar))

        let folderPath = Path.Combine(root, folderName)
                        
        let chapters = Directory.GetDirectories(folderPath) 
                        |> Seq.map(fun dir -> Path.Combine(dir, "chapter.json"))
                        |> Seq.map(ReadChapter)
                        |> Seq.choose id
                        |> Seq.sortBy(fun c -> c.Ordinal)
                        |> Seq.toList

        let pages = chapters 
                    |> Seq.collect(fun c -> c.Pages 
                                         |> Seq.map(fun p -> { Chapter = c; 
                                                               Page = p }))
                    |> Seq.sortBy(fun cp -> cp.Page.PageNumber)
                    |> Seq.sortBy(fun cp -> cp.Chapter.Ordinal)
                    |> Seq.map(fun cp -> cp.Page)
                    |> Seq.toList

        let charsFile = Path.Combine(folderPath, "characters.json")
        let characters = ReadCharacters staticRootUrl charsFile

        ComicsFolder(staticRootUrl, chapters, pages, characters)