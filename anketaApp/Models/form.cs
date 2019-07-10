namespace anketaApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using anketaApp.Models;
    using System.IO;

    public partial class form
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public form()
        {
            this.question = new HashSet<question>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public int id_state { get; set; }
        public Nullable<System.DateTime> date_start { get; set; }
        public string text_after { get; set; }
        public string text_start { get; set; }
        public string text_before_send { get; set; }


        public int getCountUser()
        {
            anketaEntities db = new anketaEntities();
            int t = db.user.Count(p=>(p.answer_text.Count()>0 && p.answer_text.FirstOrDefault().question.id_form==id) || (p.answer_variant.Count()>0 && p.answer_variant.FirstOrDefault().variant.question.id_form==id) || (p.answer_table.Count()>0 && p.answer_table.FirstOrDefault().table_question.question.id_form==id) || (p.answer_file.Count()>0 && p.answer_file.FirstOrDefault().variant_file.question.id_form==id) || (p.answer_else.Count()>0 && p.answer_else.FirstOrDefault().question_else.question.id_form==id));
            return t;
        }


        public virtual state state { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<question> question { get; set; }
    }
}
