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
    public class TransferStaffsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: TransferStaffs
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            var context = new DLSMEntities();
            var list = (from ts in context.TransferStaffs
                        join st in context.Staffs on ts.StID equals st.ID
                        join w in context.Warehouses on ts.FromWhID equals w.ID
                        join wh in context.Warehouses on ts.ToWhID equals wh.ID
                        select new {
                            ID = ts.ID,
                            StID = ts.StID,
                            StName = st.Name,
                            FromWhID = ts.FromWhID,
                            FromWhName = w.Name,
                            ToWhID = ts.ToWhID,
                            ToWhName = wh.Name,
                            EffectiveDate = ts.EffectiveDate
                        }).AsEnumerable().Select (x => new TransferStaff {
                            ID = x.ID,
                            StID = x.StID,
                            StName = x.StName,
                            FromWhID = x.FromWhID,
                            FromWhName = x.FromWhName,
                            ToWhID = x.ToWhID,
                            ToWhName = x.ToWhName,
                            EffectiveDate = (x.EffectiveDate.Value.Year < 2400 ? x.EffectiveDate.Value.AddYears(543) : x.EffectiveDate),
                        }).ToList();

            ViewBag.StID = db.Staffs.ToList();
            ViewBag.WhID = db.Warehouses.ToList();
            
            return View(list);
        }

        [HttpGet]
        public ActionResult Search(int searchwh,int searchst)
        {
            var context = new DLSMEntities();
            var list = (from ts in context.TransferStaffs
                        join st in context.Staffs on ts.StID equals st.ID
                        join w in context.Warehouses on ts.FromWhID equals w.ID
                        join wh in context.Warehouses on ts.ToWhID equals wh.ID
                        where (ts.FromWhID == searchwh || ts.ToWhID == searchwh) && ts.StID == searchst
                        select new
                        {
                            ID = ts.ID,
                            StID = ts.StID,
                            StName = st.Name,
                            FromWhID = ts.FromWhID,
                            FromWhName = w.Name,
                            ToWhID = ts.ToWhID,
                            ToWhName = wh.Name,
                            EffectiveDate = ts.EffectiveDate
                        }).AsEnumerable().Select(x => new TransferStaff
                        {
                            ID = x.ID,
                            StID = x.StID,
                            StName = x.StName,
                            FromWhID = x.FromWhID,
                            FromWhName = x.FromWhName,
                            ToWhID = x.ToWhID,
                            ToWhName = x.ToWhName,
                            EffectiveDate = x.EffectiveDate.Value.AddYears(543)
                        }).ToList();

            return PartialView("TransferStaffSearch",list);
        }
        // GET: TransferStaffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferStaff transferStaff = db.TransferStaffs.Find(id);
            if (transferStaff == null)
            {
                return HttpNotFound();
            }
            return View(transferStaff);
        }

        // GET: TransferStaffs/Create
        public ActionResult Create()
        {
            ViewBag.StID = db.Staffs.ToList();
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.ToWhID = db.Warehouses.ToList();
            return View();
        }

        // POST: TransferStaffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(TransferStaff transferStaff)
        {

            transferStaff.IsActive = 1;
            db.TransferStaffs.Add(transferStaff);
            db.SaveChanges();
            return Json("success");
            //return RedirectToAction("Index");
            

            //ViewBag.StID = db.Staffs.ToList();
            //ViewBag.WhID = db.Warehouses.ToList();
            //ViewBag.ToWhID = db.Warehouses.ToList();
            //return View(transferStaff);
        }

        // GET: TransferStaffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferStaff transferStaff = db.TransferStaffs.Find(id);
            if (transferStaff == null)
            {
                return HttpNotFound();
            }
            var context = new DLSMEntities();
            var frwh = context.Warehouses.SingleOrDefault(u => u.ID == transferStaff.FromWhID);
            transferStaff.FromWhName = frwh.Name;

            var twh = context.Warehouses.SingleOrDefault(u => u.ID == transferStaff.ToWhID);
            transferStaff.ToWhName = twh.Name;

            var st = context.Staffs.SingleOrDefault(u => u.ID == transferStaff.StID);
            transferStaff.StName = st.Name;

            var lst = new[]
            {
                new { ID = 1, Name = "ผู้จัดการ" },
                new { ID = 2, Name = "เจ้าหน้าที่" },
            };

            transferStaff.EffectiveDate = transferStaff.EffectiveDate.Value.AddYears(543);


            //List<SelectListItem> ls = new List<SelectListItem>();
            //ls.Add(new SelectListItem() { Value = "1", Text = "ผู้จัดการ", Selected = (transferStaff.IsManager == "1" ? true : false) });
            //ls.Add(new SelectListItem() { Value = "2", Text = "เจ้าหน้าที่", Selected = (transferStaff.IsManager == "2" ? true : false) });


            ViewBag.IsManager = new SelectList(lst.ToList(), "ID", "Name", transferStaff.IsManager);

            //ViewBag.ListIsManager = ls;
            ViewBag.StID = db.Staffs.ToList();
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.ToWhID = db.Warehouses.ToList();
            return View(transferStaff);
        }

        // POST: TransferStaffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]       
        public ActionResult Edit(TransferStaff transferStaff)
        {
            if (ModelState.IsValid)
            {
                transferStaff.IsActive = 1;
                db.Entry(transferStaff).State = EntityState.Modified;
                db.SaveChanges();
                return Json("success");
            }

            var lst = new[]
           {
                new { Id = 1, Name = "ผู้จัดการ" },
                new { Id = 2, Name = "เจ้าหน้าที่" },
            };

            transferStaff.EffectiveDate = transferStaff.EffectiveDate.Value.AddYears(543);

            ViewBag.IsManager = new SelectList(lst, "Id", "Name");
            ViewBag.StID = db.Staffs.ToList();
            ViewBag.WhID = db.Warehouses.ToList();
            ViewBag.ToWhID = db.Warehouses.ToList();
            return View(transferStaff);
        }

        // GET: TransferStaffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferStaff transferStaff = db.TransferStaffs.Find(id);
            if (transferStaff == null)
            {
                return HttpNotFound();
            }
            return View(transferStaff);
        }

        // POST: TransferStaffs/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            //TransferStaff transferStaff = db.TransferStaffs.Find(id);

            //transferStaff.IsActive = 0;
            //db.Entry(transferStaff).State = EntityState.Modified;
            //db.SaveChanges();

            TransferStaff transferStaff = db.TransferStaffs.Find(id);
            db.TransferStaffs.Remove(transferStaff);
            db.SaveChanges();
            TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetWH(int StaffID)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find staff in warehouse
                var wh = (from s in context.Staffs
                          join st in context.StaffWarehouses on s.ID equals st.StID
                          join w in context.Warehouses on st.WhID equals w.ID
                          where s.ID == StaffID
                          select new
                          {
                              ID = st.WhID,
                              Name = w.Name
                          }).ToList();

                return Json(wh);
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
