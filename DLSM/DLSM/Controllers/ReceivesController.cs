using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLSM.Models;
using System.Data.SqlClient;
using DLSM.Class;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class ReceivesController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Receives
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var context = new DLSMEntities();
            int Userwhid = Convert.ToInt32(Session["UserWhID"]);
            List<Document> list = new List<Document>();
            var listdata = context.sp_DocumentList(Userwhid,"1").ToList();
            if(listdata.Count() > 0)
            {
                foreach(var i in listdata)
                {
                    Document d = new Document();
                    d.ID = i.ID;
                    d.DocNo = i.DocNo;
                    d.DocDate = i.DocDate.Value.AddYears(543);
                    d.CreateBy = i.CreateBy;
                    d.CreateName = i.CreateName;
                    d.WhID = i.WhID;
                    d.WarehouseName = i.WarehouseName;
                    d.PdID = i.SpID;
                    d.ProductName = i.ProductName;
                    d.Remark = i.Remark;
                    d.Status = i.Status;
                    d.StatusName = GetStatusName(i.Status);
                    d.ApproveDate = i.ApproveDate;
                    d.ApproveBy = i.ApproveBy;
                    d.ProcessDate = i.ProcessDate;
                    d.ProcessBy = i.ProcessBy;
                    d.Qty = i.Qty;
                    list.Add(d);
                }
            }            
            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            
            return View(list);
        }

        public ActionResult SearchReceive(DateTime? searchFromDate,DateTime? searchToDate,int? searchWare,int? searchPro)
        {
            var context = new DLSMEntities();
            int Userid = Convert.ToInt32(Session["UserID"]);
            List<Document> list = new List<Document>();
            var listdata = context.sp_SearchDocument("", searchFromDate, searchToDate, searchWare, searchPro, 1, null, Userid).ToList();
            if (listdata.Count() > 0)
            {
                foreach (var i in listdata)
                {
                    Document d = new Document();
                    d.ID = i.ID;
                    d.DocNo = i.DocNo;
                    d.DocDate = i.DocDate.Value.AddYears(543);
                    d.CreateBy = i.CreateBy;
                    d.CreateName = i.CreateName;
                    d.WhID = i.WhID;
                    d.WarehouseName = i.WarehouseName;
                    d.PdID = i.PdID;
                    d.ProductName = i.ProductName;
                    d.Remark = i.Remark;
                    d.Status = i.Status;
                    d.StatusName = GetStatusName(i.Status);
                    d.ApproveDate = i.ApproveDate;
                    d.ApproveBy = i.ApproveBy;
                    d.ProcessDate = i.ProcessDate;
                    d.ProcessBy = i.ProcessBy;
                    d.Qty = i.Qty;
                    list.Add(d);
                }
            }           
            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View("Index", list);
        }
        // GET: Receives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Receives/Create
        public ActionResult Create()
        {

            ViewBag.SpID = db.Suppliers.ToList();
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.PdID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Receives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DocNo,DocDate,CreateBy,DocType,WhID,SpID,Remark,Status,ApproveDate,ApproveBy,ProcessDate,ProcessBy,ToWhID")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Documents.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreateBy = new SelectList(db.Staffs, "ID", "Name", document.CreateBy);
            ViewBag.SpID = new SelectList(db.Suppliers, "ID", "Name", document.SpID);
            return View(document);
        }

        [HttpPost]
        public ActionResult InsertReceive(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }

            db.Documents.Add(model);
            db.SaveChanges();

            int docid = model.ID;

            model.DocDetailList.RemoveAt(0);
            foreach (var i in model.DocDetailList)
            {
                DocumentDetail dd = new DocumentDetail();
                dd.DocID = docid;
                dd.PdID = i.PdID;
                dd.Qty = i.Qty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "I";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            return Json("success");
        }
        

        [HttpPost]
        public ActionResult InsertSendAppReceive(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }

            db.Documents.Add(model);
            db.SaveChanges();

            int docid = model.ID;

            model.DocDetailList.RemoveAt(0);
            foreach (var i in model.DocDetailList)
            {
                DocumentDetail dd = new DocumentDetail();
                dd.DocID = docid;
                dd.PdID = i.PdID;
                dd.Qty = i.Qty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "I";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            return Json("success");
        }

        [HttpGet]
        public ActionResult checkIsAsset(int PrdID)
        {
            string res = "n";
            using (DLSMEntities context = new DLSMEntities())
            {
                var isasset = context.Products.Where(o => o.ID == PrdID).Select(s => s.IsAsset).SingleOrDefault();

                if (null != isasset)
                {
                    if (isasset.ToLower() == "y")
                    {
                        res = "y";
                    }
                    else
                    {
                        res = "n";
                    }
                }

            }

            return this.Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult checkIsAssetSNControl(int PrdID)
        {
            string isAsset = "n";
            string isControlSerial = "n";

            using (DLSMEntities context = new DLSMEntities())
            {
                Product objProduct = db.Products.Find(PrdID);   //context.Products.Where(o => o.ID == PrdID).SingleOrDefault();

                if (null != objProduct)
                {
                    // IsAssetss
                    isAsset = objProduct.IsAsset.ToLower() == "y" ? "y" : "n";

                    // IsSerial Control
                    isControlSerial = objProduct.SerialControl.ToLower() == "y" ? "y" : "n";
                    
                }

                return this.Json(new { isAsset = isAsset.ToString(), isControlSerial = isControlSerial.ToString() }, JsonRequestBehavior.AllowGet);
            }
            
        }

        // GET: Receives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var context = new DLSMEntities();

            Document document = db.Documents.Find(id);
            
            var wh = context.Warehouses.SingleOrDefault(u => u.ID == document.WhID);

            document.WhID = wh.ID;
            document.WhName = wh.Name;

            var s = context.Staffs.SingleOrDefault(u => u.ID == document.CreateBy);
            document.CreateName = s.Name;

            var spp = context.Suppliers.SingleOrDefault(u => u.ID == document.SpID);
            document.SupplierName = spp.Name;

            document.DocDate = document.DocDate.Value.AddYears(543);
            
            if (document == null)
            {
                return HttpNotFound();
            }

            document.StatusName = GetStatusName(document.Status);

            var list = (from d in context.Documents
                        join du in context.DocumentDetails on d.ID equals du.DocID
                        join w in context.Warehouses on d.WhID equals w.ID
                        join sp in context.Suppliers on d.SpID equals sp.ID
                        join st in context.Staffs on d.CreateBy equals st.ID
                        join p in context.Products on du.PdID equals p.ID
                        where du.DocID == document.ID
                        select new {
                            PdID = du.PdID,
                            ProductName = p.Name,
                            Qty = du.Qty,
                            SerialBegin = du.SerialBegin,
                            SerialEnd = du.SerialEnd,
                            IpProperty = du.IpProperty,
                            IsAsset = p.IsAsset,
                            SerialControl = p.SerialControl
                        }).AsEnumerable().Select(x => new DocDetailList
                        {
                            PdID = x.PdID,
                            ProductName = x.ProductName,
                            Qty = x.Qty,
                            SerialBegin = x.SerialBegin,
                            SerialEnd = x.SerialEnd,
                            IpProperty = x.IpProperty,
                            IsAsset = x.IsAsset,
                            SerialControl = x.SerialControl
                        }).ToList();


            document.DocDetailList = list;

            ViewBag.CreateBy = new SelectList(db.Staffs.ToList(), "ID", "Name");
            ViewBag.SpID = db.Suppliers.ToList();
            ViewBag.PdID = new SelectList(db.Products.ToList(), "ID", "Name");
            ViewBag.WhID = db.Warehouses.ToList();
            return View(document);
        }

        // POST: Receives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DocNo,DocDate,CreateBy,DocType,WhID,SpID,Remark,Status,ApproveDate,ApproveBy,ProcessDate,ProcessBy,ToWhID")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreateBy = new SelectList(db.Staffs, "ID", "Name", document.CreateBy);
            ViewBag.SpID = new SelectList(db.Suppliers, "ID", "Name", document.SpID);
            return View(document);
        }

        //Edit Receive
        [HttpPost]
        public ActionResult EditReceive(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) && (p.ID != model.ID)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }
            model.DocDate = model.DocDate.Value.AddYears(-543);
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            //Delete old data
            db.DocumentDetails.RemoveRange(db.DocumentDetails.Where(x => x.DocID == model.ID));
            db.SaveChanges();


            model.DocDetailList.RemoveAt(0);
            foreach (var i in model.DocDetailList)
            {
                DocumentDetail dd = new DocumentDetail();
                dd.DocID = model.ID;
                dd.PdID = i.PdID;
                dd.Qty = i.Qty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "I";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();
            return Json("success");
        }
        // GET: Receives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        [HttpPost]
        public ActionResult SendApprove(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) && (p.ID != model.ID)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }
            //Update Status
            var context = new DLSMEntities();
            var upd = (from d in context.Documents
                       where d.ID == model.ID
                       select d).ToList().SingleOrDefault();
            upd.Status = model.Status;

            context.SaveChanges();

            return Json("success");
        }

        //Approve & Insert Stock
        [HttpPost]
        public ActionResult Approve(Document model)
        {
            var oldstatus = model.Status;
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    string snError = CheckSerialIn(model);
                    if (snError == "Y")
                    {
                        try
                        {
                            //Update Status
                            var upd = (from d in context.Documents
                                       where d.ID == model.ID
                                       select d).ToList().SingleOrDefault();
                            upd.Status = model.Status;
                            upd.ApproveBy = model.ApproveBy;
                            upd.ApproveDate = model.ApproveDate;
                            upd.Remark = model.Remark;
                            context.SaveChanges();


                            // Execute store procudure
                            var docid = model.ID;
                            var userid = int.Parse(Session["UserID"].ToString());

                            var result = context.sp_ProcessDocument(docid, userid).ToList();

                            if (result[0].ID == 0)
                            {
                                context.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            //update status ก่อนหน้า
                            var upd = (from d in context.Documents
                                       where d.ID == model.ID
                                       select d).ToList().SingleOrDefault();
                            upd.Status = oldstatus;
                            context.SaveChanges();

                            return Json(ex.InnerException.Message);
                        }
                    }
                    else
                    {
                        return Json(snError != "Y" ? snError : "");
                    }
                }
            }      
            return Json("success");
        }

        //InsertAndAppr Approve & Insert Stock
        [HttpPost]
        public ActionResult InsertAndAppr(Document model)
        {
            //var oldstatus = model.Status;
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    string snError = CheckSerialIn(model);
                    if (snError == "Y")
                    {
                        try
                        {
                            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo)).FirstOrDefault();

                            if (w != null)
                            {
                                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
                            }
                            context.Documents.Add(model);
                            context.SaveChanges();

                            int docid = model.ID;

                            model.DocDetailList.RemoveAt(0);
                            foreach (var i in model.DocDetailList)
                            {
                                DocumentDetail dd = new DocumentDetail();
                                dd.DocID = docid;
                                dd.PdID = i.PdID;
                                dd.Qty = i.Qty;
                                dd.SerialBegin = i.SerialBegin;
                                dd.SerialEnd = i.SerialEnd;
                                dd.IpProperty = i.IpProperty;
                                dd.TrnType = "I";
                                context.DocumentDetails.Add(dd);
                            }
                            context.SaveChanges();

                            // Execute store procudure
                            var userid = int.Parse(Session["UserID"].ToString());

                            var result = context.sp_ProcessDocument(docid, userid).ToList();

                            if (result[0].ID == 0)
                            {
                                context.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            return Json(ex.InnerException.Message);
                        }
                    }
                    else
                    {
                        return Json(snError != "Y" ? snError : "");
                    }
                }
            }

            return Json("success");
        }

        //Reject 
        [HttpPost]
        public ActionResult Reject(Document model)
        {
            //Update Status
            var context = new DLSMEntities();
            var upd = (from d in context.Documents
                       where d.ID == model.ID
                       select d).ToList().SingleOrDefault();
            upd.Status = model.Status;
            upd.Remark = model.Remark;
            context.SaveChanges();


            return Json("success");
        }
        // GET: Receives/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            //Delete old data
            db.DocumentDetails.RemoveRange(db.DocumentDetails.Where(x => x.DocID == id));
            db.SaveChanges();

            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();

            TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetWH()
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find staff in warehouse
                var SessionwhID = Session["UserWhID"].ToString();
                var SessionUserID = Session["UserID"].ToString();
                var wh = (from s in context.Staffs
                          join st in context.StaffWarehouses on s.ID equals st.StID
                          join w in context.Warehouses on st.WhID equals w.ID
                          where s.ID.ToString() == SessionUserID
                          select new
                          {
                              ID = st.WhID,
                              Name = w.Name
                          }).ToList();

                return Json(wh);
            }
        }

        [HttpPost]
        public ActionResult IsWarehouseManager(string whcode)
        {
            try
            {
                var operation = new string[] { "1", "2" };
                List<StaffWarehouse> UserWhID = (List<StaffWarehouse>)Session["WarehouseIsManger"];
                
                if (UserWhID.Where(x => x.WhID == Int32.Parse(whcode) ).Count() > 0)
                {
                    if (UserWhID.Where(s => s.WhID == Int32.Parse(whcode) && operation.Contains(s.IsManager)).Select(x => x).Count() >= 2)
                    {
                        // both
                        return Json("4");
                    }
                    else if(UserWhID.Where(x => x.WhID == Int32.Parse(whcode) && x.IsManager == "1").Count() > 0)
                    {
                        // ผู้จัดการ
                        return Json("1");
                    }
                    else if(UserWhID.Where(x => x.WhID == Int32.Parse(whcode) && x.IsManager == "2").Count() > 0)
                    {
                        // เจ้าหน้าที่
                        return Json("2");
                    }
                    else if (UserWhID.Where(x => x.WhID == Int32.Parse(whcode) && x.IsManager == "3").Count() > 0)
                    {
                        // reviewer
                        return Json("3");
                    }
                    else
                    {
                        throw new Exception();
                    }
                    
                }
                else
                {
                    // ไม่มีสิทธิ์
                    return Json("0");
                }
                
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public String GetStatusName(String statusid)
        {
            String Name = null;
            if (statusid.ToString().Contains("1"))
            {
                Name = "กำลังดำเนินการ";
            }
            else if (statusid.ToString().Contains("2"))
            {
                Name = "รออนุมัติ";
            }
            else if (statusid.ToString().Contains("3"))
            {
                Name = "อนุมัติ";
            }
            else if (statusid.ToString().Contains("4"))
            {
                Name = "ไม่อนุมัติ";
            }            
            return Name;
        }

        protected String CheckSerialIn(Document model)
        {
            string retcode = null;
            using (DLSMEntities context = new DLSMEntities())
            {
                try
                {
                    // Execute store procudure
                    var docid = model.ID;

                    var result = context.sp_CheckSerialINReceive(docid).ToList();

                    if (result[0].ID == 0)
                    {
                        retcode = "Y";
                    }

                }
                catch (Exception ex)
                {
                    retcode = ex.InnerException.Message;
                }
            }
            return retcode;
        }

    }
}
