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
    
    public partial class answer_file
    {
        public int id { get; set; }
        public int variant_file_id { get; set; }
        public string link { get; set; }
        public int user_id { get; set; }
    
        public virtual user user { get; set; }
        public virtual variant_file variant_file { get; set; }
    }
}
