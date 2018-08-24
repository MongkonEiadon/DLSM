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
    /// Summary description for ProvImage
    /// </summary>
    public class ProvImage : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();
      

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                ProvImageRequest parm = new ProvImageRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<ProvImageRequest>(data);
                }

                ProvImageResponse ap = new ProvImageResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiProvImage(Convert.ToDateTime(parm.req_date)).ToList();
                            if (result.Count() > 0)
                            {
                                ap.list_prov_image = new ProvImages[result.Count()];
                                int i = 0;
                                foreach (var a in result)
                                {
                                    ProvImages pi = new ProvImages();
                                    pi.provCode = a.Code.Trim().Substring(0, 3);
                                    pi.image = a.BackgroundPicture;
                                    pi.update_datetime = a.ImageUpdate.ToString();

                                    ap.list_prov_image[i++] = pi;
                                }
                                ap.resultcode = "1";
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
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: ProvImage catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                ProvImageResponse ap = new ProvImageResponse();
                ap.resultcode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: ProvImage catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


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