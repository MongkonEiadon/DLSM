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
    /// Summary description for GetCardInfo
    /// </summary>
    public class GetCardInfo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                GetCardInfoRequest parm = new GetCardInfoRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<GetCardInfoRequest>(data);
                }

                GetCardInfoResponse ap = new GetCardInfoResponse();
                using (DLSMEntities db = new DLSMEntities())
                {

                    db.Database.Connection.Open();

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiGetCardInfo(parm.WH_ID, parm.staffId, parm.cardEIN).ToList();

                            if (result.Count() > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.addrNo = result[0].addrNo;

                                ap.ampDesc = result[0].ampDesc;
                                ap.ampDescEng = result[0].ampDescEng;
                                ap.birthDateStr = result[0].birthDateStr;
                                ap.birthFlag = result[0].birthFlag;
                                ap.distDesc = result[0].distDesc;
                                ap.distDescEng = result[0].distDescEng;
                                ap.docNo = result[0].docNo;
                                ap.docType = result[0].docType;
                                ap.reqMasRef = result[0].reqMasRef;
                                ap.expDateStr = result[0].expDateStr;
                                ap.fname = result[0].fname;
                                ap.fnameEng = result[0].fnameEng;
                                ap.issDateStr = result[0].issDateStr;
                                ap.issOffLocCode = result[0].issOffLocCode;
                                ap.lane = result[0].lane;
                                ap.lname = result[0].lname;
                                ap.lnameEng = result[0].lnameEng;
                                ap.locFullDesc = result[0].locFullDesc;
                                ap.natDesc = result[0].natDesc;
                                ap.offLocDesc = result[0].offLocDesc;
                                ap.offLocEngDesc = result[0].offLocEngDesc;
                                ap.offRegDesc = result[0].offRegDesc;
                                ap.offRegEngDesc = result[0].offRegEngDesc;
                                ap.pcNo = result[0].pcNo;
                                ap.pltCode = result[0].pltCode;
                                ap.pltDesc = result[0].pltDesc;
                                ap.pltEngDesc = result[0].pltEngDesc;
                                ap.pltNo = result[0].pltNo;
                                ap.pltPrnDesc = result[0].pltPrnDesc;
                                ap.prevExpDateStr = result[0].prevExpDateStr;
                                ap.prevIssDateStr = result[0].prevIssDateStr;
                                ap.prevOffLocDesc = result[0].prevOffLocDesc;
                                ap.prevOffRegDesc = result[0].prevOffRegDesc;
                                ap.prevOffRegEngDesc = result[0].prevOffRegEngDesc;
                                ap.prevPltDesc = result[0].prevPltDesc;
                                ap.prevPltNo = result[0].prevPltNo;
                                ap.prvCode = result[0].prvCode;
                                ap.prvDesc = result[0].prvDesc;
                                ap.prvDescEng = result[0].prvDescEng;
                                ap.rcpNo = result[0].rcpNo;
                                ap.reqDateStr = result[0].reqDateStr;
                                ap.reqNo = result[0].reqNo;
                                ap.reqTrDesc = result[0].reqTrDesc;
                                ap.sex = result[0].sex;
                                ap.soi = result[0].soi;
                                ap.street = result[0].street;
                                ap.titleAbrev = result[0].titleAbrev;
                                ap.titleDesc = result[0].titleDesc;
                                ap.titleEngAbrev = result[0].titleEngAbrev;
                                ap.villageNo = result[0].villageNo;
                                ap.zipCode = result[0].zipCode;
                                ap.alienFlag = result[0].alienFlag;
                                ap.ccFlag = result[0].ccFlag;
                                ap.DCICode = result[0].DCICode;
                                ap.conditionDesc = result[0].conditionDesc;
                                ap.organDonateFlag = result[0].organDonateFlag;
                                ap.TRSFlag = result[0].TRSFlag;
                                ap.firstIssueDateStr = result[0].firstIssueDateStr;
                                ap.pltDescShort = result[0].pltDescShort;
                                ap.pltNo1 = result[0].pltNo1;
                                ap.pltNo2 = result[0].pltNo2;
                                ap.prevPltDescShort = result[0].prevPltDescShort;
                                ap.prevPltNo1 = result[0].prevPltNo1;
                                ap.prevPltNo2 = result[0].prevPltNo2;
                                ap.pltNoEng = result[0].pltNoEng;
                                ap.workstationId = result[0].workstationId;
                                ap.productType = result[0].productType;
                                ap.CardEIN = result[0].CardEIN;
                                ap.qrCode = result[0].qrCode;
                                ap.cardStatus = result[0].cardStatus; // 1: ดี , 2: เสีย, 3: ไม่พิมพ์
                                ap.startPrintDttmStr = result[0].startPrintDttmStr;
                                ap.finishPrintDttmStr = result[0].finishPrintDttmStr;
                                ap.person_image = result[0].PartData;

                                ap.message = "OK";
                                ap.resultCode = "1";
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
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: GetCardInfo catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                GetCardInfoResponse ap = new GetCardInfoResponse();
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: GetCardInfo catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


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