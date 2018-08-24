using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class ApiCheckResponse
    {
        public string cardEIN;
        public string resultCode;  // 0 = error, 1=ถูกต้อง
        public string message;
    }
}