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
        public DateTime PublishedAt { get; set; }
        [Display(Name = "Price")]
        public decimal RetailPrice { get; set; }
        public int Pages { get; set; }
        public Format Format { get; set; }

        public Author Author { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public List<BookGenre> BookGenres { get; set; }
        public Publisher Publisher { get; set; }
    }
}
