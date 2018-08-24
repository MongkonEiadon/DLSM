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

    public partial class Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Document()
        {
            this.DocumentDetails = new HashSet<DocumentDetail>();
        }
    
        public int ID { get; set; }
        [Required(ErrorMessage ="��سҡ�͡�Ţ����͡���")]
        public string DocNo { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> DocDate { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string DocType { get; set; }
        [Required(ErrorMessage = "��س����͡�ӹѡ�ҹ����")]
        public Nullable<int> WhID { get; set; }
        public Nullable<int> SpID { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<int> ApproveBy { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> ProcessDate { get; set; }
        public Nullable<int> ProcessBy { get; set; }
        public Nullable<int> ToWhID { get; set; }

        public List<DocDetailList> DocDetailList { get; set; }

        public virtual Product Products { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public virtual DocumentDetail DocumentDetail { get; set; }
        public Nullable<int> SubType { get; set; }
        public Nullable<int> PdID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentDetail> DocumentDetails { get; set; }

        public string CreateName { get; set; }
        public string WarehouseName { get; set; }
        public string ToWarehouseName { get; set; }
        public string WhName { get; set; }
        public string ProductName { get; set; }
        public int? Qty { get; set; }
        public string SupplierName { get; set; }
        public string StatusName { get; set; }

    }

    public class DocDetailList
    {
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

        public string IpProperty { get; set; }
        public string ProductName { get; set; }
        public string IsAsset { get; set; }
        public string SerialControl { get; set; }
        
    }

}
