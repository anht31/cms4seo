using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using cms4seo.Common.Helpers;
using cms4seo.Model.LekimaxType;
using cms4seo.Service.Provider;

namespace cms4seo.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error

        public ActionResult NotFound()
        {

            bool isEnableRedirect = false;
            var url = Request.RawUrl.TrimEnd('/');

            if (Setting.WebSettings[WebSettingType.RedirectMode] == "1")
            {
                isEnableRedirect = true;

                var redirect = FlatFileAccess.Read301CSV();

                if (redirect != null && redirect.Keys.Contains(url))
                {
                    // log
                    LogHelper.Write("Redirect", $" {url} [->] {redirect[url]}");

                    // Clear the error
                    Response.Clear();
                    Server.ClearError();

                    // 301 it to the new url
                    Response.RedirectPermanent(redirect[url]);
                    return View();
                }

            }


            // log
            LogHelper.Write("NotFound",
                isEnableRedirect
                    ? $" {url} [->] Not Redirect"
                    : $" {url} [->] Not Redirect, Disable Redirect in setting");


            // handles the default action, in case we can't map an old page to a new page\
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Todo: Pass the exception into the view model, which you can make.
            //       That's an exercise, dear reader, for -you-.
            //       In case u want to pass it to the view, if you're admin, etc.
            // if (User.IsAdmin) // <-- I just made that up :) U get the idea...
            {
                var exception = Server.GetLastError();
                // etc..
            }

            return View();
        }


        public ActionResult BadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View();
        }

        // Shhh .. secret test method .. ooOOooOooOOOooohhhhhhhh
        public ActionResult ThrowError()
        {
            throw new NotImplementedException("Pew ^ Pew");
        }
    }
}