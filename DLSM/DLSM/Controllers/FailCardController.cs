using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLSM.Models;
using DLSM.Class;
using System.Data.SqlClient;
using System.Globalization;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class FailCardController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: FailCard
        public ActionResult Index()
        {
            return View();
        }

        // GET: Receives/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult SearchCard(string searchCardNo
                                    , string searchToCardNo
                                    , DateTime? searchBeginDate
                                    , DateTime? searchEndDate)
        {
            var context = new DLSMEntities();
            int Userid = Convert.ToInt32(Session["UserID"]);
            List<StockSerial> list = new List<StockSerial>();
            //change store to get only stock serial
            var listdata = context.sp_SearchCard(Convert.ToString(searchCardNo), Convert.ToString(searchToCardNo), searchBeginDate, searchEndDate).ToList().OrderBy(a=>a.id);
            int sarch_card_limit = 500;
            if (listdata.Count() > 0)
            {
                //using loop from stock serial begin to end
                foreach (var i in listdata)
                {
                    string sbegin = i.SerialBegin;
                    UInt64 card_serialno = Convert.ToUInt64(sbegin);
                    for (int n=0; n<i.SerialCount; n++)
                    {
                        StockSerial s = new StockSerial();
                        s.PdID = n+1;
                        s.SerialBegin = card_serialno.ToString();
                        s.WhID = i.whid;
                        
                        list.Add(s);
                        card_serialno++;
                        //check item not over limit
                        if (n > sarch_card_limit)
                            break;
                    }
                   
                }
            }

            return View("Index",list);
        }

        [HttpPost]
        public ActionResult InsertFailCard(StockSerial model)
        {
            var context = new DLSMEntities();
            var UserID = Session["UserID"].ToString();

            model.CardList.RemoveAt(0);
            foreach (var a in model.CardList)
            {
                StockSerial SS = new StockSerial();

                SS.ID = a.ID;
                SS.WhID = a.WhID;
                SS.SerialBegin = a.CardSerial;
                SS.Name = a.CardDate;
                SS.IpProperty = a.Remark;

                SaveCardResponse ap = new SaveCardResponse();
                using (DLSMEntities db = new DLSMEntities())
                {

                    db.Database.Connection.Open();

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = context.sp_SaveCardManual("",
                                                                "", "", "", "",
                                                                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                                                                "", "", "", "", "", "", "", "", "", "", "", "", "",
                                                                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
                                                                "", "", "", "", "", "",
                                                                "", "", "", "", "", "", "", "", Convert.ToString(SS.WhID), UserID, "", "", SS.SerialBegin, "", Convert.ToDateTime(SS.Name), SS.IpProperty).ToList();

                            if (result[0].seqno > 0)
                            {
                                db.SaveChanges();
                                dbContextTransaction.Commit();
                                ap.resultCode = "1";
                                ap.cardEIN = SS.SerialBegin;
                                ap.message = "OK";
                            }
                            else
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

                                ap.cardEIN = SS.SerialBegin;
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
                                    return Json("ERROR : " + ex.ToString());
                                }
                                catch
                                {

                                }

                            }

                            ap.cardEIN = SS.SerialBegin;
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
                                ap.message = "Error: SaveCard catch 2";
                            }

                            ap.message += " (Step 2)";
                            //ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: SaveCard catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
                        }
                    }
                }
                //string json = new JavaScriptSerializer().Serialize(ap);
                //context.Response.ContentType = "text/javascript";
                //context.Response.Write(json);

                
            }
            return Json("success");
        }

    }
}