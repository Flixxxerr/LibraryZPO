using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryZPO.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Author")]
        public string FullName
        { 
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public ICollection<Book> Books { get; set; }
    }
}
