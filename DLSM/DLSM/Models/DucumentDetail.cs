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
    
    public partial class DucumentDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DucumentDetail()
        {
            this.SerialOuts = new HashSet<SerialOut>();
            this.StockSerials = new HashSet<StockSerial>();
        }
    
        public int DocID { get; set; }
        public int SeqNo { get; set; }
        public Nullable<int> PdID { get; set; }
        public string TrnType { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> BF { get; set; }
        public Nullable<int> CF { get; set; }
        public string SerialBegin { get; set; }
        public string SerialEnd { get; set; }
        public Nullable<int> RemainQty { get; set; }
    
        public virtual Document Document { get; set; }
        public virtual Product Product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SerialOut> SerialOuts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockSerial> StockSerials { get; set; }

        public string ProductName { get; set; }
    }
}
