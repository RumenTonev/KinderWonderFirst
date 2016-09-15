using KinderFirst.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class PicturesController : Controller
    {
        private const int AvatarScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const string TempFolder = "/Temp";
        private const string MapTempFolder = "~" + TempFolder;

        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

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
        public async Task<ActionResult> Gallery(int? page,int? size)
        {
            int pageSize = (size ?? 15);
            int pageNumber = (page ?? 1);
            var items = await DocumentDBRepository<GalleryItem>.GetItemsAsync(x=>x.IsIp==false);
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteGalleryItem( string id)
        {

            if (ModelState.IsValid)
            {
                var mainItem = await DocumentDBRepository<GalleryItem>.GetItemAsync(id);
               var result= await DocumentDBRepository<GalleryItem>.DeleteItemAsync(id);
               
                
                var path =Request.MapPath(mainItem.PicLink);
                if ((System.IO.File.Exists(path)))
                {
                    System.IO.File.Delete(path);
                }
                return RedirectToAction("Gallery");
            }

            return View();
        }

        [HttpPost]

        public async Task<ActionResult> ProcessForm(GalleryItemView input)
        {
            var top = Convert.ToInt32(input.PicTop.Replace("-", "").Replace("px", ""));
            var left = Convert.ToInt32(input.PicLeft.Replace("-", "").Replace("px", ""));
            var height = Convert.ToInt32(input.PicHeight.Replace("-", "").Replace("px", ""));
            var width = Convert.ToInt32(input.PicWidth.Replace("-", "").Replace("px", ""));
            var bottom= Convert.ToInt32(input.PicBottom.Replace("-", "").Replace("px", ""));
            var right = Convert.ToInt32(input.PicRight.Replace("-", "").Replace("px", ""));
            // Get file from temporary folder
            var fn = Path.Combine(Server.MapPath(MapTempFolder), Path.GetFileName(input.PicName));
            string extension = Path.GetExtension(fn);
            // ...get image and resize it, ...
            var img = new WebImage(fn);
            //img.Resize(width, height);
            // ... crop the part the user selected, ...
            img.Crop(top, left, img.Height - top - height, img.Width - left - width);
            //img.Crop(top, left,bottom,right);

            // ... delete the temporary file,...
            System.IO.File.Delete(fn);
            // ... and save the new one.
            GalleryItem item = new GalleryItem()
            {
                Mail = input.Mail,
                Owner = String.Format("{0} {1}", input.FirstName, input.LastName),
                IsIp=false
            };

            var result = await DocumentDBRepository<GalleryItem>.CreateItemAsync(item);
            var fileNameNew = String.Format("{0}{1}", result.Id, extension);
            var path = Path.Combine(Server.MapPath("~/Content/Images"), fileNameNew);
            item.PicLink = String.Format("~/Content/Images/{0}", fileNameNew);
            item.Id = result.Id;
            await DocumentDBRepository<GalleryItem>.UpdateItemAsync(result.Id, item);
            img.FileName = fileNameNew;
            img.Save(path);

            return RedirectToAction("Gallery"); 
        }
        [HttpPost]
        public ActionResult UploadPicture(IEnumerable<HttpPostedFileBase> files)
        {
            if (files == null || !files.Any()) return Json(new { success = false, errorMessage = "No file uploaded." });
            var file = files.FirstOrDefault();  // get ONE only
            if (file == null || !IsImage(file)) return Json(new { success = false, errorMessage = "File is of wrong format." });
            if (file.ContentLength <= 0) return Json(new { success = false, errorMessage = "File cannot be zero length." });
            var webPath = GetTempSavedFilePath(file);
            return Json(new { success = true, fileName = webPath }); // success
        }

        public ActionResult GetUploadForm()
        {
            return PartialView("_UploadForm");
        }

        public ActionResult GetPdf(string name)
        {
                string pathSource = Server.MapPath(String.Format("~/Content/Pdf/{0}.pdf",name));
                FileStream fsSource = new FileStream(pathSource, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fsSource, "application/pdf");
        }

       public ActionResult Printables()
        {
           var paths= Directory.EnumerateFiles(Server.MapPath("~/Content/Printables"));
           
            string[] stringSeparators = new string[] { "\\Printables\\",".jpg"};
            List<string> names = new List<string>();
            string[] result;
            foreach (var item in paths)
            {
                result = item.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                names.Add(result[1]);
            }

            return View(names);
        }
        public async Task<ActionResult> Like(string itemId)
        {

            string ipAddress = this.Request.UserHostAddress;

                var model = await DocumentDBRepository<IpItem>.GetItemsAsync(x=>x.IP==ipAddress&&x.ItemId==itemId&&x.IsIp);

                if (model.Count()>0)
                {
                    this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { responseText = "You can not vote more than once for given entry" }, JsonRequestBehavior.AllowGet);
                }
                await DocumentDBRepository<IpItem>.CreateItemAsync(new IpItem() { IP = ipAddress, ItemId = itemId,IsIp=true });
                var galleryItem = await DocumentDBRepository<GalleryItem>.GetItemAsync(itemId); 
                galleryItem.Likes++;
                await DocumentDBRepository<GalleryItem>.UpdateItemAsync(itemId, galleryItem);
                return this.Content(galleryItem.Likes.ToString()); 
        }
        private string GetTempSavedFilePath(HttpPostedFileBase file)
        {
            // Define destination
            var serverPath = HttpContext.Server.MapPath(TempFolder);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);
            return Path.Combine(TempFolder, fileName);
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                var currentUtcNow = DateTime.UtcNow;
                var serverPath = HttpContext.Server.MapPath("/Temp");
                if (!Directory.Exists(serverPath)) return;
                var fileEntries = Directory.GetFiles(serverPath);
                foreach (var fileEntry in fileEntries)
                {
                    var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                    var res = currentUtcNow - fileCreationTime;
                    if (res.TotalHours > hoursOld)
                    {
                        System.IO.File.Delete(fileEntry);
                    }
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }

        private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            var ratio = img.Height / (double)img.Width;
            img.Resize(AvatarScreenWidth, (int)(AvatarScreenWidth * ratio));

            var fullFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }

            img.Save(fullFileName);
            return Path.GetFileName(img.FileName);
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file == null) return false;
            return file.ContentType.Contains("image") ||
                _imageFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}