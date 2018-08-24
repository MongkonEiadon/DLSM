using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class PrintCardDailyReptResponse
    {
        public string rept_date;    // 

        // summary by office
        public StaffSumData[] List_Staff_sum_data; // if sum by staff need 1 record

        public string resultCode;
        public string message;
    }
}