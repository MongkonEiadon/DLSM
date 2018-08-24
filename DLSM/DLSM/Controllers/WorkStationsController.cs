using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLSM.Class;
using DLSM.Models;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class WorkStationsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();        
        
        // GET: WorkStations
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            List<WorkStation> w = new List<WorkStation>();
            var result = db.sp_WorkStationIndex(sessionstaffid).ToList();
            foreach (var i in result)
            {
                WorkStation workstation = new WorkStation();
                workstation.ID = i.ID;
                workstation.Name = i.Name;
                workstation.PrinterName = i.PrinterName;
                workstation.Remark = i.Remark;
                w.Add(workstation);
            }

            ViewBag.WarehouseList = db.Warehouses.ToList();
            return View(w);
        }

        public ActionResult Search(int? searchWare)
        {
            var context = new DLSMEntities();

            var sessionstaffid = Convert.ToInt32(Session["UserID"]);
            List<WorkStation> w = new List<WorkStation>();
            var result = db.sp_WorkStationSearch(sessionstaffid, searchWare).ToList();
            foreach (var i in result)
            {
                WorkStation workstation = new WorkStation();
                workstation.ID = i.ID;
                workstation.Name = i.Name;
                workstation.PrinterName = i.PrinterName;
                workstation.Remark = i.Remark;
                w.Add(workstation);
            }
            ViewBag.WarehouseList = db.Warehouses.ToList();

            return View("Index", w);
        }

        // GET: WorkStations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStation workStation = db.WorkStations.Find(id);
            if (workStation == null)
            {
                return HttpNotFound();
            }
            return View(workStation);
        }

        // GET: WorkStations/Create
        public ActionResult Create()
        {
            DLSMEntities context = new DLSMEntities();
            var list = (from p in context.Products
                        where p.IsAsset == "Y"
                        select new
                        {
                                ID = p.ID,
                               Name = p.Name
                        });

            

            ViewBag.WhID = db.Warehouses.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult GetProdByWH(int WhID)
        {
            //find product in warehouse
            var prd = (from s in db.Stocks
                       join p in db.Products on s.PdID equals p.ID
                       where s.WhID == WhID && p.IsAsset == "Y"
                       select new
                       {
                           p.ID,
                           p.Name
                       }).ToList();
            
            return Json(prd);
        }

        // POST: WorkStations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]        
        public ActionResult Create(WorkStation workStation)
        {
           
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var wk = context.WorkStations.SingleOrDefault(w => w.WhID == workStation.WhID && w.Name == workStation.Name);
                        if (wk != null)
                        {
                            return Json("มีชื่อ WorkStation นี้แล้ว กรุณาเปลี่ยนชื่อ");
                        }
                        else
                        {
                            context.WorkStations.Add(workStation);
                            context.SaveChanges();

                            int WsID = workStation.ID;

                            workStation.WorkStationDetailList.RemoveAt(0);
                            foreach (var i in workStation.WorkStationDetailList)
                            {
                                // Workstation Detail
                                WorkSationDetail dd = new WorkSationDetail();
                                dd.PdID = i.PdID;
                                dd.WsID = WsID;
                                dd.SerialNO = i.SerialNO;
                                context.WorkSationDetails.Add(dd);

                                // Find Product Serial
                                //Update                     
                                var upd = (from d in context.StockSerials
                                           where d.WhID == workStation.WhID &&
                                            d.PdID == i.PdID && d.SerialBegin == i.SerialNO
                                           select d).ToList().SingleOrDefault();
                                upd.IpProperty = i.IP;
                                upd.Name = i.Name;
                                context.SaveChanges();
                            }

                            workStation.WorkStationStaffList.RemoveAt(0);
                            foreach (var e in workStation.WorkStationStaffList)
                            {
                                WorkSationStaff ee = new WorkSationStaff();
                                ee.WsID = WsID;
                                ee.StID = e.StID;
                                context.WorkSationStaffs.Add(ee);
                            }


                            context.SaveChanges();
                            dbContextTransaction.Commit();

                            return Json("success");
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(ex.InnerException.Message);
                    }
                }
            }
            

            ViewBag.WhID = db.Warehouses.ToList();
            return View(workStation);
        }

        // GET: WorkStations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStation workStation = db.WorkStations.Find(id);
            if (workStation == null)
            {
                return HttpNotFound();
            }
            var context = new DLSMEntities();
            var list = (from ws in context.WorkStations
                        join wd in context.WorkSationDetails on ws.ID equals wd.WsID
                        join p in context.Products on wd.PdID equals p.ID
                        join s in context.StockSerials on new { a1 = wd.PdID.Value, a2 = ws.WhID, a3 = wd.SerialNO } equals new { a1 = s.PdID.Value, a2 = (int)s.WhID, a3 = s.SerialBegin } 
                        where ws.ID == id
                        select new
                        {
                            WsID = wd.WsID,
                            PdID = wd.PdID,
                            PdName = p.Name,
                            SerialNO = wd.SerialNO,
                            IP = (s.IpProperty == null ? "" : s.IpProperty),
                            Name = (s.Name == null ? "" : s.Name)
                        }).AsEnumerable().Select(x => new WorkStationDetailList{
                            WsID = x.WsID,
                            PdID = x.PdID,
                            PdName = x.PdName,
                            SerialNO = x.SerialNO,
                            IP = x.IP,
                            Name = x.Name
                        }).ToList();

            workStation.WorkStationDetailList = list;

            var listStaff = (from ws in context.WorkStations
                             join wf in context.WorkSationStaffs on ws.ID equals wf.WsID
                             join st in context.Staffs on wf.StID equals st.ID
                             where ws.ID == id
                             select new
                             {
                                 WsID = wf.WsID,
                                 StID = wf.StID,
                                 StName = st.Name
                                 
                             }).AsEnumerable().Select(x => new WorkStationStaffList
                             {
                                 WsID = x.WsID,
                                 StID = x.StID,
                                 StName = x.StName
                             }).ToList();

            workStation.WorkStationStaffList = listStaff;

            var wh = context.Warehouses.SingleOrDefault(x => x.ID == workStation.WhID);
            workStation.WhName = wh.Name;
            

            ViewBag.WhID = db.Warehouses.ToList();

            return View(workStation);
        }

        // POST: WorkStations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(WorkStation workStation)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var wk = context.WorkStations.SingleOrDefault(w => w.WhID == workStation.WhID && w.Name == workStation.Name && (w.ID != workStation.ID));
                        if(wk != null)
                        {
                            return Json("มีชื่อ WorkStation นี้แล้ว กรุณาเปลี่ยนชื่อ");
                        }
                        else
                        {
                            context.Entry(workStation).State = EntityState.Modified;

                            int WsID = workStation.ID;

                            //WorkSationDetail wd = context.WorkSationDetails.Find(WsID);
                            context.WorkSationDetails.RemoveRange(context.WorkSationDetails.Where(x => x.WsID == WsID));
                            context.SaveChanges();

                            workStation.WorkStationDetailList.RemoveAt(0);
                            foreach (var i in workStation.WorkStationDetailList)
                            {
                                WorkSationDetail dd = new WorkSationDetail();
                                dd.PdID = i.PdID;
                                dd.WsID = WsID;
                                dd.SerialNO = i.SerialNO;
                                context.WorkSationDetails.Add(dd);


                                // Find Product Serial
                                //Update                     
                                var upd = (from d in context.StockSerials
                                           where d.WhID == workStation.WhID &&
                                            d.PdID == i.PdID && d.SerialBegin == i.SerialNO
                                           select d).ToList().SingleOrDefault();
                                upd.IpProperty = i.IP;
                                upd.Name = i.Name;
                                context.SaveChanges();

                            }

                            //WorkSationStaff wf = context.WorkSationStaffs.Find(WsID);
                            context.WorkSationStaffs.RemoveRange(context.WorkSationStaffs.Where(x => x.WsID == WsID));
                            context.SaveChanges();

                            workStation.WorkStationStaffList.RemoveAt(0);
                            foreach (var e in workStation.WorkStationStaffList)
                            {
                                WorkSationStaff ee = new WorkSationStaff();
                                ee.WsID = WsID;
                                ee.StID = e.StID;
                                context.WorkSationStaffs.Add(ee);
                            }


                            context.SaveChanges();
                            dbContextTransaction.Commit();

                            return Json("success");
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(ex.InnerException.Message);
                    }
                }
            }

            ViewBag.WhID = db.Warehouses.ToList();
            return View(workStation);
        }

        [HttpPost]
        public ActionResult GetSerialNo(int pdID,int whID)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find product in warehouse
                var serial = (from sn in db.StockSerials
                           where sn.WhID == whID && sn.PdID == pdID && sn.SerialBegin != "0"
                          select new
                           {
                               sn.ID,
                               sn.SerialBegin,
                               sn.Name,
                               sn.IpProperty
                           }).ToList();

                return Json(serial);
            }
        }

        [HttpPost]
        public ActionResult GetStaffByWH(int whID)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find staff in warehouse
                var staff = (from sw in db.StaffWarehouses
                             join s in db.Staffs on sw.StID equals s.ID
                             where sw.WhID == whID
                             select new
                             {
                                 s.ID,
                                 s.Name
                             }).ToList();

                return Json(staff);
            }
        }

        [HttpPost]
        public ActionResult GetWH()
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                //find staff in warehouse
                var SessionUserID = Session["UserID"].ToString();

                var configadminpass = context.Configures.SingleOrDefault(c => c.Code == "ADMINPASS");

                var wh = db.sp_WarehouseAuthority(Convert.ToInt32(SessionUserID)).ToList();
                //var wh = (from s in context.Staffs
                //          join st in context.StaffWarehouses on s.ID equals st.StID
                //          join w in context.Warehouses on st.WhID equals w.ID
                //          where s.ID.ToString() == SessionUserID
                //          select new
                //          {
                //              ID = st.WhID,
                //              Name = w.Name
                //          }).ToList();

                //if (SessionUserID == configadminpass.Value)
                //{
                //    wh = wh.Union(from w in context.Warehouses
                //                  select new
                //                  {
                //                      ID = w.ID,
                //                      Name = w.Name
                //                  }).Distinct().ToList();
                //}
                return Json(wh);

            }
        }

        // GET: WorkStations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkStation workStation = db.WorkStations.Find(id);
            if (workStation == null)
            {
                return HttpNotFound();
            }
            return View(workStation);
        }

        
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {

            db.WorkSationDetails.RemoveRange(db.WorkSationDetails.Where(x => x.WsID == id));
            db.SaveChanges();

            db.WorkSationStaffs.RemoveRange(db.WorkSationStaffs.Where(x => x.WsID == id));
            db.SaveChanges();

            WorkStation workStation = db.WorkStations.Find(id);
            db.WorkStations.Remove(workStation);
            db.SaveChanges();

            TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";
 
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
