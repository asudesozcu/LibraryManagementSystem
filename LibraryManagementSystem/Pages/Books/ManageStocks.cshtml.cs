using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Books
{
    [Authorize(Roles = "Admin")] // Sadece admin erişebilir
    public class ManageStockModel : PageModel
    {
        private readonly LibraryContext _context;

        public ManageStockModel(LibraryContext context)
        {
            _context = context;
        }


        [BindProperty]
        public int BookId { get; set; }

        [BindProperty]
        public int NewStock { get; set; }

        public IList<Book> Books { get; set; } = new List<Book>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int? pageNumber)
        {
            var query = _context.Books
                .Include(b => b.Category) // Include category relationships if needed
                .AsQueryable();

            // Pagination logic
            int pageSize = 5; // Number of items per page
            CurrentPage = pageNumber ?? 1; // Default to page 1 if no page number is provided
            var totalItems = await query.CountAsync(); // Total number of books
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // Calculate total pages

            // Fetch books for the current page
            Books = await query.Skip((CurrentPage - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStockAsync()
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found.");
                return Page();
            }

            // Mevcut stok ve toplam stok güncellemesi
            int stockDifference = NewStock - book.Stock;

            book.Stock = NewStock;

            // Eğer AvailableStock, toplam stoktan küçükse, fark kadar artırılır
            if (stockDifference > 0)
            {
                book.AvailableStock += stockDifference;
            }
            // Eğer stok düşüyorsa, AvailableStock mevcut stoğun altına düşmeyecek şekilde ayarlanır
            else if (stockDifference < 0)
            {
                book.AvailableStock = Math.Max(book.AvailableStock + stockDifference, 0);
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Stock updated for book: {book.Title}. Available stock adjusted to: {book.AvailableStock}";
            return RedirectToPage("./ManageStocks");
        }

    }
}
