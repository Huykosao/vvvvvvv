//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ManageStudentsV2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hoc_sinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hoc_sinh()
        {
            this.Diems = new HashSet<Diem>();
            this.Lop_dang_ky = new HashSet<Lop_dang_ky>();
        }
    
        public int ma_sinh_vien { get; set; }
        public string ten_sinh_vien { get; set; }
        public int ma_lop { get; set; }
        public Nullable<int> UserID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Diem> Diems { get; set; }
        public virtual Lop_chinh Lop_chinh { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lop_dang_ky> Lop_dang_ky { get; set; }
        public virtual User User { get; set; }
    }
}
