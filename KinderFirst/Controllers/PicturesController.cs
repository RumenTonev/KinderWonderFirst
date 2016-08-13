using KinderFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class PicturesController : Controller
    {
        public ApplicationDbContext db = ApplicationDbContext.Create();
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
        [HttpGet]
        public ActionResult SetTutorialTutoPic(string id)
        {
          
            var result = db.TutoPictures.Where(x => x.TutoId == id);
            return View(result);
        }

   
    }
}