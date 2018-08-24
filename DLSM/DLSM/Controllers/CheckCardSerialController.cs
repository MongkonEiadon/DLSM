using DLSM.Models;
using DLSM.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DLSM.Controllers
{
    public class CheckCardSerialController : Controller
    {
        private DLSMEntities db = new DLSMEntities();
        //// GET: CheckCardSerial
        //public ActionResult Index()
        //{
        //    CheckCardSerialRequest cd = new CheckCardSerialRequest();

        //    return View();
        //}

        [HttpPost]
        public ActionResult Check(CheckCardSerialRequest model)
        {
            ApiResult ap = new ApiResult();
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var result = context.sp_ApiCheckCardSerial(model.WhID, model.SerialNo).ToList();
                        if (result.Count() > 0)
                        {
                            context.SaveChanges();
                            dbContextTransaction.Commit();
                            ap.Code = "1";
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            ap.Code = "2";
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        ap.Code = "0";
                        ap.Message = ex.Message;
                        
                    }


                }
            }
            return Json(ap);
            
        }
    }
}