using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class CheckCardSerialRequest
    {
        public string Key { get; set; }
        public string staffId { get; set; } // key
        public string WH_ID { get; set; }
        public string cardEIN { get; set; }

    }
}