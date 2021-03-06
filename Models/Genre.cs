using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryZPO.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Display(Name = "Genre")]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
