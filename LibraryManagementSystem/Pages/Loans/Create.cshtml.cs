using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Pages.Loans
{
    public class CreateModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(LibraryContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Loan Loan { get; set; }

        public List<SelectListItem> AvailableBooks { get; set; }  // Book yerine SelectListItem kullanın

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);  // E-posta adresini alıyoruz
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "User session not found. Please log in.";
                return RedirectToPage("/Users/Login");
            }

            // Kullanıcıyı e-posta ile arıyoruz
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == userEmail && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("User with email '{UserEmail}' not found or inactive.", userEmail);
                TempData["ErrorMessage"] = "User not found in the system.";
                return RedirectToPage("/Users/Login");
            }

            ViewData["UserName"] = $"{user.FirstName} {user.LastName}";

            // Populate available books as SelectListItem
            AvailableBooks = await _context.Books
                .Where(b => b.IsAvailable)
                .Select(b => new SelectListItem
                {
                    Value = b.BookId.ToString(),
                    Text = b.Title
                })
                .ToListAsync();

            if (!AvailableBooks.Any())
            {
                TempData["InfoMessage"] = "No books are currently available for loan.";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync started.");

            // Kullanıcının oturum bilgilerini al
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogError("User email not found in claims. Redirecting to login page."); // Log: Kullanıcı oturumu hatası
                ModelState.AddModelError("", "User session not found. Please log in.");
                await PopulateAvailableBooks();
                return Page();
            }

            _logger.LogInformation("User email retrieved: {UserEmail}", userEmail); // Log: E-posta bulundu

            // Kullanıcıyı e-posta ile arıyoruz
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {UserEmail}", userEmail);
                ModelState.AddModelError("UserId", "User is required.");
                await PopulateAvailableBooks();
                return Page();
            }

            // Kullanıcıyı Loan objesine bağla
            Loan.userId = user.UserId;

            // Kitap ID doğrulamasını yap
            if (Loan.BookId == 0)
            {
                _logger.LogWarning("BookId is required but was not provided.");
                ModelState.AddModelError("BookId", "Book selection is required.");
                await PopulateAvailableBooks();
                return Page();
            }

            // Kitabı veri tabanından bul
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == Loan.BookId);

            if (book == null)
            {
                _logger.LogWarning("Book not found for BookId: {BookId}", Loan.BookId);
                ModelState.AddModelError("BookId", "Selected book could not be found.");
                await PopulateAvailableBooks();
                return Page();
            }

            // Loan bilgilerini tamamla ve kaydet
            Loan.LoanDate = DateTime.UtcNow;
            Loan.IsReturned = false;
            book.IsAvailable = false;

            _context.Loans.Add(Loan);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Loan created successfully with LoanId: {LoanId}", Loan.LoanId);
            return RedirectToPage("./Index");
        }




        private async Task PopulateAvailableBooks()
        {
            AvailableBooks = await _context.Books
                                           .Where(b => b.IsAvailable)
                                           .Select(b => new SelectListItem
                                           {
                                               Value = b.BookId.ToString(),
                                               Text = b.Title
                                           }).ToListAsync();
        }

    }

}


