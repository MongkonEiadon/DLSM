using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DLSM.Models;
using System.Text;
using DLSM.Class;
using System.IO;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class StaffsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();
        private int StaffID ;
               

        // GET: Staffs
        public ActionResult Index()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }

            var context = new DLSMEntities();
            var UserUgID = Session["UserUgID"].ToString();
            var UserID = Session["UserID"].ToString();
            List<Staff> listT = new List<Staff>();
            if (UserUgID == "1")
            { //ผู้ดูแลระบบ
                var list = (from st in context.Staffs
                            join ug in context.UserGroups on st.UgID equals ug.ID
                            join sw in context.StaffWarehouses on st.ID equals sw.StID
                            join w in context.Warehouses on sw.WhID equals w.ID
                            select new
                            {
                                ID = st.ID,
                                Name = st.Name,
                                Email = st.Email,
                                TelNo = st.TelNo,
                                WhName = w.Name
                            }).AsEnumerable().Select(x => new Staff
                            {
                                ID = x.ID,
                                Name = x.Name,
                                Email = x.Email,
                                TelNo = x.TelNo,
                                WhName = x.WhName
                            });
                listT = list.ToList();
            }
            else
            {  //ผู้จัดการ
                var uglist = (from st in context.Staffs
                              join sw in context.StaffWarehouses on st.ID equals sw.StID
                              where st.ID.ToString() == UserID
                              select sw.Warehouse);
                    
                    var list = (from st in context.Staffs
                            join ug in context.UserGroups on st.UgID equals ug.ID
                            join sw in context.StaffWarehouses on st.ID equals sw.StID
                            join w in context.Warehouses on sw.WhID equals w.ID
                            join x in uglist on w.ID equals x.ID
                            select new
                            {
                                ID = st.ID,
                                Name = st.Name,
                                Email = st.Email,
                                TelNo = st.TelNo,
                                WhName = w.Name
                            }).AsEnumerable().Select(x => new Staff
                            {
                                ID = x.ID,
                                Name = x.Name,
                                Email = x.Email,
                                TelNo = x.TelNo,
                                WhName = x.WhName
                            });

                


                listT = list.ToList();
            }
           
            return View(listT);
        }

        public ActionResult SearchStaff(String SearchText)
        {
            var context = new DLSMEntities();
            var UserUgID = Session["UserUgID"].ToString();
            var UserID = Session["UserID"].ToString();
            List<Staff> listT = new List<Staff>();
            if (UserUgID == "1")
            { //ผู้ดูแลระบบ
                var list = (from st in context.Staffs
                            join ug in context.UserGroups on st.UgID equals ug.ID
                            join sw in context.StaffWarehouses on st.ID equals sw.StID
                            join w in context.Warehouses on sw.WhID equals w.ID
                            where st.Name.Contains(SearchText)
                            select new
                            {
                                ID = st.ID,
                                Name = st.Name,
                                Email = st.Email,
                                TelNo = st.TelNo,
                                WhName = w.Name
                            }).AsEnumerable().Select(x => new Staff
                            {
                                ID = x.ID,
                                Name = x.Name,
                                Email = x.Email,
                                TelNo = x.TelNo,
                                WhName = x.WhName
                            });
                listT = list.ToList();
            }
            else
            {  //ผู้จัดการ
                var uglist = (from st in context.Staffs
                              join sw in context.StaffWarehouses on st.ID equals sw.StID
                              where st.ID.ToString() == UserID
                              select sw.Warehouse);

                var list = (from st in context.Staffs
                            join ug in context.UserGroups on st.UgID equals ug.ID
                            join sw in context.StaffWarehouses on st.ID equals sw.StID
                            join w in context.Warehouses on sw.WhID equals w.ID
                            join x in uglist on w.ID equals x.ID
                            where st.Name.Contains(SearchText)
                            select new
                            {
                                ID = st.ID,
                                Name = st.Name,
                                Email = st.Email,
                                TelNo = st.TelNo,
                                WhName = w.Name
                            }).AsEnumerable().Select(x => new Staff
                            {
                                ID = x.ID,
                                Name = x.Name,
                                Email = x.Email,
                                TelNo = x.TelNo,
                                WhName = x.WhName
                            });
                

                listT = list.ToList();
            }
            
            return View("Index", listT);
        }
        // GET: Staffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

     
        
        // GET: Staffs/Create
        public ActionResult Create()
        {
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            ViewBag.UgID = new SelectList(db.UserGroups, "ID", "Name");
            ViewBag.Warehouselist = new SelectList((from w in db.Warehouses
                                                   select new
                                                   {
                                                       ID = w.ID,
                                                       Name = w.Code + " - " + w.Name
                                                   }), "ID", "Name");
            var configList = (from c in db.Configures
                                       where c.Code == "AUTHORIZE"
                                       select c.Value).ToList();
            ViewBag.ConfigAuthorize = configList[0];
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,UserLogin,UserPassword,UgID,TelNo,Email")] Staff staff)
        {
                DLSMEntities db = new DLSMEntities();
                    Staff stf = new Staff();
                    stf.Name = staff.Name;
                    stf.UserLogin = staff.UserLogin;
                    stf.UserPassword = Hash(staff.UserPassword);
                    stf.UgID = staff.UgID;
                    stf.TelNo = staff.TelNo;
                    stf.Email = staff.Email;


                    db.Staffs.Add(stf);
                    db.SaveChanges();

                    StaffID = stf.ID;

            ViewBag.Warehouselist = new SelectList(db.Warehouses, "ID", "Name");
            ViewBag.UgID = new SelectList(db.UserGroups, "ID", "Name");
            ViewData["StaffID"] = StaffID;
            return View();
        }

        //เข้ารหัสพาสเวิด
        public string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hasBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hasBytes);
        }

        [HttpPost]
        public ActionResult InsertStaff(Staff model)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Staff w = context.Staffs.Where(p => p.UserLogin == model.UserLogin).FirstOrDefault();

                        if (w != null)
                        {
                            return Json("UserLogin นี้มีอยู่แล้วในระบบ");
                        }

                        model.UserPassword = Hash(model.UserPassword);

                        context.Staffs.Add(model);
                        context.SaveChanges();

                        StaffID = model.ID;

                        model.StWh.RemoveAt(0);
                        foreach (var a in model.StWh)
                        {
                            StaffWarehouse SW = new StaffWarehouse();
                            SW.StID = StaffID;
                            SW.WhID = a.WhID;
                            SW.IsManager = a.IsManager;

                            context.StaffWarehouses.Add(SW);
                        }

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(ex.InnerException.Message);
                    }
                }
            }
                        
           
            return Json(StaffID);
        }

        public ActionResult PopUpModal()
        {
            ViewBag.Warehouselist = new SelectList(db.Warehouses, "ID", "Name");
            return View();
        }
        // GET: Staffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            //Werning can't delete data
            if (null != TempData["Msg"])
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var configList = (from c in db.Configures
                              where c.Code == "AUTHORIZE"
                              select c.Value).ToList();
            ViewBag.ConfigAuthorize = configList[0];

            var context = new DLSMEntities();
            var list = (from s in context.Staffs join sf in context.StaffWarehouses on s.ID equals sf.StID
                        join w in context.Warehouses on sf.WhID equals w.ID
                        join ug in context.UserGroups on s.UgID equals ug.ID
                        where s.ID == id select new {
                            Name = s.Name,
                            UserLogin = s.UserLogin,
                            UserPassword = s.UserPassword,
                            TelNo = s.TelNo,
                            Email = s.Email,
                            UgID = s.UgID,
                            UgName = ug.Name,
                            WhID = w.ID,
                            WhName = w.Name,
                            IsManager = sf.IsManager,
                            ID = s.ID
                        }).AsEnumerable().Select (x => new StaffWarehouseList
                        {
                            IsManager = x .IsManager,
                             StID = x.ID,
                             WhID = x.WhID,
                             WhName = x.WhName,

                        }).ToList();
            ViewBag.UgID = new SelectList(db.UserGroups.ToList(), "ID", "Name");
            ViewBag.Warehouselist = new SelectList((from w in db.Warehouses
                                                    select new
                                                    {
                                                        ID = w.ID,
                                                        Name = w.Code + " - " + w.Name
                                                    }), "ID", "Name");


            staff.StWh = list;

            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,UserLogin,UserPassword,UgID,TelNo,Email")] Staff staff)
        {
            if (ModelState.IsValid)
            {
              
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();

                StaffID = staff.ID;

                //Delete old data
                StaffWarehouse sf = db.StaffWarehouses.Find(StaffID);
                db.StaffWarehouses.Remove(sf);
                db.SaveChanges();


                staff.StWh.RemoveAt(0);

                
                foreach (var a in staff.StWh)
                {
                    StaffWarehouse SW = new StaffWarehouse();
                    SW.StID = StaffID;
                    SW.WhID = a.WhID;
                    SW.IsManager = a.IsManager;

                    db.StaffWarehouses.Add(SW);
                    db.SaveChanges();
                }

                return Json("success");
            }
            ViewBag.ID = new SelectList(db.UserGroups, "ID", "Name", staff.ID);
            return View(staff);
        }

        [HttpPost]
        public JsonResult EditStaff(Staff staff)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
               
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    { 
                        if (staff != null && staff.ID != 0)
                        {
                            
                            Staff w = context.Staffs.Where(p => (p.UserLogin == staff.UserLogin) && (p.ID != staff.ID)).FirstOrDefault();

                            if (w != null)
                            {
                                return Json("UserLogin นี้มีอยู่แล้วในระบบ");
                            }

                            if (staff.NewUserPassword != null && staff.NewUserPassword != "")
                            {
                                staff.UserPassword = Hash(staff.NewUserPassword);
                            }

                            context.Entry(staff).State = EntityState.Modified;
                            context.SaveChanges();

                            StaffID = staff.ID;

                            context.StaffWarehouses.RemoveRange(context.StaffWarehouses.Where(x => x.StID == StaffID));
                            context.SaveChanges();

                            staff.StWh.RemoveAt(0);

                            foreach (var a in staff.StWh)
                            {
                                StaffWarehouse SW = new StaffWarehouse();
                                SW.StID = StaffID;
                                SW.WhID = a.WhID;
                                SW.IsManager = a.IsManager;

                                context.StaffWarehouses.Add(SW);

                            }                           
                            context.SaveChanges();

                            //Update some data Referance with Authorize
                            var updRef =(from rf in context.Staffs
                                         where rf.RefAuthorize == staff.ID
                                         select rf).ToList();
                            foreach (var i in updRef)
                            {
                                i.TitleTH = staff.TitleTH;
                                i.AuthorizeNameTH = staff.AuthorizeNameTH;
                                i.AuthorizeLastNameTH = staff.AuthorizeLastNameTH;
                                i.TitleEN = staff.TitleEN;
                                i.AuthorizeNameEN = staff.AuthorizeNameEN;
                                i.AuthorizeLastNameEN = staff.AuthorizeLastNameEN;
                                i.Signature = staff.Signature;
                                i.AuthorityType = staff.AuthorityType;
                                i.AuthorizeIdCard = staff.AuthorizeIdCard;
                            }
                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                    }
                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return Json(ex.InnerException.Message);
                    }
                }
            }
                   
            return Json(StaffID);
        }
            // GET: Staffs/Delete/5
            public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirmed(int id)
        {
            var context = new DLSMEntities();
            Document dd = context.Documents.Where(p => p.CreateBy == id || p.ApproveBy == id).FirstOrDefault();


            if (dd != null)
            {
                TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่";
            }
            else
            {
                TransferStaff df = context.TransferStaffs.Where(p => p.StID == id).FirstOrDefault();
                if(df != null)
                {
                    TempData["Msg"] = "ลบไม่ได้ เนื่องจากข้อมูลมีการถูกใช้งานอยู่";
                }
                else
                {
                    db.StaffWarehouses.RemoveRange(db.StaffWarehouses.Where(x => x.StID == id));
                    db.SaveChanges();

                    Staff staff = db.Staffs.Find(id);
                    db.Staffs.Remove(staff);
                    db.SaveChanges();
                    TempData["Msg"] = "ลบข้อมูลเรียบร้อยแล้ว";
                }                
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
        public JsonResult UploadSignature(int StaffID)
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

                Staff staff = db.Staffs.Find(StaffID);
                staff.Signature = img64;
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();

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

        [HttpPost]
        public ActionResult GetAuthorize()
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                var con = (from c in context.Configures
                           where c.Code == "AUTHORIZE"
                           select c.Value).ToList();
                int i = Int32.Parse(con[0]);

                var getWh = context.sp_GetMainWarehouse(Convert.ToInt32(Session["UserID"])).ToList();
                int wh = getWh[0].ID.Value;

                var a = (from s in context.Staffs
                         join sw in context.StaffWarehouses on s.ID equals sw.StID
                         join w in context.Warehouses on sw.WhID equals w.ID
                         where s.UgID == i && w.ID == wh
                          select new
                          {
                              ID = s.ID,
                              TitleTH = s.TitleTH,
                              AuthorizeNameTH = s.AuthorizeNameTH,
                              AuthorizeLastNameTH = s.AuthorizeLastNameTH,
                              TitleEN = s.TitleEN,
                              AuthorizeNameEN = s.AuthorizeNameEN,
                              AuthorizeLastNameEN =s.AuthorizeLastNameEN,
                              Signature = s.Signature,
                              AuthorityType = s.AuthorityType,
                              AuthorizeIdCard = s.AuthorizeIdCard
                          }).ToList();

                return Json(a);
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}
