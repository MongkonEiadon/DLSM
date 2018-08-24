using DLSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DLSM.Class
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;

            var is_accept = "1"; // using entity with session["UserID"]
             

            if (session != null && session["UserID"] == null && is_accept == "0")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "PleaseLogin" }
                                });
            }
            else if(session != null && session["UserID"] != null)
            {
                using (DLSMEntities context = new DLSMEntities())
                {
                    // get controller by router
                    string controller_name = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                    //string action_name = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();

                    // set module user access
                    string user_request_controller = controller_name.ToLower();

                    int intUgID = int.Parse(session["UserUgID"].ToString());
                    string Ismanager = session["IsManager"].ToString();

                    var module_code = context.Modules
                                            .Where(m => m.Name.ToLower() == user_request_controller)
                                            .Select(m => m.Code).SingleOrDefault();

                    List<Permission> access = context.Permissions
                                        .Where(o => o.UgID == intUgID)
                                        .Where(o => o.MdCode == module_code)
                                        .ToList();

                    if (access.Count > 0)
                    {

                    }
                    else
                    {
                        if (access.Count <= 0 && controller_name != "Home")
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Error" }, { "Action", "Error" } });
                        }
                    }
                    
                    
                }
            }
        }
    }

       
    
}