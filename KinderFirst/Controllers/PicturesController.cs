using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class PicturesController : Controller
    {
        // GET: Pictures
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SetSectionTutoPic(string id)
        {
            var model = new SectionDetails(id);
            return View(model);
        }

   
    }
}