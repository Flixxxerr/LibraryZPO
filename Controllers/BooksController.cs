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

namespace LibraryZPO.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .AsNoTracking();
            return View(await books.ToListAsync());
        }

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

        public IActionResult Create()
        {
            PopulatePublishersDropDownList();
            PopulateAuthorsDropDownList();
            PopulateGenresDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
