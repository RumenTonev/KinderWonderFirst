using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Winners()
        {
            ViewBag.Message = "Winners";

            return View();
        }

        public ActionResult Rules()
        {
            ViewBag.Message = "Rules";

            return View();
        }

        public ActionResult Rewards()
        {
            ViewBag.Message = "Rewards";

            return View();
        }

    }
}