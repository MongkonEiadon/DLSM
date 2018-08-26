using DLSM.Class;
using DLSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLSM.Controllers
{
    [SessionCheck]
    public class ReportController : Controller
    {
        private DLSMEntities db = new DLSMEntities();
        
        // GET: Report
        public ActionResult Index()
        {
            using (DLSMEntities db = new DLSMEntities())
            {
                if (null != Session["UserUgID"])
                {
                    var UgID = int.Parse(Session["UserUgID"].ToString());
                    var MenuList = (from p in db.Permissions
                                    join m in db.Modules on p.MdCode equals m.Code
                                    where p.UgID == UgID
                                    select m.Name).ToList();

                    var MenuSettingAccess = sortMenu(MenuList);

                    ViewBag.MenuSettingAccess = MenuSettingAccess;
                }

            }
            return View();
        }

        
        public ActionResult Report1()
        {

            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        public async Task<ActionResult> ViewOrDownload(Reports model, string name)
        {
            string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            string output_type = ".pdf";
            switch (model.OutputType)
            {
                case Class.output_type.excel:
                    output_type = ".xls";

                    var memory = new MemoryStream();
                    string filename = Path.Combine(path, name + output_type);
                    Thread.Sleep(2000);
                    if (System.IO.File.Exists(filename))
                    {

                        using (var stream = new FileStream(filename, FileMode.Open))
                        {
                            await stream.CopyToAsync(memory);
                        }
                    }
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name + output_type);

                default:
                    output_type = ".pdf";
                    if (isReportExists(path + "\\" + name + output_type))
                    {

                        TempData["Name"] = name;
                        return RedirectToAction("ViewReport");
                    }
                    else
                    {
                        ViewBag.Msg = "Load Report Fail !";
                        return View("ReportFail");
                    }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Report1(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report1");
            }
            else
            {
                name = CreateXML(model, "report1_Choose");
            }

            return await ViewOrDownload(model, name);



        }
       
        public ActionResult Report2()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult GetST(int WhID)
        {
            using (DLSMEntities context = new DLSMEntities())
            {
                try
                {
                    var stt = (from st in context.StaffWarehouses
                               join s in context.Staffs on st.StID equals s.ID
                               where st.WhID == WhID
                               select new { s.ID, s.Name }).ToList();


                    return Json(stt);
                }
                catch(Exception ex)
                {
                    return Json("failed");
                }
            }
        }
        [HttpPost]
        public async Task<ActionResult> Report2(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report2");
            }
            else
            {
                name = CreateXML(model, "report2_Choose");
            }
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report3()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report3(Reports model)
        {
            string name = CreateXML(model, "report3");

            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report4()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report4(Reports model)
        {
            string name =CreateXML(model, "report4");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }


        public ActionResult Report5()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report5(Reports model)
        {
            string name = CreateXML(model, "report5");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report6()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report6(Reports model)
        {
            string name =CreateXML(model, "report6");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report7()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report7(Reports model)
        {
            string name =CreateXML(model, "report7");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report8()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report8(Reports model)
        {
            string name =CreateXML(model, "report8");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report9()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report9(Reports model)
        {
            string name =CreateXML(model, "report9");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report10()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report10(Reports model)
        {
            string name = CreateXML(model, "report10");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report11()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report11(Reports model)
        {
            string name = CreateXML(model, "report11");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult ViewReport()
        {
            string a = TempData["Name"].ToString();
            string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            DateTime starttime = DateTime.Now;
            try
            {
                String[] files = Directory.GetFiles(@path, a + ".pdf"); // return full path
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        var fullPathToFile = file;
                        var mimeType = "application/pdf";
                        var fileContents = System.IO.File.ReadAllBytes(fullPathToFile);

                        return new FileContentResult(fileContents, mimeType);
                    }
                }
                else
                {

                    Console.Write("\r{0}:{1}", DateTime.Now, "No job in queue.");
                }
            }
            catch (Exception exInner)
            {
                Console.WriteLine("{0}:{1}", DateTime.Now, exInner.Message);
            }


            return RedirectToAction("Index");

        }

        public ActionResult Report12()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report12(Reports model)
        {
            string name = CreateXML(model, "report12");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report13()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report13(Reports model)
        {
            string name = CreateXML(model, "report13");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult ViewExcelReport()
        {
            string a = TempData["Name"].ToString();
            string path = ConfigurationManager.AppSettings.Get("ROOT_EXCEL");
            DateTime starttime = DateTime.Now;
            try
            {
                String[] files = Directory.GetFiles(@path, a + ".xls"); // return full path
                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        var fullPathToFile = file;
                        var mimeType = "application/excel";
                        var fileContents = System.IO.File.ReadAllBytes(fullPathToFile);

                        return new FileContentResult(fileContents, mimeType);
                    }
                }
                else
                {

                    Console.Write("\r{0}:{1}", DateTime.Now, "No job in queue.");
                }
            }
            catch (Exception exInner)
            {
                Console.WriteLine("{0}:{1}", DateTime.Now, exInner.Message);
            }


            return RedirectToAction("Index");

        }

        public ActionResult Report14()
        {
            var SessionUserID = Session["UserID"].ToString();

            ViewBag.StID = db.Staffs.ToList();
            var wh = (from s in db.Staffs
                      join st in db.StaffWarehouses on s.ID equals st.StID
                      join w in db.Warehouses on st.WhID equals w.ID
                      where s.ID.ToString() == SessionUserID
                      select w).ToList();
            ViewBag.WhID = wh.GroupBy(w => w.ID).Select(w => w.First()).ToList();
            ViewBag.PdID = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report14(Reports model)
        {
            string name = CreateXML(model, "report14");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}
        }

        public ActionResult Report15()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9" 
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report15(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report15");
            }
            else
            {
                name = CreateXML(model, "report15_Choose");
            }

            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        public ActionResult Report16()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9"
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report16(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report16");
            }
            else
            {
                name = CreateXML(model, "report16_Choose");
            }

            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        public ActionResult Report17()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9"
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report17(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report17");
            }
            else
            {
                name = CreateXML(model, "report17_Choose");
            }
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        public ActionResult Report18()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9"
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report18(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report18");
            }
            else
            {
                name = CreateXML(model, "report18_Choose");
            }

            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        public ActionResult Report19()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9"
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report19(Reports model)
        {
            int PdCode = model.PdCode;
            string name = CreateXML(model, "report19");
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        public ActionResult Report20()
        {
            var SessionUserID = Session["UserID"];
            ViewBag.StID = SessionUserID;
            var Wh = db.sp_WarehouseAuthority(Convert.ToInt32(Session["UserID"])).ToList();
            ViewBag.WhID = Wh;
            var pd = (from p in db.Products
                      join ct in db.Categories on p.CtID equals ct.ID
                      where ct.ID.ToString() == "9"
                      select p).ToList();
            ViewBag.PdID = pd;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Report20(Reports model)
        {
            int PdCode = model.PdCode;
            string name;
            if (PdCode == 0)
            {
                name = CreateXML(model, "report20");
            }
            else
            {
                name = CreateXML(model, "report20_Choose");
            }
            return await ViewOrDownload(model, name);
            //string path = ConfigurationManager.AppSettings.Get("ROOT_PDF");
            //if (isReportExists(path + "\\" + name + ".pdf"))
            //{
            //    TempData["Name"] = name;
            //    return RedirectToAction("ViewReport");
            //}
            //else
            //{
            //    ViewBag.Msg = "Load Report Fail !";
            //    return View("ReportFail");
            //}

        }

        private bool isReportExists(string filename)
        {
            double seconds = double.Parse(ConfigurationManager.AppSettings.Get("COUNT_DOWN_REPORT"));
            bool success = false;
            int elapsed = 0;
            while ((!success) && (elapsed < TimeSpan.FromSeconds(seconds).TotalMilliseconds))
            {
                if (System.IO.File.Exists(filename))
                {
                    success = true;
                    goto EndWhile;
                }
                Thread.Sleep(1000);
                elapsed += 1000;
            }

            EndWhile:
            return success;
        }

        private List<ReportList> GenItem(Reports model, string report_no)
        {
            var SessionUserID = Session["UserID"].ToString();
            List<ReportList> itm = new List<ReportList>() ;
            ReportList rt = new ReportList();
            if (model != null)
            {
                if (report_no == "report1" || report_no == "report1_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    ReportList rt3 = new ReportList();
                    rt3.Name = "@stid";
                    rt3.Type = "Integer";
                    rt3.Value = SessionUserID;
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@pdid";
                    rt4.Type = "Integer";
                    rt4.Value = model.PdCode.ToString();
                    itm.Add(rt4);

                }
                else if (report_no == "report2" || report_no == "report2_Choose"  || report_no == "report5" || report_no == "report11")
                {
                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@pdid";
                    rt5.Type = "Integer";
                    rt5.Value = model.PdCode.ToString();
                    itm.Add(rt5);

                    ReportList rt6 = new ReportList();
                    rt6.Name = "@stid";
                    rt6.Type = "Integer";
                    rt6.Value = model.StCode.ToString();
                    itm.Add(rt6);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    ReportList rt7 = new ReportList();
                    rt7.Name = "PdName";
                    rt7.Type = "String";
                    rt7.Value = (model.PdName == null ? "(ไม่ระบุ)" : model.PdName);
                    itm.Add(rt7);

                    ReportList rt8 = new ReportList();
                    rt8.Name = "StName";
                    rt8.Type = "String";
                    rt8.Value = (model.StName == null ? "(ไม่ระบุ)" : model.StName);
                    itm.Add(rt8);

                    string fdate2 = Convert.ToString(model.FormDate.AddYears(543).ToString("dd/MM/yyyy"));
                    string tdate2 = Convert.ToString(model.ToDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt9 = new ReportList();
                    rt9.Name = "d1";
                    rt9.Type = "String";
                    rt9.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate2);
                    itm.Add(rt9);

                    ReportList rt10 = new ReportList();
                    rt10.Name = "d2";
                    rt10.Type = "String";
                    rt10.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate2);
                    itm.Add(rt10);

                    ReportList rt11 = new ReportList();
                    rt11.Name = "@stidpermit";
                    rt11.Type = "Integer";
                    rt11.Value = SessionUserID;
                    itm.Add(rt11);

                }
                else if (report_no == "report3" || report_no == "report4" || report_no == "report6" || report_no == "report7")
                {
                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@pdid";
                    rt5.Type = "Integer";
                    rt5.Value = model.PdCode.ToString();
                    itm.Add(rt5);

                    ReportList rt6 = new ReportList();
                    rt6.Name = "@stid";
                    rt6.Type = "Integer";
                    rt6.Value = model.StCode.ToString();
                    itm.Add(rt6);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    ReportList rt7 = new ReportList();
                    rt7.Name = "PdName";
                    rt7.Type = "String";
                    rt7.Value = (model.PdName == null ? "(ไม่ระบุ)" : model.PdName);
                    itm.Add(rt7);

                    string fdate2 = Convert.ToString(model.FormDate.AddYears(543).ToString("dd/MM/yyyy"));
                    string tdate2 = Convert.ToString(model.ToDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt9 = new ReportList();
                    rt9.Name = "d1";
                    rt9.Type = "String";
                    rt9.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate2);
                    itm.Add(rt9);

                    ReportList rt10 = new ReportList();
                    rt10.Name = "d2";
                    rt10.Type = "String";
                    rt10.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate2);
                    itm.Add(rt10);

                    ReportList rt11 = new ReportList();
                    rt11.Name = "@stidpermit";
                    rt11.Type = "Integer";
                    rt11.Value = SessionUserID;
                    itm.Add(rt11);

                }
                else if (report_no == "report8")
                {
                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@pdid";
                    rt5.Type = "Integer";
                    rt5.Value = model.PdCode.ToString();
                    itm.Add(rt5);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    ReportList rt7 = new ReportList();
                    rt7.Name = "PdName";
                    rt7.Type = "String";
                    rt7.Value = (model.PdName == null ? "(ไม่ระบุ)" : model.PdName);
                    itm.Add(rt7);

                    string fdate2 = Convert.ToString(model.FormDate.AddYears(543).ToString("dd/MM/yyyy"));
                    string tdate2 = Convert.ToString(model.ToDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt9 = new ReportList();
                    rt9.Name = "d1";
                    rt9.Type = "String";
                    rt9.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate2);
                    itm.Add(rt9);

                    ReportList rt10 = new ReportList();
                    rt10.Name = "d2";
                    rt10.Type = "String";
                    rt10.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate2);
                    itm.Add(rt10);

                    ReportList rt11 = new ReportList();
                    rt11.Name = "@stidpermit";
                    rt11.Type = "Integer";
                    rt11.Value = SessionUserID;
                    itm.Add(rt11);
                }
                else if (report_no == "report9" || report_no == "report10")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    string fdate2 = Convert.ToString(model.FormDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt9 = new ReportList();
                    rt9.Name = "d1";
                    rt9.Type = "String";
                    rt9.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate2);
                    itm.Add(rt9);

                    ReportList rt11 = new ReportList();
                    rt11.Name = "@stidpermit";
                    rt11.Type = "Integer";
                    rt11.Value = SessionUserID;
                    itm.Add(rt11);
                }
                else if (report_no == "report12" || report_no == "report13" || report_no == "report14")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "WhName";
                    rt2.Type = "String";
                    rt2.Value = (model.WhName == null ? "(ไม่ระบุ)" : model.WhName);
                    itm.Add(rt2);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    string fdate2 = Convert.ToString(model.FormDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt9 = new ReportList();
                    rt9.Name = "d1";
                    rt9.Type = "String";
                    rt9.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate2);
                    itm.Add(rt9);

                    string fdate1 = Convert.ToString(model.ToDate.AddYears(543).ToString("dd/MM/yyyy"));
                    ReportList rt10 = new ReportList();
                    rt10.Name = "d2";
                    rt10.Type = "String";
                    rt10.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate1);
                    itm.Add(rt10);

                    ReportList rt11 = new ReportList();
                    rt11.Name = "@stidpermit";
                    rt11.Type = "Integer";
                    rt11.Value = SessionUserID;
                    itm.Add(rt11);
                }
                else if(report_no == "report15" || report_no == "report15_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    ReportList rt3 = new ReportList();
                    rt3.Name = "@pdid";
                    rt3.Type = "Integer";
                    rt3.Value = model.PdCode.ToString();
                    itm.Add(rt3);
                }

                else if (report_no == "report16" || report_no == "report16_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    ReportList rt3 = new ReportList();
                    rt3.Name = "@pdid";
                    rt3.Type = "Integer";
                    rt3.Value = model.PdCode.ToString();
                    itm.Add(rt3);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt4 = new ReportList();
                    rt4.Name = "@begdate";
                    rt4.Type = "String";
                    rt4.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt4);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@enddate";
                    rt5.Type = "String";
                    rt5.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt5);
                }

                else if (report_no == "report17" || report_no == "report17_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    ReportList rt3 = new ReportList();
                    rt3.Name = "@pdid";
                    rt3.Type = "Integer";
                    rt3.Value = model.PdCode.ToString();
                    itm.Add(rt3);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt4 = new ReportList();
                    rt4.Name = "@begdate";
                    rt4.Type = "String";
                    rt4.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt4);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@enddate";
                    rt5.Type = "String";
                    rt5.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt5);
                }
                else if (report_no == "report18" || report_no == "report18_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@cardtype";
                    rt5.Type = "Integer";
                    rt5.Value = model.PdCode.ToString();
                    itm.Add(rt5);
                }
                else if (report_no == "report19")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@staffid";
                    rt5.Type = "Integer";
                    rt5.Value = model.StCode.ToString();
                    itm.Add(rt5);
                }
                else if (report_no == "report20" || report_no == "report20_Choose")
                {
                    ReportList rt1 = new ReportList();
                    rt1.Name = "@whid";
                    rt1.Type = "Integer";
                    rt1.Value = model.WhCode.ToString();
                    itm.Add(rt1);

                    ReportList rt2 = new ReportList();
                    rt2.Name = "@stid";
                    rt2.Type = "Integer";
                    rt2.Value = SessionUserID;
                    itm.Add(rt2);

                    string fdate = Convert.ToString(model.FormDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    string tdate = Convert.ToString(model.ToDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    ReportList rt3 = new ReportList();
                    rt3.Name = "@date1";
                    rt3.Type = "String";
                    rt3.Value = (model.FormDate == DateTime.MinValue ? "(ไม่ระบุ)" : fdate);
                    itm.Add(rt3);

                    ReportList rt4 = new ReportList();
                    rt4.Name = "@date2";
                    rt4.Type = "String";
                    rt4.Value = (model.ToDate == DateTime.MinValue ? "(ไม่ระบุ)" : tdate);
                    itm.Add(rt4);

                    ReportList rt5 = new ReportList();
                    rt5.Name = "@cardtype";
                    rt5.Type = "Integer";
                    rt5.Value = model.PdCode.ToString();
                    itm.Add(rt5);
                }
            }
            

            return itm;
        }

        private string CreateXML(Reports model,string reportname)
        {
            Random nameout = new Random();
            string rand = nameout.Next(0, 99999999).ToString("00000000");

            string filename = reportname + "_" + rand.ToString() + ".xml";
            string path = ConfigurationManager.AppSettings.Get("ROOT_XML") + "\\" + filename;

            string server = ConfigurationManager.AppSettings.Get("SERVER") ;
            string database = ConfigurationManager.AppSettings.Get("DATABASE") ;
            string user = ConfigurationManager.AppSettings.Get("USERDB") ;
            string pass = ConfigurationManager.AppSettings.Get("PASSWORDDB") ;

            string returnName = reportname + "_" + rand.ToString();
            string output_ext = ".pdf";

            switch (model.OutputType)
            {
                case output_type.excel: output_ext = ".xls"; break;
                case output_type.pdf: output_ext = ".pdf"; break;
                default: output_ext = ".pdf"; break;
            }
            var itm = GenItem(model, reportname);
            string strname = GetReportName(reportname);
            try
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(path))
                {
                    xmlwriter.WriteStartDocument();
                    xmlwriter.WriteStartElement("ReportCommand");

                    xmlwriter.WriteElementString("Report", strname);
                    xmlwriter.WriteElementString("Output", returnName + output_ext);
                    xmlwriter.WriteElementString("Server", server);
                    xmlwriter.WriteElementString("Database", database);
                    xmlwriter.WriteElementString("User", user);
                    xmlwriter.WriteElementString("Password", pass);
                    xmlwriter.WriteStartElement("Parameters");


                    foreach (var i in itm)
                    {
                        xmlwriter.WriteStartElement("Item");
                        xmlwriter.WriteAttributeString("Name", i.Name);
                        xmlwriter.WriteAttributeString("Type", i.Type);
                        xmlwriter.WriteAttributeString("Value", i.Value);
                        xmlwriter.WriteEndElement();
                   
                    }
                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndElement();
                    xmlwriter.WriteEndDocument();
                    
                }
            }
            catch (Exception ex)
            {
               
                throw ex.InnerException;
            }
            return returnName;
        }

        private string GetReportName(string reportno)
        {
            string reportname = "";
            if(reportno == "report1")
            {
                reportname = "NotEnough.rpt";
            }
            else if (reportno == "report1_Choose")
            {
                reportname = "NotEnough2.rpt";
            }
            else if (reportno == "report2")
            {
                reportname = "Receive.rpt";
            }
            else if (reportno == "report2_Choose")
            {
                reportname = "Receive_Choose.rpt";
            }
            else if (reportno == "report3")
            {
                reportname = "ReceiveMonthly.rpt";
            }
            else if (reportno == "report4")
            {
                reportname = "ReceiveYearly.rpt";
            }
            else if (reportno == "report5")
            {
                reportname = "Requisition.rpt";
            }
            else if (reportno == "report6")
            {
                reportname = "RequisitionMonthly.rpt";
            }
            else if (reportno == "report7")
            {
                reportname = "RequisitionYearly.rpt";
            }
            else if (reportno == "report8")
            {
                reportname = "Movement.rpt";
            }
            else if (reportno == "report9")
            {
                reportname = "Balance.rpt";
            }
            else if (reportno == "report10")
            {
                reportname = "Transfer.rpt";
            }
            else if (reportno == "report11")
            {
                reportname = "ReceiveByOrder.rpt";
            }
            else if (reportno == "report12")
            {
                reportname = "StatusCard.rpt";
            }
            else if (reportno == "report13")
            {
                reportname = "CancelCard.rpt";
            }
            else if (reportno == "report14")
            {
                reportname = "FailCardReport.rpt";
            }
            else if (reportno == "report15")
            {
                reportname = "CardRemain.rpt";
            }
            else if (reportno == "report15_Choose")
            {
                reportname = "CardRemain_Choose.rpt";
            }
            else if (reportno == "report16")
            {
                reportname = "CardTransfer.rpt";
            }
            else if (reportno == "report16_Choose")
            {
                reportname = "CardTransfer_Choose.rpt";
            }
            else if (reportno == "report17")
            {
                reportname = "CardBetweenTransfer.rpt";
            }
            else if (reportno == "report17_Choose")
            {
                reportname = "CardBetweenTransfer_Choose.rpt";
            }
            else if (reportno == "report18")
            {
                reportname = "CardByReg.rpt";
            }
            else if (reportno == "report18_Choose")
            {
                reportname = "CardByReg2.rpt";
            }
            else if (reportno == "report19")
            {
                reportname = "CardByStatus.rpt";
            }
            else if (reportno == "report20")
            {
                reportname = "CardByBranch1.rpt";
            }
            else if (reportno == "report20_Choose")
            {
                reportname = "CardByBranch2.rpt";
            }
            return reportname ;
        }

        private List<string> sortMenu(List<string> menuList)
        {
            //Sort menu in setting
            //Same name in table Module
            List<string> SortMenu = new List<string> { "Report1", "Report2", "Report3", "Report4", "Report5",
                                                        "Report6", "Report7", "Report8", "Report9", "Report10",
                                                        "Report11","Report12","Report13","Report14","Report15",
                                                        "Report16","Report17","Report18","Report19","Report20"};

            var SortedMenu = SortMenu.Intersect(menuList);

            return SortedMenu.ToList();
        }

    }
}
