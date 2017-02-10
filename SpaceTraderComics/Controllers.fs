namespace SpaceTrader.Controllers

open System
open System.Web
open System.Web.Mvc
open System.Web.Routing
open SpaceTrader.Comics
open SpaceTrader.Views

type HomeController() =
    inherit Controller()
    member self.Index() = self.View()

type ComicController(folderName) = 
    inherit Controller()
    member self.Folder = LoadFolder.OpenFolder(folderName)
    member private self.pageViewer = PageView.ShowPage(self.Folder)
    
    [<ActionName("pages")>]
    member self.Pages (id: Nullable<int>) =
        let index = if id.HasValue then id.Value else -1
        self.View("pages", self.pageViewer index)

    [<ActionName("index")>]
    member self.Index() = self.View("pages", self.pageViewer -1)

type BalanceController() =
    inherit ComicController("balance")

    [<ActionName("characters")>]
    member self.Characters() = self.View(self.Folder.Characters
                                         |> Seq.map CharacterViewModel)
    
type OldBalanceController() = inherit ComicController("php25")
type DeadlinesController() = inherit ComicController("deadlines")

type ControllerFactory() =
    inherit DefaultControllerFactory() with
        override self.CreateController(ctx:RequestContext, name:string) =
            match name with
                | "balance" -> new BalanceController() :> IController
                | "php25" -> new OldBalanceController() :> IController
                | "deadlines" -> new DeadlinesController() :> IController
                | "Home" -> new HomeController() :> IController
                | _ -> base.CreateController(ctx, name)