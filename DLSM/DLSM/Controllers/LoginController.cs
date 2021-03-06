﻿using DLSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using AutoMapper;
using DLSM.Infrastructure.API.MdmServices.Models;
using DLSM.Infrastructure.API.MdmUserServices.Interfaces;
using DLSM.Infrastructure.MdmUserServices;
using DLSM.Infrastructure.Models;

namespace DLSM.Controllers
{
    public class LoginController : Controller
    {
        private readonly IMdmServiceWrapper _mdmServiceWrapper;
        private readonly IMapper _mapper;

        public String uid = ConfigurationManager.AppSettings.Get("uid");
        public String upw = ConfigurationManager.AppSettings.Get("upw");
        public String ip = ConfigurationManager.AppSettings.Get("ip");
        public String authencode = "";
        public String message = "";
        public String passold = "";

        public LoginController(IMdmServiceWrapper mdmServiceWrapper, IMapper mapper) {
            _mdmServiceWrapper = mdmServiceWrapper;
            _mapper = mapper;
        }
        
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autherize(DLSM.Models.Staff UserModel)
        {
            using (DLSMEntities db = new DLSMEntities())
            {
                passold = UserModel.UserPassword;
                string pd = Hash(UserModel.UserPassword);

                
                var UserDetail = db.Staffs.Where(x => x.UserLogin == UserModel.UserLogin).FirstOrDefault();
            
                var UgIDConfig = db.Configures.Where(x => x.Code == "ADMIN").Select(x => x.Value).ToList();

                var CodeConfig = ConfigurationManager.AppSettings.Get("CodeConfig").ToString();

                //DateTime mydate = new DateTime(2018,5,31,0,0,0);
                //DateTime today = DateTime.Now;
                //int result = DateTime.Compare( today,mydate);
                //if(result < 0 ) 
                //{
                    if(UserDetail == null)
                    {
                        UserModel.LoginErrorMessage = "ชื่อผู้ใช้ หรือ รหัสผ่าน ไม่ถูกต้อง";
                        return View("Index",UserModel);
                    }
                    else
                    {
                        if(CodeConfig == "1" && UserDetail.UgID.ToString() != UgIDConfig[0].ToString()) //authen with MdmService
                        {
                            authencode = MdmAuthen(UserDetail);
                            if(authencode == "0")
                            {
                                UserModel.LoginErrorMessage = message;
                                return View("Index", UserModel);
                            }
                        }
                        else
                        {
                            UserDetail = db.Staffs.Where(x => x.UserLogin == UserModel.UserLogin && x.UserPassword == pd).FirstOrDefault();
                            if (UserDetail == null)
                            {
                                UserModel.LoginErrorMessage = "ชื่อผู้ใช้ หรือ รหัสผ่าน ไม่ถูกต้อง";
                                return View("Index", UserModel);
                            }
                        }
                        var WhAccess = db.sp_GetMainWarehouse(Convert.ToInt32(UserDetail.ID)).ToList();
                    var a = WhAccess[0].ID;
                        var UserPermis = db.StaffWarehouses.Where(x => x.StID == UserDetail.ID).FirstOrDefault();
                        var UserWh = db.Warehouses.SingleOrDefault(x => x.ID == a);
                        //var UserWh = db.Warehouses.SingleOrDefault(x => x.ID == UserPermis.WhID);
                        var UserGroup = db.UserGroups.SingleOrDefault(x => x.ID == UserDetail.UgID);

                        var MenuList = (from p in db.Permissions
                                          join m in db.Modules on p.MdCode equals m.Code
                                          where p.UgID == UserDetail.UgID
                                          select m.Name).ToList();

                        var MenuAccess = sortMenu(MenuList);
                        
                        
                        var WarehouseAccess = db.StaffWarehouses.Where(w => w.StID == UserDetail.ID).Select(w => w.WhID).ToList();
                        var WarehouseIsManger = db.StaffWarehouses.Where(w => w.StID == UserDetail.ID).Select(s => s).ToList();

                        Session["UserID"] = UserDetail.ID;
                        Session["UserLogin"] = UserDetail.UserLogin;
                        Session["UserName"] = UserDetail.Name;
                        Session["IsManager"] = UserPermis.IsManager;
                        Session["UserWhID"] = UserPermis.WhID;
                        Session["UserWhName"] = UserWh.Name;
                        Session["UserWhTelNo"] = UserWh.TelNo;
                        Session["UserUgID"] = UserDetail.UgID;
                        Session["UserGroupName"] = UserGroup.Name;
                        Session["UserTelNo"] = UserDetail.TelNo;
                        Session["MenuAccess"] = MenuAccess;
                        Session["WarehouseAccess"] = WarehouseAccess.ToList();
                        Session["WarehouseIsManger"] = WarehouseIsManger.ToList();

                        string to_action = "";

                        if(UserDetail.UgID == 1)    
                        {
                            //admin
                            to_action = "Setting";
                        }else if (UserDetail.UgID == 4)
                        {
                            //helpdesk
                            to_action = "HelpDesks";
                        }
                        else if (UserDetail.UgID == 2 || UserDetail.UgID == 5 || UserDetail.UgID == 6)
                        {
                            to_action = "Stocks";
                        }
                        else
                        {
                            to_action = "Home";
                        }
                    
                        return RedirectToAction("Index", to_action);
                    }
                //}
                //return RedirectToAction("Index", "Login");
            }
                
        }

        private List<string> sortMenu(List<string> menuList)
        {
            //Sort menu in navbar
            //Same name in table Module
            List<string> SortMenu = new List<string> { "Transactions", "Stocks", "Report", "HelpDesks", "Setting","WorkStations" };
            
            var SortedMenu = SortMenu.Intersect(menuList);

            return SortedMenu.ToList();
        }

        public ActionResult PleaseLogin()
        {
            ViewBag.TimeOut = "Your session has timed out, Please login again";
            return View("Index");
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index","Login");
        }
        //เข้ารหัสพาสเวิด
        public string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hasBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hasBytes);
        }

        protected String MdmAuthen(Staff UserDetail) {

            var userInfo = _mdmServiceWrapper.GetUserInfoAsync( 
                new MdmAuthenticationInput(UserDetail.UserLogin, UserDetail.UserPassword)).Result;

            if (userInfo != null)
            {
                using (var _db = new DLSMEntities()) {
                    var storeUserInfo = _mapper.Map<GetUserInfo>(userInfo);
                    _db.GetUserInfoes.Add(storeUserInfo);
                    _db.SaveChanges();
                }
            }

            string retcode = null;
            using (DLSMEntities db = new DLSMEntities())
            {

                try
                {

                    var respget = _mdmServiceWrapper
                        .GetUserInfoAsync(new MdmAuthenticationInput(UserDetail.UserLogin, passold)).Result;

                    var gui = _mapper.Map<GetUserInfo>(respget);

                    db.GetUserInfoes.Add(gui);
                    db.SaveChanges();

                    retcode = "1";

                }
                catch (Exception ex)
                {
                    retcode = "0";
                    message = "authenUser Result: Error กรุณาติดต่อเจ้าหน้าที่";


                }
            }

            return retcode;
        }

    }
}