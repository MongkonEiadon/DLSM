//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DLSM.Models
{
    using System;
    
    public partial class sp_ApiPrintDailyRept_Result
    {
        public int staffid { get; set; }
        public string regisFirstName { get; set; }
        public string regisLastName { get; set; }
        public string officeCode { get; set; }
        public string officeName { get; set; }
        public string userName { get; set; }
        public Nullable<int> total_succ { get; set; }
        public Nullable<int> total_canc { get; set; }
        public string card_type_desc { get; set; }
        public Nullable<int> total_succ_card { get; set; }
        public Nullable<int> total_fail_card { get; set; }
        public Nullable<int> total_canc_card { get; set; }
    }
}
