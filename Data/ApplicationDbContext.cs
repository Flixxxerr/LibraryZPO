using LibraryZPO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryZPO.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<Author>().ToTable("Author");
            modelBuilder.Entity<Genre>().ToTable("Genre");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<BookGenre>().ToTable("BookGenre");

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<BookGenre>(
                    j => j
                        .HasOne(bg => bg.Genre)
                        .WithMany(g => g.BookGenres)
                        .HasForeignKey(bg => bg.GenreId),
                    j => j
                        .HasOne(bg => bg.Book)
                        .WithMany(b => b.BookGenres)
                        .HasForeignKey(bg => bg.BookId),
                    j =>
                    {
                        j.HasKey(bg => new { bg.BookId, bg.GenreId });
                    });
        }
    }
}
