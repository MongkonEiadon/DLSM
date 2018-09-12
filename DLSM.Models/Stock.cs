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
    using System.ComponentModel.DataAnnotations;

    public partial class Stock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock()
        {
            this.StockSerials = new HashSet<StockSerial>();
        }
    
        public int WhID { get; set; }
        public int PdID { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###0}")]
        public Nullable<int> Qty { get; set; }
        public Nullable<int> Borrow { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###0}")]
        public Nullable<int> Transfer { get; set; }
        public string CheckStatus { get; set; }
    
        public string ProductName { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###0}")]
        public int? MinStock { get; set; }
        public string ProductCode { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,###0}")]
        public int SerialCount { get; set; }
       
        public string WhName { get; set; }
        public string SerialBegin { get; set; }
        public string SerialEnd { get; set; }
        public string IpProperty { get; set; }
        public string IsAsset { get; set; }

        public decimal? Predict { get; set; }

        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockSerial> StockSerials { get; set; }
    }
}
