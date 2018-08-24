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
    /// Summary description for SaveMagInfo
    /// </summary>
    public class SaveMagInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                SaveMagInfoRequest parm = new SaveMagInfoRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<SaveMagInfoRequest>(data);
                }

                SaveMagInfoResponse ap = new SaveMagInfoResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiSaveMagInfo(parm.WH_ID,
                                                                parm.staffId,
                                                                parm.trans_datetime,
                                                                parm.track_01,
                                                                parm.track_02,
                                                                parm.track_03,
                                                                parm.resultCode,
                                                                parm.resultMsg).ToList();
                            if (result[0].SeqNo > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultCode = "1";
                                ap.message = "OK";
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                ap.resultCode = "0";
                                ap.message = "not found";
                            }
                        }
                        catch (Exception ex)
                        {
                                dbContextTransaction.Rollback();
                                ap.resultCode = "0";
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: SaveMagInfo catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                SaveMagInfoResponse ap = new SaveMagInfoResponse();
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: SaveMagInfo catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

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