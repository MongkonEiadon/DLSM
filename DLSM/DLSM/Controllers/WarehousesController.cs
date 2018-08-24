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
using System.IO;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class WarehousesController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Warehouses
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            List<Warehouse> w = new List<Warehouse>();
            var result = db.sp_WarehouseIndex(sessionstaffid).ToList();
            foreach(var i in result)
            {
                Warehouse warehouse = new Warehouse();
                warehouse.ID = i.ID;
                warehouse.Code = i.Code;
                warehouse.Name = i.Name;
                warehouse.TelNo = i.TelNo;
                warehouse.Email = i.Email;
                warehouse.IsMain = i.IsMain;
                w.Add(warehouse);
            }
            
            return View(w);
        }

        // GET: Warehouses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        public ActionResult SearchWarehouse(String SearchText)
        {
            var context = new DLSMEntities();
            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            List<Warehouse> w = new List<Warehouse>();
            var result = db.sp_WarehouseSearch(sessionstaffid, SearchText).ToList();
            foreach (var i in result)
            {
                Warehouse warehouse = new Warehouse();
                warehouse.ID = i.ID;
                warehouse.Code = i.Code;
                warehouse.Name = i.Name;
                warehouse.TelNo = i.TelNo;
                warehouse.Email = i.Email;
                warehouse.IsMain = i.IsMain;
                w.Add(warehouse);
            }

            return View("Index", w);
        }
        // GET: Warehouses/Create
        public ActionResult Create()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            var Province = (from p in db.Provinces
                            join w in db.Warehouses on p.ID equals w.PvID
                            join sf in db.StaffWarehouses on w.ID equals sf.WhID
                            where sf.StID == sessionstaffid
                            select p).Distinct().ToList();
            ViewBag.Province = new SelectList(Province, "ID","Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,Code,Name,NameEN,TelNo,Email,IsMain,LocationName,Latitude,Longitude,PvID")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                Warehouse w = context.Warehouses.Where(p => p.Code.Equals(warehouse.Code) || p.Name.Equals(warehouse.Name)).FirstOrDefault();

                if (w != null)
                {
                    return Json("รหัส หรือ ชื่อสำนักงานขนส่งนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Warehouses.Add(warehouse);
                    db.SaveChanges();

                    int WarehouseID = warehouse.ID;
                    return Json(WarehouseID); ;
                }
            }
            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Warehouse warehouse = db.Warehouses.Find(id);

            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }



            if (warehouse == null)
            {
                return HttpNotFound();
            }
            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            var Province = (from p in db.Provinces
                            join w in db.Warehouses on p.ID equals w.PvID
                            join sf in db.StaffWarehouses on w.ID equals sf.WhID
                            where sf.StID == sessionstaffid
                            select p).Distinct().ToList();
            ViewBag.Province = new SelectList(Province, "ID", "Name");
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,NameEN,TelNo,Email,IsMain,LocationName,Latitude,Longitude,BackgroundPicture,PvID")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                Warehouse w = context.Warehouses.Where(p => p.Name.Equals(warehouse.Name) && p.ID != warehouse.ID).FirstOrDefault();

                if (w != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    //TempData["Msg"] = "ชื่อคลังวัสดุอุปกรณ์นี้มีอยู่แล้วในระบบ";
                    //return RedirectToAction("Edit", new { id = warehouse.ID, ViewBag.Msg });
                    return Json("รหัส หรือ ชื่อสำนักงานขนส่งนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Entry(warehouse).State = EntityState.Modified;
                    db.SaveChanges();
                    int WarehouseID = warehouse.ID;
                    return Json(WarehouseID); 
                }

               
            }
            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            StaffWarehouse st = context.StaffWarehouses.Where(p => p.WhID == id).FirstOrDefault();
            Stock s = context.Stocks.Where(p => p.WhID == id).FirstOrDefault();

            if (st != null || s != null)
            {
                // Cannot delete becasue Topic group is using another process
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลนี้มีการถูกใช้งานอยู่";
                return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
            }
            else
            {
                Warehouse warehouse = db.Warehouses.Find(id);
                db.Warehouses.Remove(warehouse);
                db.SaveChanges();
                TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;

                int fileSizeInBytes = file.ContentLength;
                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                Byte[] bytes = target.ToArray();

                System.IO.Stream stream = new System.IO.MemoryStream();
                Int64 data64 = stream.Read(bytes, 0, (Int32)stream.Length);
                stream.Close();

                String img64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return Json(img64);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }


        [HttpPost]
        public JsonResult UploadBG(int WarehouseID)
        {
            try
            {
                HttpPostedFileBase file = Request.Files[0] as HttpPostedFileBase;

                int fileSizeInBytes = file.ContentLength;
                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                Byte[] bytes = target.ToArray();

                System.IO.Stream stream = new System.IO.MemoryStream();
                Int64 data64 = stream.Read(bytes, 0, (Int32)stream.Length);
                stream.Close();

                String img64 = Convert.ToBase64String(bytes, 0, bytes.Length);

                Warehouse warehouse = db.Warehouses.Find(WarehouseID);
                warehouse.BackgroundPicture = img64;
                warehouse.ImageUpdate = DateTime.Now;

                db.Entry(warehouse).State = EntityState.Modified;
                db.SaveChanges();

                var w = (from a in db.Warehouses
                         where a.PvID == warehouse.PvID
                         select a).ToList();
                foreach (var a in w)
                {
                    a.BackgroundPicture = img64;
                    a.ImageUpdate = DateTime.Now;
                    db.SaveChanges();
                }
                
                

                return Json("success");
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
