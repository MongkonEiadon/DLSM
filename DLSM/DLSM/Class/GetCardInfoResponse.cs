using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class GetCardInfoResponse
    {
        public string resultCode;

        public string addrNo { get; set; }
        //alienFlag
        public string ampDesc { get; set; }
        public string ampDescEng { get; set; }
        public string birthDateStr { get; set; }
        public string birthFlag { get; set; }
        //ccFlag
        // DCICode
        // conditionDesc
        public string distDesc { get; set; }
        public string distDescEng { get; set; }
        public string docNo { get; set; }
        public string docType { get; set; }
        public string reqMasRef { get; set; }
        public string expDateStr { get; set; }
        public string fname { get; set; }
        public string fnameEng { get; set; }
        public string issDateStr { get; set; }
        public string issOffLocCode { get; set; }
        public string lane { get; set; }
        public string lname { get; set; }
        public string lnameEng { get; set; }
        public string locFullDesc { get; set; }
        public string message { get; set; }
        public string natDesc { get; set; }
        public string offLocDesc { get; set; }
        public string offLocEngDesc { get; set; }
        public string offRegDesc { get; set; }
        public string offRegEngDesc { get; set; }

        public string pcNo { get; set; }
        public string pltCode { get; set; }
        public string pltDesc { get; set; }
        public string pltEngDesc { get; set; }
        public string pltNo { get; set; }
        public string pltPrnDesc { get; set; }
        public string prevExpDateStr { get; set; }
        public string prevIssDateStr { get; set; }
        public string prevOffLocDesc { get; set; }
        public string prevOffRegDesc { get; set; }
        public string prevOffRegEngDesc { get; set; }
        public string prevPltDesc { get; set; }
        public string prevPltNo { get; set; }

        public string prvCode { get; set; }
        public string prvDesc { get; set; }
        public string prvDescEng { get; set; }
        public string rcpNo { get; set; }
        public string reqDateStr { get; set; }
        public string reqNo { get; set; }
        public string reqTrDesc { get; set; }
        public string sex { get; set; }
        public string soi { get; set; }
        public string street { get; set; }
        public string titleAbrev { get; set; }
        public string titleDesc { get; set; }
        public string titleEngAbrev { get; set; }
        public string villageNo { get; set; }
        public string zipCode { get; set; }

        // SMALLCAR
        public string alienFlag { get; set; }
        public string ccFlag { get; set; }
        public string DCICode { get; set; }
        public string conditionDesc { get; set; }
        public string organDonateFlag { get; set; }
        public string TRSFlag { get; set; }

        // BIGCAR
        public string firstIssueDateStr { get; set; }
        public string pltDescShort { get; set; }
        public string pltNo1 { get; set; }
        public string pltNo2 { get; set; }
        public string prevPltDescShort { get; set; }
        public string prevPltNo1 { get; set; }
        public string prevPltNo2 { get; set; }
        public string pltNoEng { get; set; }

        public string workstationId;
        public string productType;
        public string CardEIN;
        public string qrCode;

        public string cardStatus; // 1: ดี , 2: เสีย, 3: ไม่พิมพ์
        public string startPrintDttmStr;
        public string finishPrintDttmStr;

        public string person_image;

        public string message2;
    }
}