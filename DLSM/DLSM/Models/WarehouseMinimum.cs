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
    using System.Collections.Generic;
    
    public partial class WarehouseMinimum
    {
        public int ID { get; set; }
        public int WhID { get; set; }
        public int PdID { get; set; }
        public Nullable<int> MinStock { get; set; }
        public Nullable<int> PredictMonth { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
