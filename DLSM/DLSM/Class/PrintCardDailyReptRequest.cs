using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class PrintCardDailyReptRequest
    {
        public string WH_ID { get; set; }
        public string staffId { get; set; }
        public string rept_type;  // sum by staff 1 or sum by office 2
        public string rept_date;  // report date is print date
    }
}