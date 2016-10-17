using KinderFirst.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult PlayNow()
        {
            ViewBag.Message = "Play Now";

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

        public ActionResult SiteTermsOfUse()
        {
            ViewBag.Message = "Terms of Use";

            return View();
        }

        public ActionResult SiteRequirements()
        {
            ViewBag.Message = "Site Requirements";

            return View();
        }

        public ActionResult Contacts()
        {
            ViewBag.Message = "Contacts";

            return View();
        }
        public ActionResult CheckAge(Age item)
        {
            DateTime bday = new DateTime(Int32.Parse(item.Year), Int32.Parse(item.Month), Int32.Parse(item.Day));
            int age = (int)((DateTime.Now - bday).TotalDays / 365.242199);
            if (age < 12)
            {
                return Json(new { success = false, errorMessage = "You are under 12 years old and thus not eligible." });
            }
            return Json(new { success = true, errorMessage = "You are eligible." });
        }
    }
}