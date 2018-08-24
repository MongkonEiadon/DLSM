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
    public class BorrowsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Borrows
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
                        join tw in context.Warehouses on d.ToWhID equals tw.ID
                        join p in context.Products on du.PdID equals p.ID
                        join s in context.Staffs on d.CreateBy equals s.ID
                        orderby d.DocDate descending
                        where  (d.DocType ==  "4" || d.DocType == "5" )
                        && (d.WhID == Userwhid || d.ToWhID == Userwhid) 
                        && du.TrnType.ToString() == "O" //ยืมคืน
                        select new
                        {
                            ID = d.ID,
                            DocNo = d.DocNo,
                            DocDate = d.DocDate,
                            CreateBy = d.CreateBy,
                            CreateName = s.Name,
                            WhID = d.WhID,
                            WarehouseName = w.Name,
                            ToWhID = d.ToWhID,
                            ToWarehouseName = tw.Name,
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
                            ToWhID = x.ToWhID,
                            ToWarehouseName = x.ToWarehouseName,
                            SpID = x.SpID,
                            ProductName = x.ProductName,
                            Remark = x.Remark,
                            Status = x.Status,
                            ApproveDate = x.ApproveDate,
                            ApproveBy = x.ApproveBy,
                            ProcessDate = x.ProcessDate,
                            ProcessBy = x.ProcessBy,
                            Qty = x.Qty.Value
                        }).ToList();

            //&& (d.WhID == Userwhid || d.ToWhID == Userwhid)
            foreach (var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }

            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View(list);
        }

        public ActionResult Search(DateTime? searchFromDate, DateTime? searchToDate, int? searchWare, int? searchPro, string searchSN)
        {
            var context = new DLSMEntities();
            int Userid = Convert.ToInt32(Session["UserID"]);
            List<Document> list = new List<Document>();
            var listdata = context.sp_SearchDocument(searchSN,searchFromDate, searchToDate, searchWare, searchPro,4,null,Userid).ToList();

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
                    d.ToWhID = i.ToWhID;
                    d.ToWarehouseName = i.ToWarehouseName;
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

        // GET: Borrows/Details/5
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

        // GET: Borrows/Create
        public ActionResult Create()
        {
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.PdID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Borrows/Create
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
                dd.Qty = i.Qty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "O";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            return Json("success");
        }

        [HttpPost]
        public ActionResult InsertBorrow(Document model)
        {
            var sendidback = "";
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) && (p.ID != model.ID)).FirstOrDefault();

                        //if (w != null)
                        //{
                        //    return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
                        //}

                        //Document LastDocNo = db.Documents.Where(d => d.DocNo == model.DocNo || d.DocNo.StartsWith(model.DocNo + "/")).OrderByDescending(o => o.ID).First();
                        //string docno = "";
                        //if (LastDocNo.DocNo.IndexOf("/") >= 0)
                        //{
                        //    docno = (Int32.Parse(LastDocNo.DocNo.Substring(LastDocNo.DocNo.Length - 2, 2)) + 1).ToString("00");
                        //    docno = model.DocNo + "/" + docno;
                        //}
                        //else
                        //{
                        //    docno = LastDocNo.DocNo + "/01";
                        //}


                        //Document du = new Document();
                        //du.DocNo = docno;
                        //du.DocDate = model.DocDate;
                        //du.DocType = model.DocType;
                        //du.WhID = model.ToWhID; //ตอนคืน คลังต้นทางกับปลางทางต้องสลับกัน
                        //du.ToWhID = model.WhID;
                        //du.SpID = model.SpID; 
                        //du.Status = model.Status;
                        //du.Remark = model.Remark;
                        //du.CreateBy = model.CreateBy;

                        //db.Documents.Add(du);
                        //db.SaveChanges();

                        //int docid2 = du.ID;
                        //sendidback = docid2.ToString(); //สำหรับส่งค่า return id


                        //var remain = db.DocumentDetails.Where(p => p.DocID == model.ID && p.TrnType == "O").Select(p => p.RemainQty).FirstOrDefault();

                        //model.DocDetailList.RemoveAt(0);
                        //foreach (var i in model.DocDetailList)
                        //{
                        //    DocumentDetail dd = new DocumentDetail();
                        //    dd.DocID = docid2;
                        //    dd.PdID = i.PdID;
                        //    dd.Qty = i.Qty;
                        //    dd.SerialBegin = i.SerialBegin;
                        //    dd.SerialEnd = i.SerialEnd;
                        //    dd.RemainQty = remain;
                        //    dd.IpProperty = i.IpProperty;
                        //    dd.TrnType = "O";
                        //    db.DocumentDetails.Add(dd);
                        //}
                        //db.SaveChanges();

                        var result = db.sp_CreateBorrow(model.ID, model.DocNo, "O", model.CreateBy).ToList(); //trntype  O for ger remain Qty
                        if (result.Count() > 0)
                        {
                            dbContextTransaction.Commit();
                            return Json(new { msg = "success", docid2 = result[0].ID });
                        }

                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(ex.InnerException.Message);
                    }
                }
            }
        }
            
            

        // GET: Borrows/Edit/5
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
            var wh2 = context.Warehouses.SingleOrDefault(u => u.ID == document.ToWhID);
            document.ToWhID = wh2.ID;
            document.ToWarehouseName = wh2.Name;

            var s = context.Staffs.SingleOrDefault(u => u.ID == document.CreateBy);
            document.CreateName = s.Name;

            document.DocDate = document.DocDate.Value.AddYears(543);

            document.StatusName = GetStatusName(document.Status);
          
            var list = (from d in context.Documents
                        join du in context.DocumentDetails on d.ID equals du.DocID
                        join w in context.Warehouses on d.WhID equals w.ID
                        //join sp in context.Suppliers on d.SpID equals sp.ID
                        join st in context.Staffs on d.CreateBy equals st.ID
                        join p in context.Products on du.PdID equals p.ID
                        where du.DocID == document.ID && du.TrnType.Contains("O")
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

        // POST: Borrows/Edit/5
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
        public ActionResult EditBorrow(Document model)
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
                dd.RemainQty = i.RemainQty;
                dd.SerialBegin = i.SerialBegin;
                dd.SerialEnd = i.SerialEnd;
                dd.IpProperty = i.IpProperty;
                dd.TrnType = "O";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();
            return Json("success");
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

      
        // GET: Borrows/Delete/5
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
                    if(model.Status.ToString() == "3")
                    { 
                        string avbError = CheckAvailable(model);
                        string snError = CheckSerialOut(model);
                        if (avbError == "Y" && snError == "Y")
                        {
                            try
                            {
                                model.DocDate = model.DocDate.Value.AddYears(-543);                                
                                //Update Status ของ 3 เพราะ Out
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
                                    throw new Exception();
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
                    if (model.Status.ToString() == "6")
                    {
                        try
                        { 
                            string snError2 = CheckSerialIn(model);
                            if (snError2 == "Y")
                            {
                                //Update Status ของ 6 เพราะ In
                                var upd2 = (from d in context.Documents
                                            where d.ID == model.ID
                                            select d).ToList().SingleOrDefault();
                                upd2.Status = model.Status;
                                upd2.Remark = model.Remark;
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
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                //return Json();
                                dbContextTransaction.Rollback();
                                throw new Exception(snError2 != "Y" ? snError2 : "");
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
                        string avbError = CheckAvailable(model);
                        string snError = CheckSerialOut(model);
                    if (avbError == "Y" && snError == "Y")
                    {
                        try
                        {
                            Document w = context.Documents.Where(p => (p.DocNo == model.DocNo) ).FirstOrDefault();

                            if (w != null)
                            {
                                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
                            }
                            context.Documents.Add(model);
                            context.SaveChanges();

                            model.DocDetailList.RemoveAt(0);
                            foreach (var i in model.DocDetailList)
                            {
                                DocumentDetail dd = new DocumentDetail();
                                dd.DocID = model.ID;
                                dd.PdID = i.PdID;
                                dd.Qty = i.Qty;
                                dd.RemainQty = i.RemainQty;
                                dd.SerialBegin = i.SerialBegin;
                                dd.SerialEnd = i.SerialEnd;
                                dd.IpProperty = i.IpProperty;
                                dd.TrnType = "O";
                                context.DocumentDetails.Add(dd);
                            }
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
                              
                        }
                        catch(Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            return Json(ex.InnerException.Message);
                        }
                    }
                }
            }

            return Json("success");
        }

        //รับคืน รับของ
        [HttpPost]
        public ActionResult ReceiveStock(Document model)
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
                        dbContextTransaction.Commit();

                        if (model.Status.ToString() == "8") //ถ็า Reject มา Status ก่อนหน้าเป็น 7 ต้องรับของคืน Status จะเป็น 8 และแก้ไข Stock
                        {
                            string avbError = CheckAvailable(model);
                            string snError = CheckSerialIn(model);
                            if (avbError == "Y" && snError == "Y")
                            {
                                // Execute store procudure
                                var docid = model.ID;
                                var userid = int.Parse(Session["UserID"].ToString());

                                var result = context.sp_ProcessDocument(docid, userid).ToList();

                                if (result[0].ID == 1)
                                {
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else
                                {
                                    dbContextTransaction.Rollback();
                                }
                            }
                            else
                            {
                                //return Json();
                                throw new Exception(avbError != "Y" ? avbError : "" + snError != "Y" ? snError : "");
                            }


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

        //รับคืน
        [HttpPost]
        public ActionResult ReceiveBack(Document model)
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

        protected String CheckAvailable(Document model)
        {
            string retcode = null;
            using (DLSMEntities context = new DLSMEntities())
            {
                try
                {
                    // Execute store procudure
                    var docid = model.ID;

                    var result = context.sp_CheckAvailable(docid).ToList();

                    if (result[0].RetCodeAvb == 0)
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


        protected String CheckSerialOut(Document model)
        {
            string retcode = null;
            using (DLSMEntities context = new DLSMEntities())
            {
                try
                {
                    // Execute store procudure
                    var docid = model.ID;

                    var result = context.sp_CheckSerialOut(docid).ToList();

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

        protected String CheckSerialIn(Document model)
        {
            string retcode = null;
            using (DLSMEntities context = new DLSMEntities())
            {
                try
                {
                    // Execute store procudure
                    var docid = model.ID;

                    var result = context.sp_CheckSerialIN(docid).ToList();

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

        [HttpGet]
        public ActionResult checkQtyByProduct(int WhID, int PrdID)
        {

            string res = "n";
            using (DLSMEntities context = new DLSMEntities())
            {
                var qty = context.Stocks.Where(o => o.WhID == WhID && o.PdID == PrdID).Select(s => s.Qty).SingleOrDefault();

                if (null != qty)
                {
                    if (int.Parse(qty.ToString()) > 0)
                        res = "y";
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
