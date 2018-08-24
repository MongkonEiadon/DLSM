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

namespace DLSM.Controllers
{
    [SessionCheck]
    public class StocksController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Stocks
        public ActionResult Index()
        {
            var context = new DLSMEntities();
            var UserWhID = Convert.ToInt32(Session["UserWhID"]);
            var UserStID = Convert.ToInt32(Session["UserID"]);
            List<Stock> listStock = new List<Stock>();
            

            var list = context.sp_Stock(UserWhID, UserStID).ToList();

            if(list.Count() > 0)
            {
                foreach(var e in list)
                {
                    Stock sk = new Stock();
                    sk.WhID = e.WhID;
                    sk.PdID = e.PdID;
                    sk.ProductCode = e.ProductCode;
                    sk.ProductName = e.ProductCode + "-" + e.ProductName;
                    sk.Qty = e.Qty;
                    sk.Transfer = e.Transfer;
                    sk.Borrow = e.Borrow;
                    sk.CheckStatus = e.CheckStatus;
                    sk.MinStock = e.MinStock;
                    sk.Predict = Convert.ToDecimal(e.Predict);
                    listStock.Add(sk);
                }                
            }
            //var list = (from p in context.Products
            //            join s in context.Stocks on p.ID equals s.PdID into ss
            //            from d in ss.DefaultIfEmpty()
            //            join w in context.Warehouses on d.WhID equals w.ID
            //            join wm in context.WarehouseMinimums on new { d.PdID, d.WhID } equals new { wm.PdID, wm.WhID } into wmm
            //            from wmi in wmm.DefaultIfEmpty()
            //            where (d.WhID.ToString() == UserWhID) || (d.PdID == 0 || d.WhID == 0)                        
            //            select new
            //            {
            //                WhID = d.WhID,
            //                PdID = d.PdID,
            //                ProductCode = p.Code,
            //                ProductName = p.Name,
            //                Qty = d == null ? 0 : d.Qty,
            //                Transfer = d == null ? 0 :d.Transfer,
            //                Borrow = d == null ? 0 : d.Borrow,
            //                CheckStatus = d == null ? "N" : d.CheckStatus,
            //                MinStock = wmi == null ? p.MinStock : wmi.MinStock
            //            }).AsEnumerable().Select(x => new Stock
            //            {
            //                WhID = x.WhID,
            //                PdID = x.PdID,
            //                ProductCode = x.ProductCode,
            //                ProductName = x.ProductName,
            //                Qty = x.Qty,
            //                Borrow = x.Borrow,
            //                Transfer = x.Transfer,
            //                CheckStatus = x.CheckStatus,
            //                MinStock = x.MinStock
            //            }).ToList();



            //var a = (from p in context.Products
            //        join w in context.WarehouseMinimums on new { a = p.ID, b = UserWhID } equals new { a = w.PdID, b = w.WhID.ToString() } into wm
            //        from wmi in wm.DefaultIfEmpty()
            //        where !context.Stocks.Any(x => x.PdID == p.ID && x.WhID.ToString() == UserWhID) //&& wmi.WhID.ToString() == UserWhID
            //        select new
            //        {
            //            PdID = p.ID,
            //            ProductCode = p.Code,
            //            ProductName = p.Name,
            //            MinStock = wmi == null ? p.MinStock : wmi.MinStock
            //        }).AsEnumerable().Select(x=>new Stock {
            //             WhID = 0,
            //             PdID = x.PdID,
            //             ProductCode = x.ProductCode,
            //             ProductName = x.ProductName,
            //             Qty = 0,
            //             Borrow = 0,
            //             Transfer = 0,
            //             CheckStatus = "N",
            //             MinStock = x.MinStock
            //         }).ToList();
         
            //var listfinal = list.Union(a).OrderBy(x=> x.ProductCode);


            //foreach (var i in listfinal)
            //{
            //    i.ProductName = i.ProductCode + "-" + i.ProductName;
            //}

            ViewBag.WarehouseList = db.Warehouses.ToList();
            ViewBag.ProductList = db.Products.ToList();
            ViewBag.CategoryList = new SelectList(db.Categories, "ID", "Name");
            
            return View(listStock);
        }

