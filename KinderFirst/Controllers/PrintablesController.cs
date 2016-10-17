using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KinderFirst.Controllers
{
    public class PrintablesController : Controller
    {
        // GET: Printables
        public ActionResult Index()
        {
            var paths = Directory.EnumerateFiles(Server.MapPath("~/Content/Printables"));

            string[] stringSeparators = new string[] { "\\Printables\\", ".jpg" };
            List<string> names = new List<string>();
            string[] result;
            foreach (var item in paths)
            {
                result = item.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                names.Add(result[1]);
            }

            return View(names);
        }

        public ActionResult GetPdf(string name)
        {
            string pathSource = Server.MapPath(String.Format("~/Content/Pdf/{0}.pdf", name));
            FileStream fsSource = new FileStream(pathSource, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fsSource, "application/pdf");
        }
    }
}