using SpaceTrader.Comics;
using SpaceTrader.Views;
using System.Web;
using System.Web.Mvc;

namespace SpaceTrader.Controllers
{
    [RoutePrefix("php25")]
    public class OldBalanceController : Controller
    {
        private readonly ComicsFolder folder;

        public OldBalanceController()
        {
            folder = LoadFolder.OpenFolder("php25");
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
    }
}