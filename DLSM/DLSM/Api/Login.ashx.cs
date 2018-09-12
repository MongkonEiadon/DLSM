using DLSM.Class;
using DLSM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Web.Http;
using DLSM.Infrastructure.API.MdmUserServices.Interfaces;
using DLSM.Infrastructure.Models;

namespace DLSM.Api
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    public class Login : IHttpHandler
    {
        private readonly IMdmServiceWrapper _mdmServiceWrapper;
        private DLSMEntities db = new DLSMEntities();
        public String uid = ConfigurationManager.AppSettings.Get("uid");
        public String upw = ConfigurationManager.AppSettings.Get("upw");
        public String ip = ConfigurationManager.AppSettings.Get("ip");
        public String CodeConfig = ConfigurationManager.AppSettings.Get("CodeConfig").ToString();

        public Login(IMdmServiceWrapper mdmServiceWrapper)
        {
            _mdmServiceWrapper = mdmServiceWrapper;
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                ApiLoginRequest parm = new ApiLoginRequest();
                using (StreamReader sr = new StreamReader(context.Request.InputStream))
                {
                    String data = sr.ReadToEnd();
                    parm = new JavaScriptSerializer().Deserialize<ApiLoginRequest>(data);
                }

                ApiLoginResponse ap = new ApiLoginResponse();
                using (DLSMEntities db = new DLSMEntities())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var passold = parm.passWord;
                            var bytes = new UTF8Encoding().GetBytes(parm.passWord);
                            var hasBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
                            var hashpass = Convert.ToBase64String(hasBytes);
                            parm.passWord = hashpass;

                            if (CodeConfig == "1")
                            {
                                _mdmServiceWrapper.AuthenticationUserAsync(new MdmAuthenticationInput())

                                DLSM.MdmServiceTest.MdmUserServiceClient soap = new DLSM.MdmServiceTest.MdmUserServiceClient();

                                try
                                {
                                    MdmServiceTest.authenUser client = new MdmServiceTest.authenUser();

                                    authenUserBean bean = new authenUserBean();
                                    bean.userId = parm.userName;
                                    bean.password = passold;
                                    bean.ipAddress = ip;

                                    //bean.userId = "3859900089704";
                                    //bean.password = "@Ble1208";

                                    AuthenticationInput input = new AuthenticationInput();
                                    input.userId = uid;
                                    input.password = upw;

                                    AuthenUserInput aut = new AuthenUserInput();
                                    aut.authenticationInput = input;
                                    aut.authenUserBeanInput = bean;

                                    authenUser au = new authenUser();
                                    au.AuthenUserInput = aut;

                                    authenUserResponse resp = soap.authenUser(au);
                                    if (resp.AuthenUserOutput.authenUserResponse.@return.authenUserResult.ToString() == "True")
                                    {
                                        try
                                        {
                                            MdmServiceTest.getUserInfo clientget = new MdmServiceTest.getUserInfo();

                                            getUserInfoBean beanget = new getUserInfoBean();
                                            beanget.authenUserToken = resp.AuthenUserOutput.authenUserResponse.@return.authenUserToken;

                                            GetUserInfoInput inputget = new GetUserInfoInput();
                                            inputget.getUserInfoBeanInput = beanget;
                                            inputget.authenticationInput = input;

                                            clientget.GetUserInfoInput = inputget;

                                            getUserInfoResponse respget = soap.getUserInfo(clientget);
                                            if (respget.GetUserInfoOutput.getUserInfoResponse.@return.name.ToString() != "")
                                            {
                                                try
                                                {
                                                    GetUserInfo gui = new GetUserInfo();
                                                    gui.Title = respget.GetUserInfoOutput.getUserInfoResponse.@return.title;
                                                    gui.Name = respget.GetUserInfoOutput.getUserInfoResponse.@return.name;
                                                    gui.Surname = respget.GetUserInfoOutput.getUserInfoResponse.@return.surname;
                                                    gui.OffLocCode = respget.GetUserInfoOutput.getUserInfoResponse.@return.offLocCode;
                                                    gui.OffLocDesc = respget.GetUserInfoOutput.getUserInfoResponse.@return.offLocDesc;
                                                    gui.OrgFullNameDes = respget.GetUserInfoOutput.getUserInfoResponse.@return.orgFullNameDes;
                                                    gui.PositionDesc = respget.GetUserInfoOutput.getUserInfoResponse.@return.positionDesc;
                                                    db.GetUserInfoes.Add(gui);
                                                    db.SaveChanges();

                                                }
                                                catch (Exception ex)
                                                {
                                                    dbContextTransaction.Rollback();
                                                    ap.valid_authen = "0";
                                                    ap.message = "GetUserInfo Error";
                                                }

                                            }
                                            else
                                            {
                                                ap.valid_authen = "0";
                                                ap.message = "getUserInfo Error";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ap.valid_authen = "0";
                                            ap.message = "authenUser Error";
                                        }
                                    }
                                    else
                                    {
                                        ap.valid_authen = "0";
                                        ap.message = resp.AuthenUserOutput.authenUserResponse.@return.authenUserResult.ToString();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    ap.valid_authen = "0";
                                    ap.message = "authenUser Error";
                                }
                            }
                            //else
                            //{
                            //    //ไม่วิ่งผ่าน mdmservice
                            //    parm.passWord = null;
                            //}
                        }
                        finally
                        {
                            try
                            {
                                var result = db.sp_ApiLogin(parm.userName, parm.passWord, parm.workStationName).ToList();
                                if (result.Count() > 0)
                                {

                                    ap.WH_ID = "" + result[0].WH_ID;
                                    ap.userName = result[0].userName;
                                    ap.staffId = "" + result[0].staffId;
                                    ap.Offname = result[0].OffName;
                                    ap.regisIdNumb = result[0].regisIdNumb;
                                    ap.regisFirstName = result[0].regisFirstName;
                                    ap.regisLastName = result[0].regisLastName;
                                    ap.regisFirstNameENG = result[0].regisFirstNameENG;
                                    ap.regisLastNameENG = result[0].regisLastNameENG;
                                    ap.titleName = result[0].titleName;
                                    ap.titleNameENG = result[0].titleNameENG;
                                    ap.workstationId = "" + result[0].workstationId;
                                    ap.workstationName = result[0].workstationName;
                                    ap.officeCode = result[0].officeCode;
                                    ap.printerName = result[0].printerName;
                                    ap.printerIP = result[0].printerIP;
                                    ap.valid_authen = "1";
                                    ap.authorized = "" + result[0].authorized;
                                    ap.camaraName = result[0].camaraName;
                                    ap.cameraSerialNo = result[0].cameraSerialNo;
                                    ap.androidName = result[0].androidName;
                                    ap.anroidSerialNo = result[0].anroidSerialNo;
                                    ap.signImage = result[0].signImage;
                                    ap.message = "OK";

                                    dbContextTransaction.Commit();
                                }
                                else
                                {
                                    dbContextTransaction.Rollback();
                                    ap.valid_authen = "0";
                                    ap.message = "not found";
                                }
                            }
                            catch (Exception ex)
                            {
                                    dbContextTransaction.Rollback();
                                    ap.valid_authen = "0";
                                    ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: Login catch 2" : ex.Message) : ex.InnerException.Message ;
                            }
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(ap);

                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }
            catch(Exception ex)
            {
                ApiLoginResponse ap = new ApiLoginResponse();
                ap.valid_authen = "0";
                ap.message = ex.InnerException == null ? (ex.Message == null ? "Error: Login catch 1" : ex.Message) : ex.InnerException.Message ;

                string json = new JavaScriptSerializer().Serialize(ap);
                context.Response.ContentType = "text/javascript";
                context.Response.Write(json);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}