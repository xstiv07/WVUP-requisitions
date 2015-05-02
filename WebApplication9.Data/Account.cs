//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication9.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            this.Requisitions = new HashSet<Requisition>();
        }
    
        public int Id { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> FundId { get; set; }
        public string String { get; set; }
        public string Name { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public WebApplication9.Data.Helpers.ConfigureStatusEnum Status { get; set; }
    
        public virtual Division Division { get; set; }
        public virtual Fund Fund { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Requisition> Requisitions { get; set; }
        public virtual Department Department { get; set; }
    }
}
