using SpaceTrader.Comics;
using SpaceTrader.Views;
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
            folder = LoadFolder.OpenFolder("deadlines");
        }

        [ActionName("pages")]
        public ActionResult Pages(int id = -1)
        {
            var page = folder.PageByIndex(id).Get(or: () =>
            {
                throw new HttpException(404, "Not found");
            });

            return View("pages", new PageViewModel(folder, page));
        }

        [ActionName("index")]
        public ActionResult Index()
        {
            return Pages(-1);
        }
    }
}