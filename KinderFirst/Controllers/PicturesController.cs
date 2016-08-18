using KinderFirst.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Helpers;
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
                var result = db.GalleryItems.Take(3).ToList();
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

        [HttpPost]

        public ActionResult ProcessForm(GalleryItemView input)
        {
            
            byte[] productPicture = new byte[input.File.ContentLength];
            input.File.InputStream.Read(productPicture, 0, input.File.ContentLength);

            WebImage img = new WebImage(input.File.InputStream);

            var fileName = input.File.FileName;
            string extension = Path.GetExtension(fileName);
            GalleryItem item=new GalleryItem()
            {
                 Mail=input.Mail,
                  Owner=String.Format("{0} {1}",input.FirstName,input.LastName),                  
            };

            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {
                db.GalleryItems.Add(item);
                db.SaveChanges();
                var fileNameNew = String.Format("{0}{1}", item.Id, extension);
                var path = Path.Combine(Server.MapPath("~/Content/Images"), fileNameNew);
                item.PicLink = String.Format("~/Content/Images/{0}", fileNameNew);
                db.SaveChanges();
                img.FileName = fileNameNew;
                img.Save(path);
            }

            return RedirectToAction("Gallery"); ;
        }
        public ActionResult GetUploadForm()
        {
            return PartialView("_UploadForm");
        }
        public ActionResult Like(string itemId)
        {

            string ipAddress = this.Request.UserHostAddress;

            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {
                var model = db.IpItems.FirstOrDefault(x => x.IP == ipAddress && x.IteamId == itemId);

                if (model != null)
                {
                    this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { responseText = "You can not vote more than once for given entry" }, JsonRequestBehavior.AllowGet);
                }
                db.IpItems.Add(new IpItem() { IP = ipAddress, IteamId = itemId });
                var item = db.GalleryItems.FirstOrDefault(x => x.Id.ToString() == itemId);
                item.Likes++;
                db.SaveChanges();
                return this.Content(item.Likes.ToString());
            }
           
        }
   
    }
}