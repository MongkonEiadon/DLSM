using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DLSM.Class
{
    public class Reports
    {
        public int WhCode { get; set; }
        public string WhName { get; set; }
        public int PdCode { get; set; }
        public string PdName { get; set; }
        public int StCode { get; set; }
        public string StName { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime ToDate { get; set; }

       
    }
    public class ReportList
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    
    }

    public class ListR
    {
        public List<ReportList> ListRR { get; set; }
    }

}