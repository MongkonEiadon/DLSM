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
    public class TopicsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Topics
        public ActionResult Index()
        {
            var context = new DLSMEntities();
            var list = (from t in context.Topics
                        join tg in context.TopicGroups on t.TgID equals tg.ID
                        join st in context.Staffs on t.CreateBy equals st.ID
                        select new {
                            ID = t.ID,
                            Subject = t.Subject,
                            Symptom = t.Symptom,
                            Cause = t.Cause,
                            Solving = t.Solving,
                            CreateBy = t.CreateBy,
                            CreateName = st.Name,
                            CreateDate = t.CreateDate,
                            TgName = tg.Name

                        }).AsEnumerable().Select (x => new Topic {
                            ID = x.ID,
                            Subject = x.Subject,
                            Symptom = x.Symptom,
                            Cause = x.Cause,
                            Solving = x.Solving,
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            CreateDate = x.CreateDate.Value.AddYears(543),
                            TgName = x.TgName
                        }).ToList();

            ViewBag.TgList = new SelectList(db.TopicGroups,"ID","Name");
            return View(list);
        }

        [HttpPost]
        public ActionResult Search(String SearchText,int SearchTg)
        {
            var context = new DLSMEntities();
            var list = (from t in context.Topics
                        join tg in context.TopicGroups on t.TgID equals tg.ID
                        join st in context.Staffs on t.CreateBy equals st.ID
                        where t.Subject.Contains(SearchText) && t.TgID == SearchTg
                        select new
                        {
                            ID = t.ID,
                            Subject = t.Subject,
                            Symptom = t.Symptom,
                            Cause = t.Cause,
                            Solving = t.Solving,
                            CreateBy = t.CreateBy,
                            CreateName = st.Name,
                            CreateDate = t.CreateDate,
                            TgName = tg.Name

                        }).AsEnumerable().Select(x => new Topic
                        {
                            ID = x.ID,
                            Subject = x.Subject,
                            Symptom = x.Symptom,
                            Cause = x.Cause,
                            Solving = x.Solving,
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            CreateDate = x.CreateDate.Value.AddYears(543),
                            TgName = x.TgName
                        }).ToList();

            ViewBag.TgList = new SelectList(db.TopicGroups, "ID", "Name");
            return PartialView("TopicSearch",list);
        }

        // GET: Topics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // GET: Topics/Create
        public ActionResult Create()
        {
            ViewBag.TgList = new SelectList(db.TopicGroups, "ID", "Name");
            return View();
        }

        // POST: Topics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Subject,Symptom,Cause,Solving,CreateBy,CreateDate,TgID,LocationID")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                db.Topics.Add(topic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topic.ID);
            ViewBag.ID = new SelectList(db.TopicGroups, "ID", "Name", topic.ID);
            return View(topic);
        }

        // GET: Topics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            var context = new  DLSMEntities();
            var st = context.Staffs.SingleOrDefault(n => n.ID == topic.CreateBy);
            topic.CreateName = st.Name;

            var tg = context.TopicGroups.SingleOrDefault(n => n.ID == topic.TgID);
            topic.TgID = tg.ID;
            topic.TgName = tg.Name;

            topic.CreateDate = topic.CreateDate.Value.AddYears(543);
            
            var oldlist = (from t in context.Topics
                           where t.Subject.Contains(topic.Subject)
                           select new
                           {
                               OldID = t.ID,
                               OldSubject = t.Subject,
                               OldCreateDate = t.CreateDate
                           }).AsEnumerable().Select(c => new OldList
                           {
                               OldID = c.OldID,
                               OldSubject = c.OldSubject,
                               OldCreateDate = c.OldCreateDate.Value.AddYears(543)
                           }).ToList();


            ViewBag.TgList = db.TopicGroups.ToList();

            topic.OldList = oldlist;

            return View(topic);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Subject,Symptom,Cause,Solving,CreateBy,CreateDate,TgID,LocationID")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(topic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topic.ID);
            ViewBag.ID = new SelectList(db.TopicGroups, "ID", "Name", topic.ID);
            return View(topic);
        }

        [HttpPost]
        public ActionResult EditData(Topic topic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(topic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Issues, "ID", "Subject", topic.ID);
            ViewBag.ID = new SelectList(db.TopicGroups, "ID", "Name", topic.ID);
            return View(topic);
        }

        // GET: Topics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // POST: Topics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topic topic = db.Topics.Find(id);
            db.Topics.Remove(topic);
            db.SaveChanges();
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
