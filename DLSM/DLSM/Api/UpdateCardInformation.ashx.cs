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
    /// Summary description for UpdateCardInformation
    /// </summary>
    public class UpdateCardInformation : IHttpHandler
    {

        private DLSMEntities db = new DLSMEntities();
        private string cardEIN = "";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                UpdateCardInformationRequest parm = new UpdateCardInformationRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<UpdateCardInformationRequest>(data);
                    cardEIN = parm.CardEIN;
                }

                UpdateCardInformationResponse ap = new UpdateCardInformationResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiUpdateCardInfo(Convert.ToInt32(parm.staffId),
                                                                Convert.ToInt32(parm.WH_ID),
                                                                parm.CardEIN,
                                                                parm.cardStatus,
                                                                parm.qrCode,
                                                                parm.startPrintDttmStr,
                                                                parm.finishPrintDttmStr,
                                                                parm.cardCount,
                                                                parm.saveMDMStatus,
                                                                parm.saveMDMDesc,
                                                                parm.prnErrDesc).ToList();
                            if (result[0].ID > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultCode = "1";
                                ap.cardEIN = parm.CardEIN;
                                ap.message = "OK";
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                ap.resultCode = "0";
                                ap.cardEIN = parm.CardEIN;
                                ap.message = "Update Card Error";
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            ap.resultCode = "0";
                            ap.cardEIN = parm.CardEIN;
                            ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: UpdateCardInformation catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                UpdateCardInformationResponse ap = new UpdateCardInformationResponse();
                ap.resultCode = "0";
                ap.cardEIN = cardEIN;
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: UpdateCardInformation catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

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