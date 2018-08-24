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

    public partial class Issue
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Issue()
        {
            this.Commends = new HashSet<Commend>();
            this.WorkHistories = new HashSet<WorkHistory>();
        }
    
        public int ID { get; set; }
        public Nullable<int> CreateBy { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [Required(ErrorMessage ="��س����͡������ѭ��")]
        public Nullable<int> TgID { get; set; }
        [Required(ErrorMessage = "��سҡ�͡�������ͧ")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "��سҡ�͡��������´")]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public Nullable<int> TpID { get; set; }
        public string Status { get; set; }
        public Nullable<int> WhID { get; set; }

        public string CreateName { get; set; }
        public string WhName { get; set; }
        public string TpSubject { get; set; }
        public string StatusName { get; set; }
        public string TgName { get; set; }
        public string CommendDescription { get; set; }
        public string StTelNo { get; set; }
        public string WhTelNo { get; set; }

        public List<CommendList> CommendList { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Commend> Commends { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual TopicGroup TopicGroup { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkHistory> WorkHistories { get; set; }
    }
    public class CommendList
    {
        public int ID { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string CreateName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Nullable<int> IsID { get; set; }
    }
}
