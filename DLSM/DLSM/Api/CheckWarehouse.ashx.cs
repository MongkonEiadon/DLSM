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
    /// Summary description for CheckWarehouse
    /// </summary>
    public class CheckWarehouse : IHttpHandler
    {
        private DLSMEntities db = new DLSMEntities();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                CheckWarehouseRequest parm = new CheckWarehouseRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<CheckWarehouseRequest>(data);
                }

                CheckWarehouseResponse ap = new CheckWarehouseResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var result = db.sp_ApiCheckWarehouse(Convert.ToInt32(parm.WH_ID)).ToList();
                            if (result.Count() > 0)
                            {
                                ap.resultcode = "1";
                                ap.WH_ID = parm.WH_ID;
                                ap.WhName = "" + result[0].WhName;
                                ap.update_datetime = DateTime.Now.ToString();

                                ap.product_remain = new WarehouseProducts[result.Count()];
                                int e = 0;
                                foreach (var i in result)
                                {
                                    WarehouseProducts a = new WarehouseProducts();
                                    a.product_name = i.PdName;
                                    a.curr_qty = "" + i.Qty;
                                    a.min_qty = "" + i.MinStock;

                                    ap.product_remain[e++] = a;
                                }
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
                                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CheckWarehouse catch 2" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;

                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                CheckWarehouseResponse ap = new CheckWarehouseResponse();
                ap.resultcode = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: CheckWarehouse catch 1" : ex.Message) : ex.InnerException.Message + " StackTrace:" + ex.StackTrace;


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