        public ActionResult Search(int searchWare, int searchPro,int searchCat)
        {
            var context = new DLSMEntities();
            var UserStID = Convert.ToInt32(Session["UserID"]);
            List<Stock> listStock = new List<Stock>();

            var list = context.sp_SearchStock(searchWare, UserStID, searchPro, searchCat).ToList();

            if (list.Count() > 0)
            {
                foreach (var e in list)
                {
                    Stock sk = new Stock();
                    sk.WhID = e.WhID;
                    sk.PdID = e.PdID;
                    sk.ProductCode = e.ProductCode;
                    sk.ProductName = e.ProductCode + "-" + e.ProductName;
                    sk.Qty = e.Qty;
                    sk.Transfer = e.Transfer;
                    sk.Borrow = e.Borrow;
                    sk.CheckStatus = e.CheckStatus;
                    sk.MinStock = e.MinStock;
                    sk.Predict = Convert.ToDecimal(e.Predict);
                    listStock.Add(sk);
                }
            }
            // Product in Stock, Warehouses and not/in WarehouseMinimums
            //var a = (from p in context.Products
            //            join s in context.Stocks on p.ID equals s.PdID into ss
            //            from d in ss.DefaultIfEmpty()
            //            join w in context.Warehouses on d.WhID equals w.ID
            //            join wm in context.WarehouseMinimums on new { d.PdID, d.WhID } equals new { wm.PdID, wm.WhID } into wmm
            //            from wmi in wmm.DefaultIfEmpty()
            //            where (d.WhID == searchWare || searchWare == 0) && (d.PdID == searchPro || searchPro == 0)
            //            && (p.CtID == searchCat || searchCat == 0) 
            //            select new
            //            {
            //                WhID = d.WhID,
            //                PdID = d.PdID,
            //                ProductCode = p.Code,
            //                ProductName = p.Name,
            //                Qty = d == null ? 0 : d.Qty,
            //                Transfer = d == null ? 0 : d.Transfer,
            //                Borrow = d == null ? 0 : d.Borrow,
            //                CheckStatus = d == null ? "N" : d.CheckStatus,
            //                MinStock = wmi == null ? p.MinStock : wmi.MinStock
            //            }).AsEnumerable().Select(x => new Stock
            //            {
            //                WhID = x.WhID,
            //                PdID = x.PdID,
            //                ProductCode = x.ProductCode,
            //                ProductName = x.ProductName,
            //                Qty = x.Qty,
            //                Borrow = x.Borrow,
            //                Transfer = x.Transfer,
            //                CheckStatus = x.CheckStatus,
            //                MinStock = x.MinStock
            //            }).ToList();



            //// Product not in Stock , not/in WarehouseMinimums
            //var b = (from p in context.Products
            //         join w in context.WarehouseMinimums on new { a = p.ID, b = searchWare } equals new { a = w.PdID, b = w.WhID } into wm
            //         from wmi in wm.DefaultIfEmpty()
            //         where !context.Stocks.Any(x => x.PdID == p.ID && x.WhID == searchWare)  && (p.CtID == searchCat || searchCat == 0)
            //         select new
            //         {
            //             PdID = p.ID,
            //             ProductCode = p.Code,
            //             ProductName = p.Name,
            //             MinStock = wmi == null ? p.MinStock : wmi.MinStock
            //         }).AsEnumerable().Select(x => new Stock
            //         {
            //             WhID = 0,
            //             PdID = x.PdID,
            //             ProductCode = x.ProductCode,
            //             ProductName = x.ProductName,
            //             Qty = 0,
            //             Borrow = 0,
            //             Transfer = 0,
            //             CheckStatus = "N",
            //             MinStock = x.MinStock
            //         }).ToList();




            //var listfinal = a.Union(b).OrderBy(x => x.ProductCode);



            //foreach (var i in listfinal)
            //{
            //    i.ProductName = i.ProductCode + "-" + i.ProductName;
            //}

            //ViewBag.WarehouseList = db.Warehouses.ToList();
            //ViewBag.ProductList = db.Products.ToList();
            //ViewBag.CategoryList = new SelectList(db.Categories, "ID", "Name");

            //return View("Index", listfinal);


            return PartialView("StockSearch", listStock);
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int? pdid,int? whid)
        {
            if (pdid == null && whid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var context = new DLSMEntities();
            var list = (from s in context.Stocks
                        join sr in context.StockSerials on new { PdID = s.PdID , WhID = s.WhID } equals new { PdID = sr.PdID.Value, WhID = sr.WhID.Value }
                        join p in context.Products on s.PdID equals p.ID
                        join w in context.Warehouses on s.WhID equals w.ID
                        where sr.PdID == pdid.Value && sr.WhID == whid.Value && (sr.SerialBegin != "0" && sr.SerialEnd != "0")
                        orderby sr.SerialBegin
                        select new {
                            Qty = sr.SerialCount,
                            PdID = p.ID,
                            ProductName = p.Name,
                            WhID = w.ID,
                            WhName = w.Name,
                            SerialBegin = sr.SerialBegin,
                            SerialEnd = sr.SerialEnd,
                            IpProperty = sr.IpProperty,
                            IsAsset = p.IsAsset
                        }).AsEnumerable().Select(x => new Stock {
                            Qty = x.Qty,
                            PdID = x.PdID,
                            ProductName = x.ProductName,
                            WhID = x.WhID,
                            WhName = x.WhName,
                            SerialBegin = x.SerialBegin,
                            SerialEnd = x.SerialEnd,
                            IpProperty = x.IpProperty,
                            IsAsset = x.IsAsset
                        }).ToList();

            int total = Int32.Parse(list.Select(x => x.Qty).Sum().ToString());
            ViewData["TotalQty"] = total.ToString("#,##0");

            var wh = context.Warehouses.SingleOrDefault(x => x.ID == whid);
            ViewData["WhName"] = wh.Name;

            var pp = context.Products.SingleOrDefault(x => x.ID == pdid);
            ViewData["ProductName"] = pp.Name;
            return View(list);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            ViewBag.PdID = new SelectList(db.Products, "ID", "Code");
            ViewBag.WhID = new SelectList(db.Warehouses, "ID", "Code");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WhID,PdID,Qty,Borrow,Transfer,CheckStatus")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PdID = new SelectList(db.Products, "ID", "Code", stock.PdID);
            ViewBag.WhID = new SelectList(db.Warehouses, "ID", "Code", stock.WhID);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            ViewBag.PdID = new SelectList(db.Products, "ID", "Code", stock.PdID);
            ViewBag.WhID = new SelectList(db.Warehouses, "ID", "Code", stock.WhID);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WhID,PdID,Qty,Borrow,Transfer,CheckStatus")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PdID = new SelectList(db.Products, "ID", "Code", stock.PdID);
            ViewBag.WhID = new SelectList(db.Warehouses, "ID", "Code", stock.WhID);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
            db.SaveChanges();
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
                var wh = db.sp_WarehouseAuthority(Convert.ToInt32(SessionUserID)).ToList();
                //var wh = (from s in context.Staffs
                //             join st in context.StaffWarehouses on s.ID equals st.StID
                //             join w in context.Warehouses on st.WhID equals w.ID
                //             where s.ID.ToString() == SessionUserID 
                //             select new
                //             {
                //                 ID = st.WhID,
                //                 Name = w.Name
                //             }).ToList();

                return Json(wh);
            }           
        }


        [HttpGet]
        public ActionResult InsertMinStock(int whid, int pdid, int minstock, int predict)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var wh = context.WarehouseMinimums.SingleOrDefault(x => x.WhID == whid && x.PdID == pdid);
                        if(wh == null)
                        { //insert
                            WarehouseMinimum min = new WarehouseMinimum();
                            min.WhID = whid;
                            min.PdID = pdid;
                            min.MinStock = minstock;
                            min.PredictMonth = (predict == 0 ? min.PredictMonth : predict);
                            context.WarehouseMinimums.Add(min);
                            context.SaveChanges();
                        }
                        else
                        {
                            var mm = context.WarehouseMinimums.Where(o => o.PdID == pdid && o.WhID == whid).FirstOrDefault();
                            WarehouseMinimum min = context.WarehouseMinimums.Find(mm.ID);
                            min.WhID = whid;
                            min.PdID = pdid;
                            min.MinStock = minstock;
                            min.PredictMonth = (predict == 0 ? min.PredictMonth : (int?)predict);
                            context.Entry(min).State = EntityState.Modified;
                            context.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json("fail", JsonRequestBehavior.AllowGet);
                    }
                    
                }
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
