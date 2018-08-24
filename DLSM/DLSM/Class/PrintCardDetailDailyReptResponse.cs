using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class PrintCardDetailDailyReptResponse
    {
        public string regisFirstName;
        public string regisLastName;
        public string officeCode;
        public string officeName;
        public string userName;
        public string rept_datetime;
        public string total_succ;
        public string total_canc;
        public string total_fail;

        public PrintCardDetail[] list_print_card_detail;
    }
}