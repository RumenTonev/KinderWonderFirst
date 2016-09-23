using System.Web;
using System.Web.Optimization;

namespace KinderFirst
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.3.min.js",
                         "~/Scripts/jquery.unobtrusive-ajax.min.js",
                         "~/Scripts/jquery.cookie-1.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                      "~/Scripts/jquery.form.min.js",
                      "~/Scripts/jquery.Jcrop.min.js",
                      "~/Scripts/site.avatar.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                      "~/Scripts/toastr.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/toast").Include(
                      "~/Content/toastr.min.css" ));

            bundles.Add(new StyleBundle("~/Content/jcrop").Include(
                      "~/Content/jquery.Jcrop.min.css",
                      "~/Content/site.avatar.css"));

        }
    }
}
