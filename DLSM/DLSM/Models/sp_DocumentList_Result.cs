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
    
    public partial class sp_DocumentList_Result
    {
        public int ID { get; set; }
        public string DocNo { get; set; }
        public Nullable<System.DateTime> DocDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string CreateName { get; set; }
        public Nullable<int> WhID { get; set; }
        public string WarehouseName { get; set; }
        public int SpID { get; set; }
        public string ProductName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<int> ApproveBy { get; set; }
        public Nullable<System.DateTime> ProcessDate { get; set; }
        public Nullable<int> ProcessBy { get; set; }
        public Nullable<int> Qty { get; set; }
    }
}
