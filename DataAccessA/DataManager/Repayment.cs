//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccessA.DataManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class Repayment
    {
        public int ID { get; set; }
        public string Reference { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<int> LedgerFlag { get; set; }
        public System.DateTime Created { get; set; }
    }
}
