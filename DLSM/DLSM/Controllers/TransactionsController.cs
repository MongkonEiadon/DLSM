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

namespace DLSM.Controllers
{
    [SessionCheck]
    public class TransactionsController : Controller
    {
        private DLSMEntities db = new DLSMEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            var UgID = int.Parse(Session["UserUgID"].ToString());
            var MenuList = (from p in db.Permissions
                            join m in db.Modules on p.MdCode equals m.Code
                            where p.UgID == UgID
                            select m.Name).ToList();

            var MenuSettingAccess = sortMenu(MenuList);

            ViewBag.MenuSettingAccess = MenuSettingAccess;

            return View();
        }

        private List<string> sortMenu(List<string> menuList)
        {
            //Sort menu in setting
            //Same name in table Module
            List<string> SortMenu = new List<string> { "Receives", "Transfers", "Requisitions", "Borrows", "CheckStocks","FailCard"};

            var SortedMenu = SortMenu.Intersect(menuList);

            return SortedMenu.ToList();
        }
    }
}