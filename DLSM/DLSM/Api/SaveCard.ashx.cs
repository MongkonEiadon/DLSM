using DLSM.Class;
using DLSM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DLSM.Api
{
    /// <summary>
    /// Summary description for SaveCard
    /// </summary>
    public class SaveCard : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();
        private string cardEIN = "";

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                SaveCardRequest parm = new SaveCardRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<SaveCardRequest>(data);
                    cardEIN = parm.CardEIN;
                }

                SaveCardResponse ap = new SaveCardResponse();
                using (DLSMEntities db = new DLSMEntities())
                {

                    db.Database.Connection.Open();

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiSaveCard(parm.addrNo, parm.ampDesc, parm.ampDescEng, parm.birthDateStr, parm.birthFlag,
                                                                    parm.distDesc, parm.distDescEng, parm.docNo, parm.docType, parm.reqMasRef, parm.expDateStr,
                                                                    parm.fname, parm.fnameEng, parm.issDateStr, parm.issOffLocCode, parm.lane, parm.lname,
                                                                    parm.lnameEng, parm.locFullDesc, parm.message, parm.natDesc, parm.offLocDesc, parm.offLocEngDesc,
                                                                    parm.offRegDesc, parm.offRegEngDesc, parm.pcNo, parm.pltCode, parm.pltDesc, parm.pltEngDesc,
                                                                    parm.pltNo, parm.pltPrnDesc, parm.prevExpDateStr, parm.prevIssDateStr, parm.prevOffLocDesc,
                                                                    parm.prevOffRegDesc, parm.prevOffRegEngDesc, parm.prevPltDesc, parm.prevPltNo, parm.prvCode,
                                                                    parm.prvDesc, parm.prvDescEng, parm.rcpNo, parm.reqDateStr, parm.reqNo, parm.reqTrDesc,
                                                                    parm.sex, parm.soi, parm.street, parm.titleAbrev, parm.titleDesc, parm.titleEngAbrev, parm.villageNo,
                                                                    parm.zipCode, parm.alienFlag, parm.ccFlag, parm.DCICode, parm.conditionDesc, parm.organDonateFlag,
                                                                    parm.TRSFlag, parm.firstIssueDateStr, parm.pltDescShort, parm.pltNo1, parm.pltNo2, parm.prevPltDescShort,
                                                                    parm.prevPltNo1, parm.prevPltNo2, parm.pltNoEng, parm.WH_ID, parm.staffId, parm.workstationId,
                                                                    parm.productType, parm.CardEIN, parm.qrCode).ToList();

                            if (result[0].seqno > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultCode = "1";
                                ap.cardEIN = parm.CardEIN;
                                ap.message = "OK";
                            }
                            else
                            {
                                if(db.Database.Connection != null)
                                {
                                    try
                                    {
                                        dbContextTransaction.Rollback();
                                    }
                                    catch
                                    {
                                       
                                    }
                                }
                                
                                ap.cardEIN = parm.CardEIN;
                                ap.resultCode = "0";
                                ap.message = "not found";
                                
                            }
                            //dbContextTransaction.Rollback();
                        }
                        catch (Exception ex)
                        {
                            if (db.Database.Connection != null)
                            {
                                try
                                {
                                    dbContextTransaction.Rollback();
                                }
                                catch
                                {

                                }
                                
                            }
                                
                            ap.cardEIN = parm.CardEIN;
                            ap.resultCode = "0";
                            if(ex.InnerException != null)
                            {
                                ap.message = ex.InnerException.Message;
                                if (ex.InnerException.StackTrace != null)
                                {
                                    ap.message += " + InnerException.StackTrace: " + ex.InnerException.StackTrace;
                                }
                            }
                            if(ex.Message != null)
                            {
                                ap.message += "+" +ex.Message;
                                if(ex.StackTrace != null)
                                {
                                    ap.message += " + StackTrace: " + ex.StackTrace;
                                }
                            }
                            if(ex.InnerException == null && ex.Message == null)
                            {
                                ap.message = "Error: SaveCard catch 2";
                            }

                            ap.message += " (Step 2)";
                            //ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: SaveCard catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                SaveCardResponse ap = new SaveCardResponse();
                ap.cardEIN = cardEIN;
                ap.resultCode = "0";
                if (ex.InnerException != null)
                {
                    ap.message = ex.InnerException.Message;
                    if (ex.InnerException.StackTrace != null)
                    {
                        ap.message += " + InnerException.StackTrace: " + ex.InnerException.StackTrace;
                    }
                }
                if (ex.Message != null)
                {
                    ap.message += "+" + ex.Message;
                    if (ex.StackTrace != null)
                    {
                        ap.message += " + StackTrace: " + ex.StackTrace;
                    }
                }
                if (ex.InnerException == null && ex.Message == null)
                {
                    ap.message = "Error: SaveCard catch 1";
                }
                ap.message += " (Step 1)";

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