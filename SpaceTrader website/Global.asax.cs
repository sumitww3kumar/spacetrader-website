using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpaceTrader
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ControllerBuilder.Current.SetControllerFactory(new Controllers.ControllerFactory());
            
            AreaRegistration.RegisterAllAreas();
            
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action="index", id = UrlParameter.Optional });
        }
    }
}
