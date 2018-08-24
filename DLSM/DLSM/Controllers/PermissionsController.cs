using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLSM.Models;

namespace DLSM.Controllers
{
    public class PermissionsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Permissions
        public ActionResult Index()
        {
            var context = new DLSMEntities();
            var list = (from u in context.UserGroups
                            //from p in context.Permissions
                            //join m in context.Modules on p.MdCode equals m.Code
                            //join u in context.UserGroups on p.UgID equals u.ID
                        select new {
                            UserGroupID = u.ID,
                            UserGroupName = u.Name
                        }).AsEnumerable().Select(x => new Permission {
                            UserGroupID = x.UserGroupID,
                            UserGroupName = x.UserGroupName
                        }).ToList();

            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            return View(list);
        }

        public ActionResult Search(String SearchText)
        {
            var context = new DLSMEntities();
            var list = (from p in context.Permissions
                        join m in context.Modules on p.MdCode equals m.Code
                        join u in context.UserGroups on p.UgID equals u.ID
                        where u.Name.Contains(SearchText)
                        select new
                        {
                            UserGroupID = u.ID,
                            UserGroupName = u.Name
                        }).AsEnumerable().Select(x => new Permission
                        {
                            UserGroupID = x.UserGroupID,
                            UserGroupName = x.UserGroupName
                        }).ToList();
            return PartialView("PermissionSearch", list);
        }

        // GET: Permissions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // GET: Permissions/Create
        public ActionResult Create()
        {
            var mainmenu = (from m in db.Modules
                            where m.ParentMenu == m.Name
                            select m);

            var submenu = (from m in db.Modules
                            where m.ParentMenu != m.Name
                            select m);

            ViewBag.MainMenu = mainmenu;
            ViewBag.SubMenu = submenu;

            //ViewBag.MainMenu = new SelectList(mainmenu, "Name", "ParentMenu");
            //ViewBag.SubMenu = new SelectList(submenu, "Name", "ParentMenu");
            ViewBag.UgID = new SelectList(db.UserGroups, "ID", "Name");
            return View();
        }

        // POST: Permissions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //public ActionResult Create(string Menu,string Parent,string UserGroupName)
        public ActionResult Create(Permission p)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    UserGroup ug = new UserGroup();
                    ug.Name = p.UserGroupName;
                    db.UserGroups.Add(ug);
                    db.SaveChanges();

                    int UgID = ug.ID;


                    if(p.Menu != null && p.Menu.Count() > 0)
                    {
                        foreach (var i in p.Menu)
                        {

                            var mod = db.Modules.SingleOrDefault(x => x.Name == i.ModuleName);
                            Permission per = new Permission();
                            per.MdCode = mod.Code;
                            per.UgID = UgID;
                            per.Active = 1;
                            db.Permissions.Add(per);
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    return Json("success");
                }
                catch(Exception ex)
                {
                    dbContextTransaction.Rollback();

                    return Json("failed");
                }
            }
        }

        // GET: Permissions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            var permission = db.Permissions.Where(o => o.UgID == id).ToList();
            Permission p = new Permission();
            var ug = db.UserGroups.SingleOrDefault(x => x.ID == id);
            p.UserGroupName = ug.Name;
            p.UserGroupID = ug.ID;
            
            var mainmenu = (from m in db.Modules
                                join x in db.Permissions on new { Code = m.Code, UgID = id.Value } equals new { Code = x.MdCode, UgID = x.UgID } into xx
                                from md in xx.DefaultIfEmpty()
                                where m.ParentMenu == m.Name
                                select new
                                {
                                    Code = m.Code,
                                    Name = m.Name,
                                    ParentMenu = m.ParentMenu,
                                    NameTH = m.NameTH,
                                    IsCheck = md.MdCode
                                }).AsEnumerable().Select(x => new Module
                                {
                                    Code = x.Code,
                                    Name = x.Name,
                                    ParentMenu = x.ParentMenu,
                                    NameTH = x.NameTH,
                                    isChecked = x.IsCheck != null ? true : false

                                }).ToList();
            
            var submenu = (from m in db.Modules
                            join x in db.Permissions on new { Code = m.Code, UgID = id.Value } equals new { Code = x.MdCode, UgID = x.UgID } into xx
                            from md in xx.DefaultIfEmpty()
                            where m.ParentMenu != m.Name
                            select new
                            {
                                Code = m.Code,
                                Name = m.Name,
                                ParentMenu = m.ParentMenu,
                                NameTH = m.NameTH,
                                IsCheck = md.MdCode
                            }).AsEnumerable().Select(x => new Module
                            {
                                Code = x.Code,
                                Name = x.Name,
                                ParentMenu = x.ParentMenu,
                                NameTH = x.NameTH,
                                isChecked = x.IsCheck != null ? true : false

                            }).ToList();

            ViewBag.MainMenu = mainmenu;
            ViewBag.SubMenu = submenu;
            
            ViewBag.UgID = new SelectList(db.UserGroups, "ID", "Name");

            return View(p);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Permission p)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    UserGroup ug = new UserGroup();
                    ug.ID = p.UserGroupID;
                    ug.Name = p.UserGroupName;
                    db.Entry(ug).State = EntityState.Modified;
                    db.SaveChanges();

                    int UgID = ug.ID;

                    db.Permissions.RemoveRange(db.Permissions.Where(x => x.UgID == p.UserGroupID));
                    db.SaveChanges();

                    if (p.Menu != null && p.Menu.Count() > 0)
                    {
                        foreach (var i in p.Menu)
                        {

                            var mod = db.Modules.SingleOrDefault(x => x.Name == i.ModuleName);
                            Permission per = new Permission();
                            per.MdCode = mod.Code;
                            per.UgID = UgID;
                            per.Active = 1;
                            db.Permissions.Add(per);
                            db.SaveChanges();
                            
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    return Json("success");
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    return Json("failed");
                }
            }
        }

        // GET: Permissions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: Permissions/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            Staff st = context.Staffs.Where(p => p.UgID == id).FirstOrDefault();

            if (st != null)
            {
                // Cannot delete becasue Topic group is using another process
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลนี้มีการถูกใช้งานอยู่";
                return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
            }
            else
            {
                db.Permissions.RemoveRange(db.Permissions.Where(x => x.UgID == id));
                db.SaveChanges();

                db.UserGroups.Remove(db.UserGroups.SingleOrDefault(x=> x.ID == id));
                db.SaveChanges();
                TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";
                return RedirectToAction("Index", new { ViewBag.Msg  });
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
