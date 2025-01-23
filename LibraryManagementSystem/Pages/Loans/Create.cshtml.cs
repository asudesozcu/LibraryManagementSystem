using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Loans
{
    public class CreateModel : PageModel
    {
        private readonly LibraryContext _context;

        public CreateModel(LibraryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Loan Loan { get; set; }

        public List<Book> AvailableBooks { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Sadece ödünç alınabilir kitapları göster
            AvailableBooks = await _context.Books
                .Where(b => b.IsAvailable)
                .ToListAsync();

            if (AvailableBooks.Count == 0)
            {
                TempData["ErrorMessage"] = "Ödünç alınabilir kitap bulunamadı.";
                return RedirectToPage("/Loans/Index");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Tekrar mevcut kitapları dolduralım
                AvailableBooks = await _context.Books
                    .Where(b => b.IsAvailable)
                    .ToListAsync();
                return Page();
            }

            // Kullanıcı bilgisini oturumdan al
            var userEmail = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bilgisi alınamadı.");
                return Page();
            }

            // Loan kaydını oluştur
            Loan.userId = user.UserId;
            Loan.LoanDate = DateTime.UtcNow; // Başlangıç tarihi şimdi
            Loan.IsReturned = false; // Varsayılan olarak iade edilmedi
            _context.Loans.Add(Loan);

            // Kitabın durumu güncelle
            var book = await _context.Books.FindAsync(Loan.BookId);
            if (book != null)
            {
                book.IsAvailable = false; // Kitap artık ödünç alındı
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Loans/Index");
        }
    }
}
