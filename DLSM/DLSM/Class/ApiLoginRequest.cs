using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class ApiLoginRequest
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string workStationName { get; set; }
        public string clientVersion { get; set; }
    }
}