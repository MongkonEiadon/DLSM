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
using System.Text.RegularExpressions;
using System.Collections;

namespace DLSM.Controllers
{
    public class StockSerialNum
    {
        public StockSerialNum() { }
        public StockSerialNum(int ID, int PdID, int SerialBegin, int SerialCount, int SerialEnd) {
            this.ID = ID;
            this.PdID = PdID;
            this.SerialBegin = SerialBegin;
            this.SerialCount = SerialCount;
            this.SerialEnd = SerialEnd;

        }
        public int ID;
        public int PdID;
        public long SerialBegin;
        public int SerialCount;
        public long SerialEnd;
    }
    [SessionCheck]
    public class TransfersController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Transfers
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
                        where d.DocType == "3" && (d.WhID == Userwhid  || d.ToWhID == Userwhid)  //Transfer 
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
                            Qty = du.Qty,
                            SubType = d.SubType
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
                            Qty = x.Qty,
                            SubType = x.SubType
                        }).ToList();


            foreach(var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }
            
            ViewBag.WarehouseList = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View(list);
        }

        public ActionResult Search(DateTime? searchFromDate, DateTime? searchToDate, int? searchWare, int? searchPro, int? searchStatus, string searchSN)
        {
            var context = new DLSMEntities();
            int Userid = Convert.ToInt32(Session["UserID"]);
            List<Document> list = new List<Document>();
            var listdata = context.sp_SearchDocument(searchSN, searchFromDate, searchToDate, searchWare, searchPro, 3, searchStatus,Userid).ToList();

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
                    d.SubType = i.SubType;
                    list.Add(d);
                }
            }
            ViewBag.WarehouseList = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            return View("Index", list);
        }

        // GET: Transfers/Details/5
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

        // GET: Transfers/Create
        public ActionResult Create()
        {
            ViewBag.WhID = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.PdID = new SelectList(db.Products, "ID", "Name");
            return View();
            
        }

        // POST: Transfers/Create
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

        // GET: Transfers/Edit/5
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
                        where du.DocID == document.ID && du.TrnType == "O"
                        select new
                        {
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
            ViewBag.PdID = new SelectList(db.Products.ToList(), "ID", "Name");
            ViewBag.WhID = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.WarehouseAccess = Session["WarehouseAccess"];

            return View(document);
        }

        // POST: Transfers/Edit/5
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
        public ActionResult EditTransfer(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) && (p.ID != model.ID)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }


            var chkResults = this.chk_Stock(model.ID, db);
            var chkOk = (bool)chkResults[0];
            var chkMsg = (string)chkResults[1];
            if (!chkOk)
            {
                return Json(chkMsg);
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
                dd.TrnType = "O";
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();
            return Json("success");
        }



        public object[] chk_Stock(int DocID, DLSMEntities _context)
        {

            bool chkOk = true;
            string strError = "";
            _context = null;
            using (DLSMEntities context = _context != null ? _context : new DLSMEntities())
            {
                var docid = DocID;
                var Doc = context.Documents.SingleOrDefault(a => a.ID == DocID);
                if (Doc == null)
                {
                    return new object[] { false, "data not found." };
                }
                /*
                var isCard = (from docdet in context.DocumentDetails
                              join ss in context.StockSerials on docdet.PdID equals ss.PdID
                              join p in context.Products on docdet.PdID equals p.ID
                              join c in context.Configures on p.Code equals c.Value
                              where docdet.DocID == DocID && docdet.Document.WhID == Doc.WhID
                              && c.Code == "PRD-CARD"
                              select p).FirstOrDefault() != null;*/
                var dd = (from docdet in context.DocumentDetails
                          join p in context.Products on docdet.PdID equals p.ID
                          join c in context.Configures on p.Code equals c.Value
                          where docdet.DocID == Doc.ID && docdet.TrnType == "O"  && docdet.Document.WhID == Doc.WhID
                          select new { docdet.DocID,docdet.PdID,docdet.Qty,docdet.SerialBegin,docdet.SerialEnd,c.Code,docdet.Product});

                var ss_grp = (from docdet in context.DocumentDetails
                              join ss in context.StockSerials on docdet.PdID equals ss.PdID
                              join p in context.Products on docdet.PdID equals p.ID
                              where docdet.TrnType == "O" && p.SerialControl == "Y" && docdet.Document.WhID == Doc.WhID
                              group ss by new { ss.ID, ss.PdID, ss.SerialBegin, ss.SerialCount, ss.SerialEnd } into g
                              select g).AsEnumerable().Select(g =>
                               new StockSerialNum()
                               {
                                   ID = g.Key.ID,
                                   PdID = g.Key.PdID.HasValue ? g.Key.PdID.Value : 0,
                                   SerialBegin = Convert.ToInt64(g.Key.SerialBegin),
                                   SerialCount = g.Key.SerialCount.HasValue ? g.Key.SerialCount.Value : 0,
                                   SerialEnd = Convert.ToInt64(g.Key.SerialEnd)
                               });


                var htb = new Hashtable();
                if (dd.Count() == 0) { return new object[] { false, "data not found.." }; }
                foreach (var docdet in dd)
                {
                    bool isCard = docdet.Code == "PRD-CARD";
                    if (isCard)
                    {
                        var ddBegin = long.Parse(docdet.SerialBegin);
                        var ddEnd = long.Parse(docdet.SerialEnd);
                        var objExist = ss_grp.SingleOrDefault(a => a.PdID == docdet.PdID && ddBegin >= a.SerialBegin && ddEnd <= a.SerialEnd);
                        if (objExist == null)
                        {
                            strError = docdet.Product.Name + " serial ที่ระบุไม่มีอยู่ในคลัง";
                            chkOk = false;
                            break;
                        }
                        #region comment  for support chk multi record
                        //else
                        //{

                        /*
                        var objBegin = new StockSerialInt(-99, objExist.PdID, objExist.SerialBegin, -99, objExist.SerialEnd);
                        var objEnd = new StockSerialInt(-99, objExist.PdID, objExist.SerialBegin, -99, objExist.SerialEnd);
                        objExist.SerialBegin 
                        ss_grp.Remove(objExist);
                        */
                        //}
                        #endregion
                    }
                    else {
                        #region not isCard                    
                            if (!htb.ContainsKey(docdet.PdID)) { htb.Add(docdet.PdID, 0); }
                            int lastSumQty = Convert.ToInt32(htb[docdet.PdID]);
                            var existObj = context.Stocks.FirstOrDefault(a => a.WhID == Doc.WhID && a.PdID == docdet.PdID && a.Qty >= (lastSumQty + docdet.Qty));
                            if (existObj != null)
                            {
                                htb[docdet.PdID] = Convert.ToInt32(htb[docdet.PdID]) + docdet.Qty;
                            }
                            else
                            {
                                strError = docdet.Product.Name + " serial ที่ระบุไม่มีอยู่ในคลัง";
                                break;
                            }

                     
                        #endregion
                    }
                }

              
             

            }


            return new object[] { chkOk, strError };

        }





        // GET: Transfers/Delete/5
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
        public ActionResult checkSerialInRequisitions(int DocID, int prdID, string SNBegin, string SNEnd)
        {

            bool inSNBegin = false;
            bool inSNEnd = false;

            try
            {

                var doc = db.DocumentDetails.Where(d => d.DocID == DocID && d.PdID == prdID).ToList();
            
                // Extract Prifix from Serial
                var chkSNBeginPrefix = Regex.Match(SNBegin, "\\D+").Value;
                var chkSNEndNoPrefix = Regex.Match(SNEnd, "\\D+").Value;
                // Extract Number from Serial
                var chkSNBeginNo = Regex.Match(SNBegin, "\\d+").Value;
                var chkSNEndNo = Regex.Match(SNEnd, "\\d+").Value;


                foreach(var d in doc)
                {
                    // Extract Prefix SN from DocumentDetail
                    var docSNBeginPrefix = Regex.Match(d.SerialBegin, "\\D+").Value;
                    var docSNEndNoPrefix = Regex.Match(d.SerialEnd, "\\D+").Value;
                    // Extract Number SN from DocumentDetail
                    var docSNBeginNo = Regex.Match(d.SerialBegin, "\\d+").Value;
                    var docSNEndNo = Regex.Match(d.SerialEnd, "\\d+").Value;

                    if (chkSNBeginPrefix == docSNBeginPrefix && chkSNEndNoPrefix == docSNEndNoPrefix)
                    {
                        if (Int64.Parse(chkSNBeginNo) >= Int64.Parse(docSNBeginNo))
                        {
                            inSNBegin = true;
                        }
                        
                        if(Int64.Parse(chkSNEndNo) <= Int64.Parse(docSNEndNo))
                        {

                            inSNEnd = true;
                        }
                    }
                }
                
                if (inSNBegin == true && inSNEnd == true)
                {
                    return Json("success");
                }
                else
                {
                    return Json("serial not match");
                }
                

            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }


            
        }

        [HttpPost]
        public ActionResult SendApprove(Document model)
        {
            Document w = db.Documents.Where(p => (p.DocNo == model.DocNo) && (p.ID != model.ID)).FirstOrDefault();

            if (w != null)
            {
                return Json("เลขที่เอกสาร นี้มีอยู่แล้วในระบบ");
            }

            var chkResults = this.chk_Stock(model.ID, db);
            var chkOk = (bool)chkResults[0];
            var chkMsg = (string)chkResults[1];
            if (!chkOk)
            {
                return Json(chkMsg);
            }

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
                        string avbError = CheckAvailable(model); //check when status 3 , O
                        string snError = CheckSerialOut(model);
                        if (avbError == "Y" && snError == "Y")
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
                            return Json(avbError != "Y" ? avbError : "" + snError != "Y" ? snError : "");
                        }
                    }
                    else
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
                        try
                        {
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
                            var upd2 = (from d in context.Documents
                                       where d.ID == model.ID
                                       select d).ToList().SingleOrDefault();
                            upd2.Status = oldstatus;
                            context.SaveChanges();

                            return Json(ex.InnerException.Message);
                        }

                    }
                  

                }
            }
                       
           



            return Json("success");
        }


        //InsertandAppr Approve & Insert Stock
        [HttpPost]
        public ActionResult InsertandAppr(Document model)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                  
                    string avbError = CheckAvailable(model); //check when status 3 , O
                    string snError = CheckSerialOut(model);
                    if (avbError == "Y" && snError == "Y")
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

                            // Execute store procudure
                            var docid = model.ID;

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
                                dd.TrnType = "O";
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
                    else
                    {
                        return Json(avbError != "Y" ? avbError : "" + snError != "Y" ? snError : "");
                    }
                    
                }
            }
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
                    string avbError = CheckAvailable(model);
                    string snError = CheckSerialIn(model);
                    if (avbError == "Y" && snError == "Y")
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
                        return Json(avbError != "Y" ? avbError : "" + snError != "Y" ? snError : "");
                    }




                }
            }
                       
            return Json("success");
        }

        [HttpPost]
        public ActionResult ReceiveStock(Document model)
        {
            model.DocDate = model.DocDate.Value.AddYears(-543);
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
                dd.TrnType = "I";   //รับของเข้า stock
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();

            return Json("success");
        }

        //Reject 
        [HttpPost]
        public ActionResult RejectFromWh(Document model)
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

            //Reject 
        [HttpPost]
        public ActionResult Reject(Document model)
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

                            return Json(ex.InnerException.Message);
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
                Name =  "กำลังดำเนินการ";
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
            return Name ;
        }

        [HttpPost]
        public ActionResult GetWH()
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find staff in warehouse
                var SessionwhID = Session["UserWhID"].ToString();
                var SessionUserID = Session["UserID"].ToString();
                var wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
                //var wh = (from s in context.Staffs
                //          join st in context.StaffWarehouses on s.ID equals st.StID
                //          join w in context.Warehouses on st.WhID equals w.ID
                //          where s.ID.ToString() == SessionUserID
                //          select new
                //          {
                //              ID = st.WhID,
                //              Name = w.Name
                //          }).ToList();

                return Json(wh);
            }
        }

        // GET: Transfers/ReceiveTransfer/5
        public ActionResult ReceiveTransfer(int? id)
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

            if(document.DocNo.IndexOf("/R") <= -1)
            {
                document.DocNo = document.DocNo + "/R" ;
            }
            

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
            ViewBag.PdID = new SelectList(db.Products.ToList(), "ID", "Name");
            ViewBag.WhID = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.WarehouseAccess = Session["WarehouseAccess"];


            return View(document);
        }

        [HttpPost]
        public ActionResult ReceiveTransfer(Document model)
        {
            return View("Index");
        }


        // GET: Transfers/ReceiveBackTransfer/5    //รับของ
        public ActionResult ReceiveBackTransfer(int? id)
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
                        join st in context.Staffs on d.CreateBy equals st.ID
                        join p in context.Products on du.PdID equals p.ID
                        where du.DocID == document.ID && du.TrnType == "I" && d.SubType == 3
                        select new
                        {
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
            ViewBag.PdID = new SelectList(db.Products.ToList(), "ID", "Name");
            ViewBag.WhID = db.sp_WarehouseTransferAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            //ViewBag.WhID = db.Warehouses.ToList();

            return View(document);
        }

        [HttpPost]
        public ActionResult ReceiveBackTransfer(Document model)
        {
            //Update Status old data
            var upd = (from d in db.Documents
                       where d.ID == model.ID && d.SubType == 1
                       select d).ToList().SingleOrDefault();
            upd.Status = "5"; //รับของ
            upd.Remark = model.Remark;
            db.SaveChanges();

            //add new DocNo + "/R"
            model.DocDate = model.DocDate.Value.AddYears(-543);
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
                dd.TrnType = "I"; //รับของคืนเข้า stock
                db.DocumentDetails.Add(dd);
            }

            db.SaveChanges();


            

            return Json("success");
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
                    if (UserWhID.Where(s => s.WhID == Int32.Parse(whcode) /*&& (s.StID == 38)*/ && operation.Contains(s.IsManager)).Select(x => x).Count() >= 2)
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




        public static int DigitsOnly(string strRawData)
        {
            int number = 0;
            if (Int32.TryParse((Regex.Replace(strRawData, "[^0-9]", "")), out number))
            {

            }

            return number;
        }


    }
}
