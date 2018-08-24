using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DLSM.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error(int? code)
        {
            //0 = access ristrict
            if(code != null && code.Value == 0)
            {
                ViewBag.ErrorMessage = "Access Denied";
            }
            else
            {
                ViewBag.ErrorMessage = "Error";
            }

            return View();
        }
    }
}