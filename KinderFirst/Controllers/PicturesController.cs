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
        //[HttpGet]
        //public async Task<ActionResult> Gallery(int take)
        //{
        //    var items = await DocumentDBRepository<GalleryItem>.GetItemsAsync(take);
        //    return View(items);
        //}

        [HttpGet]
        public async Task<ActionResult> Gallery(int? page,int? size)
        {
            int pageSize = (size ?? 3);
            int pageNumber = (page ?? 1);
            var items = await DocumentDBRepository<GalleryItem>.GetItemsAsync();
           // PagedList<GalleryItem> model = new PagedList<GalleryItem>(items,page,pageSize);
            
           
            return View(items.ToPagedList(pageNumber, pageSize));
          //  return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateGalleryItem([Bind(Include = "Id,Owner,Mail,,PicLink,Likes")] GalleryItem item)
        {

                if (ModelState.IsValid)
                {
                    await DocumentDBRepository<GalleryItem>.CreateItemAsync(item);
                    return RedirectToAction("Index");
                }

            return View();
        }

        [HttpPost]

        public async Task<ActionResult> ProcessForm(GalleryItemView input)
        {
            try
            {
                using (var img1 = Image.FromStream(input.File.InputStream))
                {
                    var picture= img1.RawFormat.Equals(ImageFormat.Jpeg);
                }
            }
            catch {
               return new JsonResult
           {
                Data = new { ErrorMessage = "Model is not valid-that is not picture file", Success = false },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };; 
            
            }


            if (this.ModelState.IsValid)
            {
                byte[] productPicture = new byte[input.File.ContentLength];
                input.File.InputStream.Read(productPicture, 0, input.File.ContentLength);

                WebImage img = new WebImage(input.File.InputStream);

                var fileName = input.File.FileName;
                string extension = Path.GetExtension(fileName);
                GalleryItem item = new GalleryItem()
                {
                    Mail = input.Mail,
                    Owner = String.Format("{0} {1}", input.FirstName, input.LastName),
                };

             var result=   await DocumentDBRepository<GalleryItem>.CreateItemAsync(item);
                
                    var fileNameNew = String.Format("{0}{1}", result.Id, extension);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileNameNew);
                    item.PicLink = String.Format("~/Content/Images/{0}", fileNameNew);
                item.Id = result.Id;
                    await DocumentDBRepository<GalleryItem>.UpdateItemAsync(result.Id, item);
                img.FileName = fileNameNew;
                    img.Save(path);
                }
            

            return RedirectToAction("Gallery"); 
        }
        public ActionResult GetUploadForm()
        {
            return PartialView("_UploadForm");
        }
        public async Task<ActionResult> Like(string itemId)
        {

            string ipAddress = this.Request.UserHostAddress;

                var model = await DocumentDBRepository<IpItem>.GetItemsAsync(x=>x.IP==ipAddress&&x.IteamId==itemId);

                if (model.Count()>0)
                {
                    this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { responseText = "You can not vote more than once for given entry" }, JsonRequestBehavior.AllowGet);
                }
                await DocumentDBRepository<IpItem>.CreateItemAsync(new IpItem() { IP = ipAddress, IteamId = itemId });
                var galleryItem = await DocumentDBRepository<GalleryItem>.GetItemAsync(itemId); 
                galleryItem.Likes++;
                await DocumentDBRepository<GalleryItem>.UpdateItemAsync(itemId, galleryItem);
                return this.Content(galleryItem.Likes.ToString()); 
        }
   
    }
}