using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Fiction" },
                new Category { CategoryId = 2, Name = "Non-Fiction" }
            );

            // Seed data for books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "Test Book",
                    Author = "Test Author",
                    CategoryId = 1,
                    PublicationYear = 2023,
                    IsAvailable = true
                }
            );

            // Configure relationships for Loan
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.user)
                .WithMany()
                .HasForeignKey(l => l.userId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.book)
                .WithMany()
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
