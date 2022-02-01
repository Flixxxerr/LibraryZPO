using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryZPO.Models
{
    public enum Format
    {
        Paperback,
        CD
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "Published At")]
        [DataType(DataType.Date)]
        public DateTime PublishedAt { get; set; }
        public int Pages { get; set; }
        public Format Format { get; set; }

        public int AuthorID { get; set; }
        public int PublisherID { get; set; }
        public int GenreID { get; set; }

        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public Publisher Publisher { get; set; }
    }
}
