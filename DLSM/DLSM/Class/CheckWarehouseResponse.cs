using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class CheckWarehouseResponse
    {
        public string WH_ID;
        public string WhName;
        public WarehouseProducts[] product_remain;
        public string update_datetime;
        public string resultcode;
        public string message;
    }
}