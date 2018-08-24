using DLSM.Class;
using DLSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DLSM.Api
{
    /// <summary>
    /// Summary description for UpdatePrintStatus
    /// </summary>
    public class UpdatePrintStatus : IHttpHandler
    {

        private DLSMEntities db = new DLSMEntities();

        public void ProcessRequest(HttpContext context)
        {
            UpdatePrintStatusRequest parm = new UpdatePrintStatusRequest();
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                String data = sr.ReadToEnd();
                parm = new JavaScriptSerializer().Deserialize<UpdatePrintStatusRequest>(data);
            }

            ApiResult ap = new ApiResult();
            using (DLSMEntities db = new DLSMEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var result = db.sp_ApiUpdatePrintStatus(parm.SerialNo,
                                         parm.Status,parm.PrinterStatus);
                        if (result.ToString() == "1")
                        {
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            ap.Code = "1";
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            ap.Code = "0";
                            ap.Message = "Update Print Error";
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
            string json = new JavaScriptSerializer().Serialize(ap);

            context.Response.ContentType = "text/javascript";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}