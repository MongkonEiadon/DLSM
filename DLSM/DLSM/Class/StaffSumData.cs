using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class StaffSumData
    {
        public string staffId;
        public string regisFirstName;
        public string regisLastName;
        public string officeCode;
        public string officeName;
        public string userName;
        public string rept_datetime; 
        public string total_succ;
        public string total_canc;

        public SumCardData[] ListSumCardData; // need for sum by staff
    }
}