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
    public class CategoriesController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Categories
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Category category)
        {
                var context = new DLSMEntities();
                Category c = context.Categories.Where(p => p.Code == category.Code || p.Name == category.Name).FirstOrDefault();

                if (c != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    //TempData["Msg"] = "รหัส หรือ ชื่อกลุ่มวัสดุอุปกรณ์นี้มีอยู่แล้วในระบบ";
                    return Json("รหัส หรือ ชื่อกลุ่มวัสดุนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return Json("success");
                }
               
                //return RedirectToAction("Index"); 
        }

        public ActionResult SearchCategory(string SearchText)
        {
            var context = new DLSMEntities();
            List<Category> category = (from c in context.Categories where c.Name.Contains(SearchText) select new
                                    {
                                        ID = c.ID,
                                        Code = c.Code,
                                        Name = c.Name
                                    }).AsEnumerable().Select(x => new Category{
                                        ID = x.ID,
                                        Code = x.Code,
                                        Name = x.Name
                                    }).ToList();

            return View("Index", category);
            //return PartialView("CategorySearch",list);
        }
        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Category category)
        {  
                var context = new DLSMEntities();
                Category c = context.Categories.Where(p => p.Name == category.Name && p.ID != category.ID).FirstOrDefault();

                if (c != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    //TempData["Msg"] = "ชื่อกลุ่มวัสดุอุปกรณ์นี้มีอยู่แล้วในระบบ";
                    //return RedirectToAction("Edit", new { id = category.ID, ViewBag.Msg });
                    return Json("ชื่อกลุ่มวัสดุนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json("success");
                }

        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            Product pp = context.Products.Where(p => p.CtID == id).FirstOrDefault();


            if (pp != null)
            {
                // Cannot delete becasue Topic group is using another process
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่";
                return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
            }
            else
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
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
