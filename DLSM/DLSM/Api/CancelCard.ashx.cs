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
    /// Summary description for CancelCard
    /// </summary>
    public class CancelCard : IHttpHandler
    {
        private string cardEIN = "";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                CancelCardRequest parm = new CancelCardRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<CancelCardRequest>(data);
                    cardEIN = parm.cardEIN;
                }

                CancelCardResponse ap = new CancelCardResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiCancelCard(parm.WH_ID, parm.staffId, parm.cardEIN).ToList();
                            if (result[0].ID == 1)
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
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CancelCard catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                CancelCardResponse ap = new CancelCardResponse();
                ap.cardEIN = cardEIN;
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CancelCard catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


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