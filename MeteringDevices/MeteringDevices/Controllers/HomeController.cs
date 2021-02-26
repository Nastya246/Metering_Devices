using System.Web.Mvc;
using MeteringDevices.Models;

namespace MeteringDevices.Controllers
{
    public class HomeController : Controller
    {
        private InstrumentationEntities db = new InstrumentationEntities();
        
        public ActionResult Index()
        {

            return View();
        }

       
    }
}