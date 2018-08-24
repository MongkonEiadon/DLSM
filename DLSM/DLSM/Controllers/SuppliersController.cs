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
    public class SuppliersController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Suppliers
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            return View(db.Suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        public ActionResult SearchSuppliers(String SearchText)
        {
            var context = new DLSMEntities();
            List<Supplier> list = (from s in context.Suppliers
                                   where s.Name.Contains(SearchText) select new {
                                        ID = s.ID,
                                        Name = s.Name }).AsEnumerable().Select(x => new Supplier {
                                        ID = x.ID,
                                        Name = x.Name}).ToList();

            return PartialView("SupplierSearch",list);
        }
        // GET: Suppliers/Create
        public ActionResult Create()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                Supplier c = context.Suppliers.Where(p => p.Name == supplier.Name).FirstOrDefault();

                if(c != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    //TempData["Msg"] = "ชื่อผู้จำหน่ายนี้มีอยู่แล้วในระบบ";
                    //return RedirectToAction("Create", new { ViewBag.Msg });
                    return Json("ชื่อผู้ประกอบการนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Suppliers.Add(supplier);
                    db.SaveChanges();
                   
                }
                return Json("success");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                Supplier c = context.Suppliers.Where(p => p.Name == supplier.Name && p.ID != supplier.ID).FirstOrDefault();

                if (c != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    //TempData["Msg"] = "ชื่อผู้จำหน่ายนี้มีอยู่แล้วในระบบ";
                    //return RedirectToAction("Edit", new { id = supplier.ID, ViewBag.Msg });
                    return Json("ชื่อผู้ประกอบการนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Entry(supplier).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json("success");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            Document d = context.Documents.Where(p => p.SpID == id).FirstOrDefault();


            if (d != null)
            {
                // Cannot delete becasue Topic group is using another process
                //TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่";
                //return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
                return Json("ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่");
            }
            else
            {
                Supplier supplier = db.Suppliers.Find(id);
                db.Suppliers.Remove(supplier);
                db.SaveChanges();
                TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";
            }
           
            return RedirectToAction("Index");
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
