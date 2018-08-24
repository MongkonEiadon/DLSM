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
    
    public partial class StockSerial
    {
        public int ID { get; set; }
        public Nullable<int> WhID { get; set; }
        public Nullable<int> PdID { get; set; }
        public string SerialBegin { get; set; }
        public string SerialEnd { get; set; }
        public Nullable<int> SerialCount { get; set; }
        public Nullable<int> DocID { get; set; }
        public Nullable<int> SeqNo { get; set; }
        public string IpProperty { get; set; }
        public string Name { get; set; }
    
        public virtual DocumentDetail DocumentDetail { get; set; }
        public virtual Stock Stock { get; set; }

        public List<CardList> CardList { get; set; }
    }

    public class CardList
    {
        public int ID { get; set; }
        public int WhID { get; set; }
        public string CardSerial { get; set; }
        public string CardDate { get; set; }
        public string Remark { get; set; }
    }
}
