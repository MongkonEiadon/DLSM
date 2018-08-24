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
    /// Summary description for PrintCardDailyRept
    /// </summary>
    public class PrintCardDailyRept : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                PrintCardDailyReptRequest parm = new PrintCardDailyReptRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<PrintCardDailyReptRequest>(data);
                }

                PrintCardDailyReptResponse ap = new PrintCardDailyReptResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        if (parm.rept_type == "1")
                        {
                            ReportSumByStaff(parm, dbContextTransaction, ap);
                        }
                        else if (parm.rept_type == "2")
                        {
                            ReportSumByOffice(parm, dbContextTransaction, ap);
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                PrintCardDailyReptResponse ap = new PrintCardDailyReptResponse();
                ap.resultCode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: PrintCardDailyRept catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            
        }

        private void ReportSumByStaff(PrintCardDailyReptRequest parm, System.Data.Entity.DbContextTransaction dbContextTransaction, PrintCardDailyReptResponse ap)
        {
            try
            {
                var result = db.sp_ApiPrintDailyRept(parm.WH_ID,parm.staffId,parm.rept_type, parm.rept_date).ToList();
                if (result.Count() > 0)
                {
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    List<SumCardData> listCard = new List<SumCardData>();
                    List<StaffSumData> listStaff = new List<StaffSumData>();

                    int lastStaffID = 0;
                    bool isFirst = true;
                    foreach (var a in result)
                    {
                        if (lastStaffID != a.staffid)
                        {
                            // old staff
                            if (!isFirst)
                            {
                                listStaff[listStaff.Count - 1].ListSumCardData = new SumCardData[listCard.Count];
                                for (int n = 0; n < listCard.Count; n++)
                                {
                                    listStaff[listStaff.Count - 1].ListSumCardData[n] = listCard[n];
                                }
                            }
                            isFirst = false;

                            // new staff

                            listCard.Clear();
                            StaffSumData s = new StaffSumData();
                            s.staffId = "" + a.staffid;
                            s.regisFirstName = a.regisFirstName;
                            s.regisLastName = a.regisLastName;
                            s.officeCode = a.officeCode;
                            s.officeName = a.officeName;
                            s.userName = a.userName;
                            s.rept_datetime = parm.rept_date;
                            s.total_succ = "" + a.total_succ;
                            s.total_canc = "" + a.total_canc;

                            listStaff.Add(s);
                        }
                        lastStaffID = a.staffid;

                        SumCardData sc = new SumCardData();
                        sc.card_type_desc = a.card_type_desc;
                        sc.total_canc_card = "" + a.total_canc_card;
                        sc.total_succ_card = "" + a.total_succ_card;
                        sc.total_fail_card = "" + a.total_fail_card;
                        listCard.Add(sc);

                    } // end foreach

                    // remain card
                    listStaff[listStaff.Count - 1].ListSumCardData = new SumCardData[listCard.Count];
                    for (int n = 0; n < listCard.Count; n++)
                    {
                        listStaff[listStaff.Count - 1].ListSumCardData[n] = listCard[n];
                    }
                    // copy result to array
                    ap.List_Staff_sum_data = new StaffSumData[listStaff.Count];
                    for (int i = 0; i < listStaff.Count; i++)
                    {
                        ap.List_Staff_sum_data[i] = listStaff[i];
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
                    ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: PrintCardDailyRept(ReportSumByStaff) catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
            }
        }

        private void ReportSumByOffice(PrintCardDailyReptRequest parm, System.Data.Entity.DbContextTransaction dbContextTransaction, PrintCardDailyReptResponse ap)
        {
            try
            {
                var result = db.sp_ApiPrintDailyRept(parm.WH_ID, parm.staffId, parm.rept_type, parm.rept_date).ToList();
                if (result.Count() > 0)
                {
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    ap.List_Staff_sum_data = new StaffSumData[result.Count];
                    int i=0;

                    foreach (var a in result)
                    {
                        
                        StaffSumData s = new StaffSumData();
                        s.staffId = "" + a.staffid;
                        s.regisFirstName = a.regisFirstName;
                        s.regisLastName = a.regisLastName;
                        s.officeCode = a.officeCode;
                        s.officeName = a.officeName;
                        s.userName = a.userName;
                        s.rept_datetime = parm.rept_date;
                        s.total_succ = "" + a.total_succ;
                        s.total_canc = "" + a.total_canc;

                        ap.List_Staff_sum_data[i++] = s;
                        

                    } // end foreach
                    
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
                    ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: PrintCardDailyRept(ReportSumByOffice) catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;
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