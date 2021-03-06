﻿using KinderFirst.Models;
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
    public class GalleryController : Controller
    {
        private const int AvatarScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const string TempFolder = "/Temp";
        private const string BigPicFolder = "/Content/Images/BigPic";
        private const string MapTempFolder = "~" + TempFolder;
        private const string MapBigPicFolder = "~" + BigPicFolder;

        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

        public ActionResult Index()
        {
            return View();
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
            var bigPicName= Path.Combine(Server.MapPath(MapBigPicFolder), Path.GetFileName(input.PicName));
            string extension = Path.GetExtension(fn);
            // ...get image and resize it, ...
            var img = new WebImage(fn);
            var bigImg = new WebImage(bigPicName);
           
            //img.Resize(width, height);
            // ... crop the part the user selected, ...
            img.Crop(top, left, img.Height - top - height, img.Width - left - width);
            //img.Crop(top, left,bottom,right);

            // ... delete the temporary file,...
            System.IO.File.Delete(fn);
            System.IO.File.Delete(bigPicName);
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
            var bigPath = Path.Combine(Server.MapPath("~/Content/Images/BigPic"), fileNameNew);
            item.PicLink = String.Format("~/Content/Images/{0}", fileNameNew);
            item.Id = result.Id;
            await DocumentDBRepository<GalleryItem>.UpdateItemAsync(result.Id, item);
            img.FileName = fileNameNew;
            bigImg.FileName = fileNameNew;
            bigImg.Save(bigPath);
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

        public async Task<ActionResult> Like(string itemId)
        {

            string ipAddress = this.Request.UserHostAddress;

                var model = await DocumentDBRepository<IpItem>.GetItemsAsync(x=>x.IP==ipAddress&&x.ItemId==itemId&&x.IsIp);

                if (model.Count()>0)
                {
                    this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { errorMessage = "You can not vote more than once for given entry" }, JsonRequestBehavior.AllowGet);
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
            var serverBigPath= HttpContext.Server.MapPath(BigPicFolder);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }
            if (Directory.Exists(serverBigPath) == false)
            {
                Directory.CreateDirectory(serverBigPath);
            }
            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath,serverBigPath, fileName);

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

        private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath,string serverBigPath, string fileName)
        {
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes(file.ContentLength);
            //var kur=file.InputStream.ConvertToBytes()
            var img = new WebImage(binData);
            WebImage bigImg = new WebImage(binData);
            var ratio = img.Height / (double)img.Width;
            img.Resize(AvatarScreenWidth, (int)(AvatarScreenWidth * ratio));
            var fullBigName= Path.Combine(serverBigPath, fileName);
            var fullFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            if (System.IO.File.Exists(fullBigName))
            {
                System.IO.File.Delete(fullBigName);
            }

            img.Save(fullFileName);
            bigImg.Save(fullBigName);
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