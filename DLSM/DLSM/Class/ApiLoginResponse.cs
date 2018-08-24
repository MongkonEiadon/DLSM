using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
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
}