using System.Web.Mvc;

namespace SpaceTrader.Controllers
{
    public class HomeController : Controller
    {
        [ActionName("index")]
        public ActionResult Index()
        {
            return View();
        }
    }
}