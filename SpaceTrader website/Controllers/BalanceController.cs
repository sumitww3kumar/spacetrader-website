using SpaceTrader.Model;
using SpaceTrader.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceTrader.Controllers
{
    [RoutePrefix("balance")]
    public class BalanceController : Controller
    {
        private readonly ComicsFolder folder;

        public BalanceController()
        {
            folder = new ComicsFolder("balance");
        }

        [ActionName("pages")]
        public ActionResult Pages(int id = -1)
        {
            var page = folder.PageByIndex(id);
            if (page == null)
            {
                throw new HttpException(404, "Not found");
            }

            return View("pages", new PageViewModel(folder, page));
        }

        [ActionName("index")]
        public ActionResult Index()
        {
            return Pages(-1);
        }

        [ActionName("characters")]
        public ActionResult Characters()
        {
            return View(folder.Characters.Select(c => new CharacterListViewModel(c)));
        }
    }
}