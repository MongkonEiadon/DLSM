using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using System.Configuration;
using TestCallWebApi;

namespace TestCallWebApi
{
    public class AuthenUserRequest
    {
        public string userId;
        public string password;
        public string ipAddress;
    }
    public class AuthenUserResponse
    {
        public Boolean authenUserResult;
        public string authenUserToken;
    }

    public class ApiLoginRequest
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string workStationName { get; set; }
        public string clientVersion { get; set; }
    }

    public class ApiLoginResponse
    {
        public string Key;
        public string WH_ID;
        public string userName;
        public string staffId;
        public string Offname;
        public string regisFirstName; //นายทะเบียน
        public string regisLastName;
        public string regisFirstNameENG;
        public string regisLastNameENG;
        public string titleName;
        public string titleNameENG;
        public string workstationId;
        public string workstationName;
        public string officeCode; //warehousecode
        public string printerName; //
        public string printerIP;
        public string valid_authen; // 0: invalid, 1: valid
        public string authorized; // 0: รถเล็ก, 1: รถใหญ่ 3: both
        public string camaraName;
        public string cameraSerialNo;
        public string androidName;
        public string anroidSerialNo;
        public string signImage;
        public string regisIdNumb;

        public string message;
    }
    public class SaveCardRequest
    {
        public string addrNo { get; set; }
        //alienFlag
        public string ampDesc { get; set; }
        public string ampDescEng { get; set; }
        public string birthDateStr { get; set; }
        public string birthFlag { get; set; }
        //ccFlag
        // DCICode
        // conditionDesc
        public string distDesc { get; set; }
        public string distDescEng { get; set; }
        public string docNo { get; set; }
        public string docType { get; set; }
        public string reqMasRef { get; set; }
        public string expDateStr { get; set; }
        public string fname { get; set; }
        public string fnameEng { get; set; }
        public string issDateStr { get; set; }
        public string issOffLocCode { get; set; }
        public string lane { get; set; }
        public string lname { get; set; }
        public string lnameEng { get; set; }
        public string locFullDesc { get; set; }
        public string message { get; set; }
        public string natDesc { get; set; }
        public string offLocDesc { get; set; }
        public string offLocEngDesc { get; set; }
        public string offRegDesc { get; set; }
        public string offRegEngDesc { get; set; }

        public string pcNo { get; set; }
        public string pltCode { get; set; }
        public string pltDesc { get; set; }
        public string pltEngDesc { get; set; }
        public string pltNo { get; set; }
        public string pltPrnDesc { get; set; }
        public string prevExpDateStr { get; set; }
        public string prevIssDateStr { get; set; }
        public string prevOffLocDesc { get; set; }
        public string prevOffRegDesc { get; set; }
        public string prevOffRegEngDesc { get; set; }
        public string prevPltDesc { get; set; }
        public string prevPltNo { get; set; }

        public string prvCode { get; set; }
        public string prvDesc { get; set; }
        public string prvDescEng { get; set; }
        public string rcpNo { get; set; }
        public string reqDateStr { get; set; }
        public string reqNo { get; set; }
        public string reqTrDesc { get; set; }
        public string sex { get; set; }
        public string soi { get; set; }
        public string street { get; set; }
        public string titleAbrev { get; set; }
        public string titleDesc { get; set; }
        public string titleEngAbrev { get; set; }
        public string villageNo { get; set; }
        public string zipCode { get; set; }

        // SMALLCAR
        public string alienFlag { get; set; }
        public string ccFlag { get; set; }
        public string DCICode { get; set; }
        public string conditionDesc { get; set; }
        public string organDonateFlag { get; set; }
        public string TRSFlag { get; set; }

        // BIGCAR
        public string firstIssueDateStr { get; set; }
        public string pltDescShort { get; set; }
        public string pltNo1 { get; set; }
        public string pltNo2 { get; set; }
        public string prevPltDescShort { get; set; }
        public string prevPltNo1 { get; set; }
        public string prevPltNo2 { get; set; }
        public string pltNoEng { get; set; }



        public string Key; //ไม่เกี่ยว
        public string WH_ID; //ไม่เกี่ยว
        public string staffId;
        public string workstationId;
        public string productType;
        public string CardEIN;
        public string qrCode;
    }
    public class SaveCardResponse
    {
        public string cardEIN;
        public string resultCode;
        public string message;
    }
    public class CheckCardSerialRequest
    {
        public string Key { get; set; }
        public string staffId { get; set; } // key
        public string WH_ID { get; set; }
        public string cardEIN { get; set; }

    }
    public class ApiCheckResponse
    {
        public string cardEIN;
        public string resultCode;  // 0 = error, 1=ถูกต้อง
        public string message;
    }
    public class CheckWarehouseRequest
    {
        public string Key;
        public string WH_ID;
    }
    public class CheckWarehouseResponse
    {
        public string WH_ID;
        public string WhName;
        public WarehouseProducts[] product_remain;
        public string update_datetime;
        public string resultcode;
        public string message;
    }
    public class WarehouseProducts
    {
        public string product_name;
        public string curr_qty;
        public string min_qty;
    }
    public class UploadPictureRequest
    {
        public string Key { get; set; }
        public string StID { get; set; }
        public string WH_ID { get; set; }
        public string cardEIN { get; set; }

        public string Status { get; set; }
        public string ImageSize { get; set; }
        public string PartSize { get; set; }
        public string PartPosition { get; set; }
        public string PartData { get; set; }

        public string person_image { get; set; }
    }
    public class UploadPictureResponse
    {
        public string cardEIN;
        public string resultCode;
        public string message;
    }
    public class PersonImageRequest
    {
        public string citizenId;
    }
    public class PersonImageResponse
    {
        public string citizenId;
        public string person_image;  // เลือกภาพล่าสุด
        public string resultcode;
        public string message;
    }


    class Program
    {
        
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            string[] cardein = new string[] { "600000004171", "600000004172", "600000004173",
                "600000004174", "600000004175", "600000004176", "600000004177",
                "600000004178", "600000004179", "600000004180", "600000004181" };
            string[] citizenit = new string[] { "1549900631543", "1549900631544",
                "1549900631545", "1549900631546", "1549900631547", "1549900631548",
                "1549900631549", "1549900631550", "1549900631551", "1549900631552",
                "1549900631553" };

            for (int i = 0; i < cardein.Length; i++)
            {
                SaveCardRequest prd = new SaveCardRequest();
                prd.workstationId = "1"; //120
                prd.productType = "0";
                prd.CardEIN = cardein[i];//"600000009999"
                prd.qrCode = "80SV607VD13Z9K0S04403901W";
                prd.Key = "";
                prd.WH_ID = "9"; //121
                prd.staffId = "38"; //1219
                prd.addrNo = "77/6";
                prd.ampDesc = "เมือง";
                prd.ampDescEng = "MUANG UDON THANI";
                prd.birthDateStr = "2002 - 02 - 28";
                prd.birthFlag = "0";
                prd.distDesc = "หนองบัว";
                prd.distDescEng = "NONG  BUA";
                prd.docNo = citizenit[i];//"1549900631486"
                prd.docType = "8";
                prd.reqMasRef = "16496118";
                prd.expDateStr = "2020-03-21";
                prd.fname = "อานนท์";
                prd.fnameEng = "ARNON";
                prd.issDateStr = "2018-03-21";
                prd.issOffLocCode = "40200";
                prd.lane = "";
                prd.lname = "พานิชย์";
                prd.lnameEng = "PHANICH";
                prd.locFullDesc = "ตำบลหนองบัว อำเภอเมือง จังหวัดอุดรธานี";
                prd.message = null;
                prd.natDesc = "ไทย";
                prd.offLocDesc = "สำนักงานขนส่งจังหวัดอุดรธานี";
                prd.offLocEngDesc = "UDONTHANI PROVINCIAL LAND TRANSPORT OFFICE";
                prd.offRegDesc = "อุดรธานี";
                prd.offRegEngDesc = "UDONTHANI";
                prd.pcNo = "2002";
                prd.pltCode = "13";
                prd.pltDesc = "รถจักรยานยนต์ส่วนบุคคลชั่วคราว";
                prd.pltEngDesc = "PRIVATE MOTORCYCLE DRIVING LICENCE (TEMPORARY)";
                prd.pltNo = "61000797";
                prd.pltPrnDesc = "รถจักรยานยนต์ส่วนบุคคลชั่วคราว";
                prd.prevExpDateStr = null;
                prd.prevIssDateStr = null;
                prd.prevOffLocDesc = "";
                prd.prevOffRegDesc = "";
                prd.prevOffRegEngDesc = "";
                prd.prevPltDesc = "";
                prd.prevPltNo = "";
                prd.prvCode = "402";
                prd.prvDesc = "อุดรธานี";
                prd.prvDescEng = "UDONTHANI";
                prd.rcpNo = "610009722";
                prd.reqDateStr = "2018-03-15";
                prd.reqNo = "37";
                prd.reqTrDesc = "ออกใบอนุญาตใหม่";
                prd.sex = "1";
                prd.soi = "";
                prd.street = "";
                prd.titleAbrev = "นาย";
                prd.titleDesc = "นาย";
                prd.titleEngAbrev = "MR.";
                prd.villageNo = "5";
                prd.zipCode = "41000";
                prd.alienFlag = "0";
                prd.ccFlag = "Y";
                prd.DCICode = "00";
                prd.conditionDesc = "";
                prd.organDonateFlag = "";
                prd.TRSFlag = "N";
                prd.firstIssueDateStr = null;
                prd.pltDescShort = null;
                prd.pltNo1 = null;
                prd.pltNo2 = null;
                prd.prevPltDescShort = null;
                prd.prevPltNo1 = "";
                prd.prevPltNo2 = "";
                prd.pltNoEng = "";

                string configUrl = System.Configuration.ConfigurationManager.AppSettings["url"];
                //SaveCardResponse oprd = await CallApi<SaveCardResponse>("http://localhost:1920/Api/SaveCard.ashx", prd);
                SaveCardResponse oprd = await CallApi<SaveCardResponse>("http://localhost:1920/Api/SaveCard.ashx", prd);


                Console.WriteLine($" Response Message :{oprd.cardEIN} {oprd.resultCode} {oprd.message}" + "   " + DateTime.Now);



                
            }
            Console.ReadLine();

            //ApiLoginRequest prd = new ApiLoginRequest();
            //prd.userName = "3859900089704";
            //prd.passWord = "@Ble1208";
            //prd.workStationName = "Work2";

            //CheckCardSerialRequest prd = new CheckCardSerialRequest();
            //prd.cardEIN = "100000001070";//"600000004002";
            //prd.Key = null;
            //prd.staffId = "38";
            //prd.WH_ID = "13";

            //CheckWarehouseRequest prd = new CheckWarehouseRequest();
            //prd.WH_ID = "3000";
            //prd.Key = null;

            //ApiLoginRequest prd = new ApiLoginRequest();
            //prd.userName = "Testbydv003";
            //prd.passWord = "12345678";
            //prd.workStationName = "TEST2018-0001";
            //prd.clientVersion = null;

            //UploadPictureRequest prd = new UploadPictureRequest();
            //prd.cardEIN = "600000004120";            
            //prd.person_image = "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";
            //prd.WH_ID = "9";

            //PersonImageRequest prd = new PersonImageRequest();
            //prd.citizenId = "1549900631400";


        }

        static async Task<T> CallApi<T>(string url, object reqData)
        {
            object ret = null;
            var jsonString = JsonConvert.SerializeObject(reqData);

            StringContent content = new StringContent(jsonString);
            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.PostAsync(url, content);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = await resp.Content.ReadAsStreamAsync();
                StreamReader sr = new StreamReader(stream);
                string responseText = sr.ReadToEnd();
                sr.Close();

                try
                {
                    ret = JsonConvert.DeserializeObject<T>(responseText);
                }
                catch
                {
                }
            }

            return (T)ret;
        }

    }
}