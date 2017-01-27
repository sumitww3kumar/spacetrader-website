using SpaceTrader.Model;
using SpaceTrader.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceTrader.Controllers
{
    [RoutePrefix("deadlines")]
    public class DeadlinesController : Controller
    {
        private readonly ComicsFolder folder;

        public DeadlinesController()
        {
            folder = new ComicsFolder("deadlines");
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
    }
}