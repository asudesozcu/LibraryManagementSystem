using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementSystem.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(LibraryContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Loan> Loans { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Fetching loans for user: {UserEmail}", User.Identity?.Name);

            if (User.IsInRole("Admin"))
            {
                Loans = await _context.Loans
                    .Include(l => l.book)
                    .Include(l => l.user) // Ensure the user data is loaded
                    .ToListAsync();
                _logger.LogInformation("Admin is viewing all loans. Total loans: {LoanCount}", Loans.Count);
            }
            else
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User session is missing. Redirecting to login.");
                    TempData["ErrorMessage"] = "User session not found. Please log in.";
                    return RedirectToPage("/Users/Login");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                {
                    _logger.LogWarning("User with email '{UserEmail}' not found in database.", userEmail);
                    TempData["ErrorMessage"] = "User not found in the system.";
                    return RedirectToPage("/Users/Login");
                }

                Loans = await _context.Loans
                    .Include(l => l.book)
                    .Include(l => l.user) // Ensure the user data is loaded
                    .Where(l => l.userId == user.UserId)
                    .ToListAsync();

                _logger.LogInformation("User {UserEmail} is viewing their loans. Total loans: {LoanCount}", userEmail, Loans.Count);
            }

            // Log details about loaded loans
            foreach (var loan in Loans)
            {
                _logger.LogInformation("Loan ID: {LoanId}, User: {User}, Book: {Book}, Loan Date: {LoanDate}",
                    loan.LoanId,
                    loan.user != null ? $"{loan.user.FirstName} {loan.user.LastName}" : "Unknown",
                    loan.book != null ? loan.book.Title : "Unknown",
                    loan.LoanDate);
            }

            return Page();
        }
    }
}
