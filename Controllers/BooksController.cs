using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryZPO.Data;
using LibraryZPO.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace LibraryZPO.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexUser(string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["PublisherSortParm"] = sortOrder == "Publisher" ? "publisher_desc" : "Publisher";
            ViewData["CurrentFilter"] = searchString;
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            books = sortOrder switch
            {
                "title_desc" => books.OrderByDescending(b => b.Title),
                "Date" => books.OrderBy(b => b.PublishedAt),
                "date_desc" => books.OrderByDescending(b => b.PublishedAt),
                "Author" => books.OrderBy(b => b.Author.LastName),
                "author_desc" => books.OrderByDescending(b => b.Author.LastName),
                "Genre" => books.OrderBy(b => b.Genre.Name),
                "genre_desc" => books.OrderByDescending(b => b.Genre.Name),
                "Publisher" => books.OrderBy(b => b.Publisher.Name),
                "publisher_desc" => books.OrderByDescending(b => b.Publisher.Name),
                _ => books.OrderBy(b => b.Title),
            };
            return View(await books.ToListAsync());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Rent(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            return View(book);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["PublisherSortParm"] = sortOrder == "Publisher" ? "publisher_desc" : "Publisher";
            ViewData["CurrentFilter"] = searchString;
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            books = sortOrder switch
            {
                "title_desc" => books.OrderByDescending(b => b.Title),
                "Date" => books.OrderBy(b => b.PublishedAt),
                "date_desc" => books.OrderByDescending(b => b.PublishedAt),
                "Author" => books.OrderBy(b => b.Author.LastName),
                "author_desc" => books.OrderByDescending(b => b.Author.LastName),
                "Genre" => books.OrderBy(b => b.Genre.Name),
                "genre_desc" => books.OrderByDescending(b => b.Genre.Name),
                "Publisher" => books.OrderBy(b => b.Publisher.Name),
                "publisher_desc" => books.OrderByDescending(b => b.Publisher.Name),
                _ => books.OrderBy(b => b.Title),
            };
            return View(await books.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateAuthorsDropDownList();
            PopulatePublishersDropDownList();
            PopulateGenresDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,PublishedAt,Pages,Format,AuthorID,PublisherID,GenreID")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            PopulateAuthorsDropDownList(book.AuthorID);
            PopulatePublishersDropDownList(book.PublisherID);
            PopulateGenresDropDownList(book.GenreID);
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            PopulateAuthorsDropDownList(book.AuthorID);
            PopulatePublishersDropDownList(book.PublisherID);
            PopulateGenresDropDownList(book.GenreID);
            return View(book);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
                return NotFound();
            var bookToUpdate = await _context.Books.FirstOrDefaultAsync(s => s.Id == id);
            if (await TryUpdateModelAsync(bookToUpdate, "", s => s.Title, s => s.Description, s => s.PublishedAt, s => s.Pages, s => s.Format, s => s.AuthorID, s => s.PublisherID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists, " + "see your system administrator.");
                }
            }
            PopulateAuthorsDropDownList(bookToUpdate.AuthorID);
            PopulatePublishersDropDownList(bookToUpdate.PublisherID);
            PopulateGenresDropDownList(bookToUpdate.GenreID);
            return View(bookToUpdate);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists " + "see your system administrator.";
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return RedirectToAction(nameof(Index));
            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private void PopulateAuthorsDropDownList(object selectedAuthor = null)
        {
            var authorQuery = from a in _context.Authors
                              orderby a.LastName
                              select a;
            ViewBag.AuthorID = new SelectList(authorQuery.AsNoTracking(), "Id", "FullName", selectedAuthor);
        }

        private void PopulatePublishersDropDownList(object selectedPublisher = null)
        {
            var publisherQuery = from p in _context.Publishers
                              orderby p.Name
                              select p;
            ViewBag.PublisherID = new SelectList(publisherQuery.AsNoTracking(), "Id", "Name", selectedPublisher);
        }

        private void PopulateGenresDropDownList(object selectedGenre = null)
        {
            var genreQuery = from g in _context.Genres
                                 orderby g.Name
                                 select g;
            ViewBag.GenreID = new SelectList(genreQuery.AsNoTracking(), "Id", "Name", selectedGenre);
        }
    }
}
