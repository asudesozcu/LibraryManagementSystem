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

        public IList<Loan> Loans { get; set; } = new List<Loan>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public async Task<IActionResult> OnGetAsync(int? pageNumber, bool export = false)
        {
            _logger.LogInformation("Fetching loans for user: {UserEmail}", User.Identity?.Name);

            var query = _context.Loans
                .Include(l => l.book)
                .Include(l => l.user)
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin is viewing all loans.");

                // Handle export request
                if (export)
                {
                    var loansToExport = await query.ToListAsync(); // Fetch all loans for export
                    var fileContent = GenerateLoansFile(loansToExport);
                    var fileName = $"Loans_{DateTime.Now:yyyyMMddHHmmss}.csv";

                    _logger.LogInformation("Admin is exporting all loans data to file: {FileName}", fileName);

                    return File(fileContent, "text/csv", fileName);
                }
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

                query = query.Where(l => l.userId == user.UserId);
                _logger.LogInformation("User {UserEmail} is viewing their loans.", userEmail);
            }

            // Pagination logic
            var pageSize = 10;
            CurrentPage = pageNumber ?? 1;
            var totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            Loans = await query.Skip((CurrentPage - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            _logger.LogInformation("Total loans fetched: {LoanCount}", Loans.Count);

            return Page();
        }

        private byte[] GenerateLoansFile(IEnumerable<Loan> loans)
        {
            var csvBuilder = new System.Text.StringBuilder();

            // Add header
            csvBuilder.AppendLine("Loan ID,User,Book,Loan Date,Return Date,Status");

            // Add data rows
            foreach (var loan in loans)
            {
                var user = loan.user != null ? $"{loan.user.FirstName} {loan.user.LastName}" : "Unknown";
                var book = loan.book != null ? loan.book.Title : "Unknown";
                var loanDate = loan.LoanDate.ToString("yyyy-MM-dd");
                var returnDate = loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("yyyy-MM-dd") : "N/A";
                var status = loan.IsReturned ? "Returned" : "Not Returned";

                csvBuilder.AppendLine($"{loan.LoanId},{user},{book},{loanDate},{returnDate},{status}");
            }

            // Convert to byte array
            return System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

    }
}
