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
    public class ProductsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Products
        public ActionResult Index()
        {
            //var products = db.Products.Include(p => p.Category);
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            var products = (from p in db.Products
                            join c in db.Categories on p.CtID equals c.ID
                            select new
                            {
                                ID = p.ID,
                                Code = p.Code,
                                Name = p.Name,
                                CtID = c.ID,
                                CategoryName = c.Name,
                                MinStock = p.MinStock,
                                SerialControl = p.SerialControl,
                                IsAsset = p.IsAsset

                            }).AsEnumerable().Select(x => new Product
                            {
                                ID = x.ID,
                                Code = x.Code,
                                Name = x.Name,
                                CtID = x.CtID,
                                CategoryName = x.CategoryName,
                                MinStock = x.MinStock,
                                SerialControl = x.SerialControl,
                                IsAsset = x.IsAsset
                            }).ToList();


            List<Category> catelist = db.Categories.ToList();
            ViewBag.CateList = new SelectList(catelist, "ID", "Name");

            
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult SearchProduct(string SearchText,int? SearchCate)
        {
            var context = new DLSMEntities();
            var products = (from p in context.Products
                        join c in context.Categories on p.CtID equals c.ID
                        where (p.Name.Contains(SearchText) || SearchText == null) && 
                                (c.ID == SearchCate.Value || SearchCate == null)
                        select new
                                   {
                                        ID = p.ID,
                                       Code = p.Code,
                                       Name = p.Name,
                                       CtID = c.ID,
                                       CategoryName = c.Name,
                                       MinStock = p.MinStock,
                                       SerialControl = p.SerialControl,
                                       IsAsset = p.IsAsset

                                   }).AsEnumerable().Select(x => new Product
                                   {
                                       ID = x.ID,
                                       Code = x.Code,
                                       Name = x.Name,
                                       CtID = x.CtID,
                                       CategoryName = x.CategoryName,
                                       MinStock = x.MinStock,
                                       SerialControl = x.SerialControl,
                                       IsAsset = x.IsAsset
                                   }).ToList();

            List<Category> catelist = db.Categories.ToList();
            ViewBag.CateList = new SelectList(catelist, "ID", "Name");

            //return PartialView("ProductSearch",list);
            return View("Index", products);
        }
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CtID = new SelectList(db.Categories, "ID", "Name");

            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Product product)
        {
         
                var context = new DLSMEntities();
                Product pp = context.Products.Where(p => p.Code == product.Code ||  p.Name == product.Name).FirstOrDefault();

                if (pp != null)
                {
                    return Json("รหัส หรือ ชื่อวัสดุนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    //bool check = product.SerialControl == "on" ? true : false;
                    bool check = product.SerialControl.ToLower() == "true" ? true : false;
                    if (check == true)
                    {
                        product.SerialControl = "Y";
                    }
                    else
                    {
                        product.SerialControl = "N";
                    }
                    bool check2 = product.IsAsset.ToLower() == "true" ? true : false;
                    if (check2 == true)
                    {
                        product.IsAsset = "Y";
                    }
                    else
                    {
                        product.IsAsset = "N";
                    }
                    product.MinStock = 0;
                    db.Products.Add(product);
                    db.SaveChanges();
                    return Json("success");
            }         
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            if(product.SerialControl == "Y")
            {
                product.SerialControl = "true";
            }
            else
            {
                product.SerialControl = "false";
            }

            if (product.IsAsset == "Y")
            {
                product.IsAsset = "true";
            }
            else
            {
                product.IsAsset = "false";
            }



            ViewBag.CtID = new SelectList(db.Categories, "ID", "Name");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Product product)
        {
                var context = new DLSMEntities();
                Product pp = context.Products.Where(p => p.Name == product.Name && p.ID != product.ID).FirstOrDefault();

                if (pp != null)
                {
                return Json("ชื่อวัสดุนี้มีอยู่แล้วในระบบ");
                }
                else
                {
                    bool check = product.SerialControl.ToLower() == "true" ? true : false;
                    if (check == true)
                    {
                        product.SerialControl = "Y";
                    }
                    else
                    {
                        product.SerialControl = "N";
                    }

                    bool check2 = product.IsAsset.ToLower() == "true" ? true : false;
                    if (check2 == true)
                    {
                        product.IsAsset = "Y";
                    }
                    else
                    {
                        product.IsAsset = "N";
                    }
                    product.MinStock = 0;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json("success");
                }

        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            DocumentDetail st = context.DocumentDetails.Where(p => p.PdID == id).FirstOrDefault();

            if (st != null)
            {
                // Cannot delete becasue Topic group is using another process
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลนี้มีการถูกใช้งานอยู่";
                return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
            }
            else
            {
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
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
