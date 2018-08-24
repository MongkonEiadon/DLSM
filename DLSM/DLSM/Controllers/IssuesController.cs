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
    public class IssuesController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Issues
        public ActionResult Index()
        {
            var context = new DLSMEntities();
            var list = (from i in context.Issues
                        join st in context.Staffs on i.CreateBy equals st.ID
                        join tg in context.TopicGroups on i.TgID equals tg.ID
                        join w in context.Warehouses on i.WhID equals w.ID
                        select new {
                            ID = i.ID,
                            CreateBy = i.CreateBy,
                            CreateName = st.Name,
                            CreateDate = i.CreateDate,
                            TgID = i.TgID,
                            TgName = tg.Name,
                            Subject = i.Subject,
                            Description = i.Description,
                            Status = i.Status,
                            WhID = i.WhID,
                            WhName = w.Name
                        }).AsEnumerable().Select(x => new Issue {
                            ID = x.ID,
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            CreateDate = x.CreateDate.Value.AddYears(543),
                            TgID = x.TgID,
                            TgName = x.TgName,
                            Subject = x.Subject,
                            Description = x.Description,                           
                            Status = x.Status,
                            WhID = x.WhID,
                            WhName = x.WhName
                        }).ToList();

            foreach(var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }
            return View(list);
        }

        public ActionResult Search(DateTime searchFromDate, DateTime searchToDate, string SearchText)
        {
            var context = new DLSMEntities();
            var list = (from i in context.Issues
                        join st in context.Staffs on i.CreateBy equals st.ID
                        join tg in context.TopicGroups on i.TgID equals tg.ID
                        join w in context.Warehouses on i.WhID equals w.ID
                        where i.CreateDate >= searchFromDate && i.CreateDate <= searchToDate && i.Subject.Contains(SearchText)
                        select new
                        {
                            ID = i.ID,
                            CreateBy = i.CreateBy,
                            CreateName = st.Name,
                            CreateDate = i.CreateDate,
                            TgID = i.TgID,
                            TgName = tg.Name,
                            Subject = i.Subject,
                            Description = i.Description,                           
                            Status = i.Status,
                            WhID = i.WhID,
                            WhName = w.Name
                        }).AsEnumerable().Select(x => new Issue
                        {
                            ID = x.ID,
                            CreateBy = x.CreateBy,
                            CreateName = x.CreateName,
                            CreateDate = x.CreateDate.Value.AddYears(543),
                            TgID = x.TgID,
                            TgName = x.TgName,
                            Subject = x.Subject,
                            Description = x.Description,                          
                            Status = x.Status,
                            WhID = x.WhID,
                            WhName = x.WhName
                        }).ToList();

            foreach (var i in list)
            {
                i.StatusName = GetStatusName(i.Status);
            }
            return PartialView("IssueSearch", list);
        }

        // GET: Issues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        // GET: Issues/Create
        public ActionResult Create()
        {
            ViewBag.CommendList = new SelectList(db.Commends, "ID", "Description");
           
            ViewBag.TgList = new SelectList(db.TopicGroups, "ID", "Name");
            ViewBag.WhList = db.Warehouses.ToList();
            ViewBag.StList = db.Staffs.ToList();
            
            var list = db.Topics.ToList();
                foreach(var i in list)
            {
                i.Name = i.Subject;
            }
            ViewBag.TpList = list;
            return View();
        }

        // POST: Issues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                issue.Status = "1";  //กำลังดำเนินการ 

                db.Issues.Add(issue);
                db.SaveChanges();

                int isid = issue.ID;

               if(issue.CommendDescription != null && issue.CommendDescription.ToString() != "")
                {
                    Commend c = new Commend();
                    c.CreateBy = issue.CreateBy;
                    //c.CreateDate = issue.CreateDate;
                    c.Description = issue.CommendDescription;
                    c.Status = issue.Status;
                    c.IsID = isid;

                    db.Commends.Add(c);
                    db.SaveChanges();
                }
               


                return RedirectToAction("Index");
            }

            ViewBag.CommendList = new SelectList(db.Commends, "ID", "Description");

            ViewBag.TgList = new SelectList(db.TopicGroups, "ID", "Name");
            ViewBag.WhList = db.Warehouses.ToList();
            ViewBag.StList = db.Staffs.ToList();

            var list = db.Topics.ToList();
            foreach (var i in list)
            {
                i.Name = i.Subject;
            }
            ViewBag.TpList = list;
            return View(issue);
        }

        // GET: Issues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            var context = new DLSMEntities();

            issue.StatusName = GetStatusName(issue.Status);

            if (issue.WhID != null)
            {
                var wh = context.Warehouses.SingleOrDefault(x => x.ID == issue.WhID);
                issue.WhName = wh.Name;
                issue.WhTelNo = wh.TelNo;
            }

            if (issue.TpID != null)
            {
                var tp = context.Topics.SingleOrDefault(x => x.ID == issue.TpID);
                issue.TpSubject = tp.Subject;
            }

            if (issue.TgID != null)
            {
                var tg = context.TopicGroups.SingleOrDefault(x => x.ID == issue.TgID);
                issue.TgName = tg.Name;
            }

            var st = context.Staffs.SingleOrDefault(x => x.ID == issue.CreateBy);
            issue.CreateName = st.Name;
            issue.StTelNo = st.TelNo;

            issue.CreateDate = issue.CreateDate.Value.AddYears(543);

            if (issue == null)
            {
                return HttpNotFound();
            }
            
            var list = (from c in context.Commends
                        join i in context.Issues on c.IsID equals i.ID
                        where c.IsID == issue.ID
                        select new
                        {
                            ID = c.ID,
                            CreateBy = c.CreateBy,
                            CreateDate = c.CreateDate,
                            Description = c.Description
                          
                        }).AsEnumerable().Select(x => new CommendList
                        {
                            ID = x.ID,
                            CreateBy = x.CreateBy,
                            CreateDate = x.CreateDate,
                            Description = x.Description
                        }).ToList();

            foreach(var e in list)
            {
                var sff = context.Staffs.SingleOrDefault(x => x.ID == e.CreateBy);
                e.CreateName = sff.Name;
            }

            issue.CommendList = list;


            ViewBag.CommendList = new SelectList(db.Commends, "ID", "Description");
            ViewBag.TgList = db.TopicGroups.ToList();
            ViewBag.WhList = db.Warehouses.ToList();
            ViewBag.StList = db.Staffs.ToList();

            //because DB Topic has no name but has subject but Viewbag for dropdownlist must has name
            var listT = db.Topics.ToList();
            foreach (var i in listT)
            {
                i.Name = i.Subject;
            }
            ViewBag.TpList = listT;

            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription,CommendList")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                issue.Status = "1";

                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();

                int isid = issue.ID;

                if (issue.CommendDescription != null && issue.CommendDescription != "")
                {
                    Commend c = new Commend();
                    c.CreateBy = issue.CreateBy;
                    c.CreateDate = issue.CreateDate;
                    c.Description = issue.CommendDescription;
                    c.Status = issue.Status;
                    c.IsID = isid;

                    db.Commends.Add(c);
                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }
            ViewBag.CommendList = new SelectList(db.Commends, "ID", "Description");
            ViewBag.TgList = new SelectList(db.TopicGroups, "ID", "Name");
            ViewBag.WhList = db.Warehouses.ToList();
            ViewBag.StList = db.Staffs.ToList();

            //because DB Topic has no name but has subject but Viewbag for dropdownlist must has name
            var listT = db.Topics.ToList();
            foreach (var i in listT)
            {
                i.Name = i.Subject;
            }
            ViewBag.TpList = listT;
            return View(issue);
        }

        [HttpPost]
        public ActionResult CheckJob([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription,CommendList")] Issue issue)
        {
            issue.Status = "2";

            if(issue.ID == 0)
            {
                db.Issues.Add(issue);
                db.SaveChanges();
            }
            else
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();

            }

            int isid = issue.ID;

            if (issue.CommendDescription != null && issue.CommendDescription != "")
            {
                Commend c = new Commend();
                c.CreateBy = issue.CreateBy;
                c.CreateDate = issue.CreateDate;
                c.Description = issue.CommendDescription;
                c.Status = issue.Status;
                c.IsID = isid;

                db.Commends.Add(c);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
           
        }

        [HttpPost]
        public ActionResult ReqCloseJob([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription,CommendList")] Issue issue)
        {
            issue.Status = "3";
            if (issue.ID == 0)
            {
                db.Issues.Add(issue);
                db.SaveChanges();
            }
            else
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();

            }

            int isid = issue.ID;

            if (issue.CommendDescription != null && issue.CommendDescription != "")
            {
                Commend c = new Commend();
                c.CreateBy = issue.CreateBy;
                c.CreateDate = issue.CreateDate;
                c.Description = issue.CommendDescription;
                c.Status = issue.Status;
                c.IsID = isid;

                db.Commends.Add(c);
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult CloseJob([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription,CommendList")] Issue issue)
        {
            issue.Status = "4";

            if (issue.ID == 0)
            {
                db.Issues.Add(issue);
                db.SaveChanges();
            }
            else
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();

            }

            int isid = issue.ID;

            if (issue.CommendDescription != null && issue.CommendDescription != "")
            {
                Commend c = new Commend();
                c.CreateBy = issue.CreateBy;
                c.CreateDate = issue.CreateDate;
                c.Description = issue.CommendDescription;
                c.Status = issue.Status;
                c.IsID = isid;

                db.Commends.Add(c);
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult NoCloseJob([Bind(Include = "ID,CreateBy,CreateDate,TgID,Subject,Description,Picture,TpID,Status,WhID,CommendDescription,CommendList")] Issue issue)
        {
            issue.Status = "5";

            if (issue.ID == 0)
            {
                db.Issues.Add(issue);
                db.SaveChanges();
            }
            else
            {
                db.Entry(issue).State = EntityState.Modified;
                db.SaveChanges();

            }

            int isid = issue.ID;

            if (issue.CommendDescription != null && issue.CommendDescription != "")
            {
                Commend c = new Commend();
                c.CreateBy = issue.CreateBy;
                c.CreateDate = issue.CreateDate;
                c.Description = issue.CommendDescription;
                c.Status = issue.Status;
                c.IsID = isid;

                db.Commends.Add(c);
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        // GET: Issues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Issue issue = db.Issues.Find(id);
            if (issue == null)
            {
                return HttpNotFound();
            }
            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Issue issue = db.Issues.Find(id);
            db.Issues.Remove(issue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public String GetStatusName(String statusid)
        {
            String Name = null;
            if (statusid.ToString().Contains("1"))
            {
                Name = "กำลังดำเนินการ";
            }
            else if (statusid.ToString().Contains("2"))
            {
                Name = "กำลังตรวจสอบ";
            }
            else if (statusid.ToString().Contains("3"))
            {
                Name = "ขอปิดงาน";
            }
            else if (statusid.ToString().Contains("4"))
            {
                Name = "ปิดงาน";
            }
            else if (statusid.ToString().Contains("5"))
            {
                Name = "ไม่ปิดงาน";
            }
           
            return Name;
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
