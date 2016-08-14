using KinderFirst.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class AdminUploadController : Controller
    {
        // GET: AdminUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpAdminPhoto(UploadAdminPhotoViews photos)
        {


            byte[] productPicture = new byte[photos.File.ContentLength];
            photos.File.InputStream.Read(productPicture, 0, photos.File.ContentLength);

            WebImage img = new WebImage(photos.File.InputStream);

            var fileName = photos.File.FileName;
            string extension = Path.GetExtension(fileName);
            
            var fileNameNew = String.Format("{0}{1}{2}", photos.TutoId, photos.Number,extension);

            var path = Path.Combine(Server.MapPath("~/Content/Images"), fileNameNew);
            img.FileName = fileNameNew;
            img.Save(path);

            using (ApplicationDbContext db = ApplicationDbContext.Create())
            {

                var input = new TutoPicture()
                {
                    Path = String.Format("~/Content/Images/{0}", fileNameNew),
                    Number = photos.Number,
                    TutoId = photos.TutoId
                };
                db.TutoPictures.Add(input);
                db.SaveChanges();
                return View("Index");
            }
        }
    }
}