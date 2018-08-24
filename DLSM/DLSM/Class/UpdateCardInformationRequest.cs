using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class UpdateCardInformationRequest
    {
        public string Key { get; set; }
        public string staffId { get; set; }
        public string WH_ID { get; set; }
        public string CardEIN { get; set; }
        public string cardStatus { get; set; } // 1: ดี , 2: เสีย, 3: ไม่พิมพ์  
        public string qrCode { get; set; }
        public string startPrintDttmStr { get; set; }
        public string finishPrintDttmStr { get; set; }
        public string cardCount { get; set; }
        public string saveMDMStatus { get; set; }
        public string saveMDMDesc { get; set; }
        public string prnErrDesc { get; set; }

    }
}