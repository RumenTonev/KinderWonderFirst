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
       // public ApplicationDbContext db = ApplicationDbContext.Create();
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
            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {
                var result = db.TutoPictures.Where(x => x.TutoId == id).ToList();
                return View(result);
            }
        }
        [HttpGet]
        public ActionResult Gallery()
        {
            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {
                var result = db.GalleryItems.ToList();
                return View(result);
            }
        }

        [HttpPost]

        public ActionResult CreateGalleryItem(GalleryItem item)
        {
            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {
                var result = db.GalleryItems.Add(item);
                db.SaveChanges();
                return View();
            }
        }
   
    }
}