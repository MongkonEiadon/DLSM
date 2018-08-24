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

namespace DLSM.Controllers
{
    [SessionCheck]
    public class CheckStocksController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: CheckStocks
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }


            var context = new DLSMEntities();
            int Userwhid = Convert.ToInt32(Session["UserWhID"]);
            var list = (from d in context.Documents
                        join du in context.DocumentDetails on d.ID equals du.DocID
                        join w in context.Warehouses on d.WhID equals w.ID
                        join p in context.Products on du.PdID equals p.ID
                        join s in context.Staffs on d.CreateBy equals s.ID
                        orderby d.DocDate descending
                        where d.DocType == "6" && (d.WhID == Userwhid ) //ตรวจนับ
                        select new
                        {
                            ID = d.ID,
                            DocNo = d.DocNo,
                            DocDate = d.DocDate,
                            CreateBy = d.CreateBy,
                            CreateName = s.Name,
                            WhID = d.WhID,
                            WarehouseName = w.Name,
                            SpID = du.PdID,
                            ProductName = p.Name,
                            Remark = d.Remark,
                            Status = d.Status,
                            ApproveDate = d.ApproveDate,
                            ApproveBy = d.ApproveBy,
                            ProcessDate = d.ProcessDate,
                            ProcessBy = d.ProcessBy,
                            Qty = du.Qty
                        }).AsEnumerable().Select(x => new Document
                        {
                            ID = x.ID,
                            DocNo = x.DocNo,
                            DocDate = x.DocDate.Value.AddYears(543),
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            WhID = x.WhID,
                            WarehouseName = x.WarehouseName,
                            SpID = x.SpID,
                            ProductName = x.ProductName,
                            Remark = x.Remark,
                            Status = x.Status,
                            ApproveDate = x.ApproveDate,
                            ApproveBy = x.ApproveBy,
                            ProcessDate = x.ProcessDate,
                            ProcessBy = x.ProcessBy,
                            Qty = x.Qty
                        }).ToList();

            foreach (var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }

            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View(list);
        }

        public ActionResult Search(DateTime? searchFromDate, DateTime? searchToDate, int? searchWare, int? searchPro)
        {
            var context = new DLSMEntities();
            int Userwhid = Convert.ToInt32(Session["UserWhID"]);
            var list = (from d in context.Documents
                        join du in context.DocumentDetails on d.ID equals du.DocID
                        join w in context.Warehouses on d.WhID equals w.ID
                        join p in context.Products on du.PdID equals p.ID
                        join s in context.Staffs on d.CreateBy equals s.ID
                        orderby d.DocDate descending
                        where 
                            ( 
                                (d.DocDate >= searchFromDate || searchFromDate == null) && 
                                (d.DocDate <= searchToDate || searchToDate == null ) && 
                                (d.WhID == searchWare || searchWare == null) && 
                                (du.PdID == searchPro || searchPro == null)
                            )
                            && d.ID == du.DocID && d.DocType == "6" && (d.WhID == Userwhid ) //ตรวจนับ
                        select new
                        {
                            ID = d.ID,
                            DocNo = d.DocNo,
                            DocDate = d.DocDate,
                            CreateBy = d.CreateBy,
                            CreateName = s.Name,
                            WhID = d.WhID,
                            WarehouseName = w.Name,
                            SpID = du.PdID,
                            ProductName = p.Name,
                            Remark = d.Remark,
                            Status = d.Status,
                            ApproveDate = d.ApproveDate,
                            ApproveBy = d.ApproveBy,
                            ProcessDate = d.ProcessDate,
                            ProcessBy = d.ProcessBy,
                            Qty = du.Qty
                        }).AsEnumerable().Select(x => new Document
                        {
                            ID = x.ID,
                            DocNo = x.DocNo,
                            DocDate = x.DocDate.Value.AddYears(543),
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            WhID = x.WhID,
                            WarehouseName = x.WarehouseName,
                            SpID = x.SpID,
                            ProductName = x.ProductName,
                            Remark = x.Remark,
                            Status = x.Status,
                            ApproveDate = x.ApproveDate,
                            ApproveBy = x.ApproveBy,
                            ProcessDate = x.ProcessDate,
                            ProcessBy = x.ProcessBy,
                            Qty = x.Qty
                        }).ToList();

            foreach (var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }

            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View("Index", list);
            //return PartialView("CheckStockSearch", list);
        }

        // GET: CheckStocks/Details/5
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

        // GET: CheckStocks/Create
        public ActionResult Create()
        {
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.PdID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: CheckStocks/Create
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
        public ActionResult Insert(Document model)
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

                dd.Qty = (i.Qty == null ? 0 : i.Qty);
                dd.RemainQty = i.RemainQty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "A";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            foreach (var i in model.DocDetailList)
            { 
                //Update to Unlock Row
                List<Stock> res = (from s in db.Stocks
                               where s.WhID == model.WhID && s.PdID == i.PdID
                               select s).ToList();


                res.ForEach(o => o.CheckStatus = "Y");
                db.SaveChanges();
            }
            return Json("success");
        }

        [HttpPost]
        public ActionResult InsertSendApp(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) ).FirstOrDefault();

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
                dd.Qty = (i.Qty == null ? 0 : i.Qty);
                dd.RemainQty = i.RemainQty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "A";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            return Json("success");
        }

        // GET: CheckStocks/Edit/5
        public ActionResult Edit(int? id)
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
            var context = new DLSMEntities();
            var wh = context.Warehouses.SingleOrDefault(u => u.ID == document.WhID);
            document.WhID = wh.ID;
            document.WhName = wh.Name;
          

            var s = context.Staffs.SingleOrDefault(u => u.ID == document.CreateBy);
            document.CreateName = s.Name;

            document.DocDate = document.DocDate.Value.AddYears(543);

            document.StatusName = GetStatusName(document.Status);

            var list = (from d in context.Documents
                        join du in context.DocumentDetails on d.ID equals du.DocID
                        join w in context.Warehouses on d.WhID equals w.ID
                        join st in context.Staffs on d.CreateBy equals st.ID
                        join p in context.Products on du.PdID equals p.ID
                        where du.DocID == document.ID
                        select new
                        {
                            PdID = du.PdID,
                            ProductName = p.Name,
                            Qty = du.Qty,
                            RemainQty = du.RemainQty,
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
                            RemainQty = x.RemainQty,
                            SerialBegin = x.SerialBegin,
                            SerialEnd = x.SerialEnd,
                            IpProperty = x.IpProperty,
                            IsAsset = x.IsAsset,
                            SerialControl = x.SerialControl
                        }).ToList();

            

            document.DocDetailList = list;

            ViewBag.CreateBy = new SelectList(db.Staffs.ToList(), "ID", "Name");
            ViewBag.PdID = new SelectList(db.Products.ToList(), "ID", "Name");
            ViewBag.WhID = db.Warehouses.ToList();


            return View(document);
        }

        // POST: CheckStocks/Edit/5
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

        [HttpPost]
        public ActionResult EditCheckStock(Document model)
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
                dd.Qty = (i.Qty == null ? 0 : i.Qty);
                dd.RemainQty = i.RemainQty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "A";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            foreach (var i in model.DocDetailList)
            {
                //Update to Unlock Row
                List<Stock> res = (from s in db.Stocks
                                   where s.WhID == model.WhID && s.PdID == i.PdID
                                   select s).ToList();


                res.ForEach(o => o.CheckStatus = "Y");
                db.SaveChanges();
            }

            return Json("success");
        }

        // GET: CheckStocks/Delete/5
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

        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            //Update to Unlock Row
            List<Stock> res = (from dd in db.DocumentDetails
                               join d in db.Documents on dd.DocID equals d.ID
                               join s in db.Stocks on new { a = dd.PdID.Value, b = d.WhID.Value } equals new { a = s.PdID, b = s.WhID } into ss
                               from x in ss.DefaultIfEmpty()
                               where d.ID == id
                               select x).ToList();
            
            res.ForEach(o => o.CheckStatus = "N");
            db.SaveChanges();
            
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


            foreach (var i in model.DocDetailList)
            {
                //Update to Unlock Row
                List<Stock> res = (from s in db.Stocks
                                   where s.WhID == model.WhID && s.PdID == i.PdID
                                   select s).ToList();

                res.ForEach(o => o.CheckStatus = "N");
                db.SaveChanges();
            }


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
                    try
                    {
                        //Update Status
                        var upd = (from d in context.Documents
                                   where d.ID == model.ID
                                   select d).ToList().SingleOrDefault();
                        upd.Status = model.Status;
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
                        //Update Status
                        var upd = (from d in context.Documents
                                   where d.ID == model.ID
                                   select d).ToList().SingleOrDefault();
                        upd.Status = oldstatus;

                        context.SaveChanges();

                        return Json(ex.Message);
                    }
                }
            }
                     
            return Json("success");
        }


        //InsertAndAppr Approve & Insert Stock
        [HttpPost]
        public ActionResult InsertAndAppr(Document model)
        {            
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) ).FirstOrDefault();

                        if (w != null)
                        {
                            return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
                        }
                        context.Documents.Add(model);
                        context.SaveChanges();

                        var docid = model.ID;

                        model.DocDetailList.RemoveAt(0);
                        foreach (var i in model.DocDetailList)
                        {
                            DocumentDetail dd = new DocumentDetail();
                            dd.DocID = docid;
                            dd.PdID = i.PdID;
                            dd.Qty = i.Qty;
                            dd.RemainQty = i.RemainQty;
                            dd.SerialBegin = i.SerialBegin;
                            dd.SerialEnd = i.SerialEnd;
                            dd.IpProperty = i.IpProperty;
                            dd.TrnType = "A";
                            context.DocumentDetails.Add(dd);
                        }

                        context.SaveChanges();


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

        [HttpGet]
        public ActionResult checkQtyByProduct(int WhID, int PrdID)
        {

            string res = "0";
            using (DLSMEntities context = new DLSMEntities())
            {
                var qty = context.Stocks.Where(o => o.WhID == WhID && o.PdID == PrdID).Select(s => s.Qty).SingleOrDefault();

                if (null != qty)
                {
                    res = qty.ToString();
                }
                else
                {
                    res = "0";
                }

            }

            return this.Json(res, JsonRequestBehavior.AllowGet);
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
            else if (statusid.ToString().Contains("5"))
            {
                Name = "รับของ";
            }
            else if (statusid.ToString().Contains("6"))
            {
                Name = "อนุมัติรับของ";
            }
            else if (statusid.ToString().Contains("7"))
            {
                Name = "ไม่อนุมัติรับของ";
            }
            else if (statusid.ToString().Contains("8"))
            {
                Name = "รับคืน";
            }
            return Name;
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

                if (UserWhID.Where(x => x.WhID == Int32.Parse(whcode)).Count() > 0)
                {
                    if (UserWhID.Where(s => s.WhID == Int32.Parse(whcode) && operation.Contains(s.IsManager)).Select(x => x).Count() >= 2)
                    {
                        // both
                        return Json("4");
                    }
                    else if (UserWhID.Where(x => x.WhID == Int32.Parse(whcode) && x.IsManager == "1").Count() > 0)
                    {
                        // ผู้จัดการ
                        return Json("1");
                    }
                    else if (UserWhID.Where(x => x.WhID == Int32.Parse(whcode) && x.IsManager == "2").Count() > 0)
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
    }
}
