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

        public List<Book> Books { get; set; } = new();

        [BindProperty]
        public int BookId { get; set; }

        [BindProperty]
        public int NewStock { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Books = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStockAsync()
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found.");
                return await OnGetAsync();
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
