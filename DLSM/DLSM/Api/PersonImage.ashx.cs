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
    /// Summary description for PersonImage
    /// </summary>
    public class PersonImage : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                PersonImageRequest parm = new PersonImageRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<PersonImageRequest>(data);
                }

                PersonImageResponse ap = new PersonImageResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiPersonImage((parm.citizenId)).ToList();
                            if (result.Count() > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultcode = "1";
                                ap.person_image = result[0].PartData;
                                ap.citizenId = parm.citizenId;
                                ap.message = "OK";
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                ap.resultcode = "0";
                                ap.message = "not found";
                            }
                        }
                        catch (Exception ex)
                        {
                                dbContextTransaction.Rollback();
                                ap.resultcode = "0";
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: PersonImage catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                PersonImageResponse ap = new PersonImageResponse();
                ap.resultcode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: PersonImage catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

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