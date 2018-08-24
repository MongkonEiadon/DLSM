using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class GetListPrintTransRequest
    {
        public string Key;
        public string WH_ID;
        public string staffId;

        public string startDateTime;
        public string endDateTime;

        public string productType; // optional if null = all, 0=small car, 1=big car
    }
}