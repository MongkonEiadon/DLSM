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
    /// Summary description for GetListPrintTrans
    /// </summary>
    public class GetListPrintTrans : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                GetListPrintTransRequest parm = new GetListPrintTransRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<GetListPrintTransRequest>(data);
                }

                GetListPrintTransResponse ap = new GetListPrintTransResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiGetListTrans(parm.WH_ID,
                                                             parm.staffId,
                                                             parm.startDateTime,
                                                             parm.endDateTime,
                                                             parm.productType).ToList();
                            if (result.Count() > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();

                                ap.list_print_trans = new PrintCardTrans[result.Count()];
                                int i = 0;
                                foreach (var a in result)
                                {
                                    PrintCardTrans pi = new PrintCardTrans();
                                    pi.reqNO = a.reqNo;
                                    pi.driverLicenseType = a.pltPrnDesc;
                                    pi.driverLicenseNo = a.pltNo;
                                    pi.titleAbrev = a.titleAbrev;
                                    pi.fname = a.fname;
                                    pi.lname = a.lname;
                                    pi.cardEIN = a.SerialNo;
                                    pi.staffName = a.staffName;
                                    pi.cardStatus = a.cardStatus;
                                    pi.printDateTime = a.startPrintDttmStr;
                                    pi.productType = a.productType;
                                    pi.trans_id = a.TRSFlag;
                                    pi.req_date = a.reqDateStr;
                                    pi.saveMDMStatus = a.saveMDMStatus;
                                    pi.saveMDMDesc = a.saveMDMDesc;
                                    pi.prnErrDesc = a.prnErrDesc;

                                    ap.list_print_trans[i++] = pi;
                                }

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
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: GetListPrintTrans catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                GetListPrintTransResponse ap = new GetListPrintTransResponse();
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: GetListPrintTrans catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

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