using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class UpdatePrintStatusRequest
    {
        public string Key { get; set; }
        public int StID { get; set; }
        public int WhID { get; set; }
        public string SerialNo { get; set; }
       
        public string Status { get; set; }
        public string PrinterStatus { get; set; }
    }
}