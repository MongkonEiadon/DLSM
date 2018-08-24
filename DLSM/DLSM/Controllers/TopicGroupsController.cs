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
    public class TopicGroupsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: TopicGroups
        public ActionResult Index()
        {
            var context = new DLSMEntities();
            List<TopicGroup> list = (from t in context.TopicGroups
                                     select new
                                     {
                                         ID = t.ID,
                                         Name = t.Name
                                     }).AsEnumerable().Select(x => new TopicGroup
                                     {
                                         ID = x.ID,
                                         Name = x.Name
                                     }).ToList();
            return View(list);
        }

        // GET: TopicGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TopicGroup topicGroup = db.TopicGroups.Find(id);
            if (topicGroup == null)
            {
                return HttpNotFound();
            }
            return View(topicGroup);
        }

        public ActionResult SearchTopicGroup(String SearchText)
        {
            var context = new DLSMEntities();
            List<TopicGroup> list = (from t in context.TopicGroups
                                   where t.Name.Contains(SearchText)
                                   select new
                                   {
                                       ID = t.ID,
                                       Name = t.Name
                                   }).AsEnumerable().Select(x => new TopicGroup
                                   {
                                       ID = x.ID,
                                       Name = x.Name
                                   }).ToList();
            return PartialView("TopicGroupsSearch",list);
        }
        // GET: TopicGroups/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject");
            ViewBag.ID = new SelectList(db.Topics, "ID", "Subject");

            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            return View();
        }

        // POST: TopicGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] TopicGroup topicGroup)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                TopicGroup t = context.TopicGroups.Where(p => p.Name == topicGroup.Name).FirstOrDefault();

                if (t != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    TempData["Msg"] = "ชื่อกลุ่มปัญหานี้มีอยู่แล้วในระบบ";
                    return RedirectToAction("Create", new { ViewBag.Msg });
                }
                else
                {
                    db.TopicGroups.Add(topicGroup);
                    db.SaveChanges();
                }
                
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topicGroup.ID);
            ViewBag.ID = new SelectList(db.Topics, "ID", "Subject", topicGroup.ID);
            return View(topicGroup);
        }

        // GET: TopicGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TopicGroup topicGroup = db.TopicGroups.Find(id);
            if (topicGroup == null)
            {
                return HttpNotFound();
            }

            if(null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            

            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topicGroup.ID);
            ViewBag.ID = new SelectList(db.Topics, "ID", "Subject", topicGroup.ID);
            return View(topicGroup);
        }

        // POST: TopicGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] TopicGroup topicGroup)
        {
            if (ModelState.IsValid)
            {
                var context = new DLSMEntities();
                TopicGroup t = context.TopicGroups.Where(p => p.Name == topicGroup.Name && p.ID != topicGroup.ID).FirstOrDefault();

                if (t != null)
                {
                    // Cannot delete becasue Topic group is using another process
                    TempData["Msg"] = "ชื่อกลุ่มปัญหานี้มีอยู่แล้วในระบบ";
                    return RedirectToAction("Edit", new { id = topicGroup.ID, ViewBag.Msg });
                }
                else
                {
                    db.Entry(topicGroup).State = EntityState.Modified;
                    db.SaveChanges();
                }

              
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topicGroup.ID);
            ViewBag.ID = new SelectList(db.Topics, "ID", "Subject", topicGroup.ID);
            return View(topicGroup);
        }

        // GET: TopicGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TopicGroup topicGroup = db.TopicGroups.Find(id);
            if (topicGroup == null)
            {
                return HttpNotFound();
            }
            return View(topicGroup);
        }

        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            Topic topic = context.Topics.Where(p => p.TgID == id).FirstOrDefault();


            if(topic != null)
            {
                // Cannot delete becasue Topic group is using another process
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่";
                return RedirectToAction("Edit", new { id = id, ViewBag.Msg });
            }
            else
            {
                // Can delete topic group
                TopicGroup topicGroup = db.TopicGroups.Find(id);
                db.TopicGroups.Remove(topicGroup);
                db.SaveChanges();
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
