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
    /// Summary description for CheckCardSerial
    /// </summary>
    public class CheckCardSerial : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();
        private string cardEIN = "";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                CheckCardSerialRequest parm = new CheckCardSerialRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<CheckCardSerialRequest>(data);
                    cardEIN = parm.cardEIN;
                }

                ApiCheckResponse ap = new ApiCheckResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiCheckCardSerial(Convert.ToInt32(parm.WH_ID), parm.cardEIN, Convert.ToInt32(parm.staffId)).ToList();
                            if (result.Count() > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultCode = "1";
                                ap.cardEIN = parm.cardEIN;
                                ap.message = "OK";
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                ap.cardEIN = parm.cardEIN;
                                ap.resultCode = "0";
                                ap.message = "not found";
                            }
                        }
                        catch (Exception ex)
                        {
                                dbContextTransaction.Rollback();
                                ap.cardEIN = parm.cardEIN;
                                ap.resultCode = "0";
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CheckCardSerial catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                ApiCheckResponse ap = new ApiCheckResponse();
                ap.cardEIN = cardEIN;
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CheckCardSerial catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


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