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
    /// Summary description for UploadPicture
    /// </summary>
    public class UploadPicture : IHttpHandler
    {

        private DLSMEntities db = new DLSMEntities();
        private string cardEIN = "";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                UploadPictureRequest parm = new UploadPictureRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<UploadPictureRequest>(data);
                    cardEIN = parm.cardEIN;
                }

                UploadPictureResponse ap = new UploadPictureResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiUpdatePicture(parm.cardEIN, parm.WH_ID, parm.person_image).ToList();
                            if (result.Count() > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.cardEIN = parm.cardEIN;
                                ap.resultCode = "1";
                                ap.message = "OK";
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                ap.cardEIN = parm.cardEIN;
                                ap.resultCode = "0";
                                ap.message = "Upload Picture Error";
                            }
                        }
                        catch (Exception ex)
                        {                           
                                dbContextTransaction.Rollback();
                                ap.cardEIN = parm.cardEIN;
                                ap.resultCode = "0";
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: UploadPicture catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                UploadPictureResponse ap = new UploadPictureResponse();
                ap.cardEIN = cardEIN;
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: UploadPicture catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
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