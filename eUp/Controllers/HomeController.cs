using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eUp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "The simple way to collect all types of data from clients and potential clients";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is a 3rd Year Computing project submission for the Institute of Technology, Tallaght." +
                " It was also entered into Microsoft's Imagine Cup 2013";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "You can get in contact in the following ways:";

            return View();
        }
    }
}
