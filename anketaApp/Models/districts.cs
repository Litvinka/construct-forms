//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace anketaApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class districts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public districts()
        {
            this.schools = new HashSet<schools>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int id_area { get; set; }
    
        public virtual areas areas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<schools> schools { get; set; }
    }
}
