using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Loans
{
    public class EditModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(LibraryContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Loan Loan { get; set; }

        

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Entered OnGetAsync with LoanId: {LoanId}", id);

            Loan = await _context.Loans
                .Include(l => l.book)
                .Include(l => l.user)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (Loan == null)
            {
                _logger.LogWarning("Loan with ID {LoanId} not found.", id);
                TempData["ErrorMessage"] = "Loan not found.";
                return RedirectToPage("./Index");
            }

           

            _logger.LogInformation("Loaded Loan details: {Loan}", Loan);
            return Page();
        }

            public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Entered OnPostAsync for loan return.");
            _logger.LogInformation("Form Data: LoanId={Loan.LoanId}, ReturnDate={Loan.ReturnDate}, IsReturned={Loan.IsReturned}", Loan.LoanId ,Loan.ReturnDate,Loan.IsReturned);

            // ModelState.Remove for navigation properties
            ModelState.Remove("Loan.book");
            ModelState.Remove("Loan.user");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed. Listing all errors:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Field: {FieldName}, Error: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
                }
                TempData["ErrorMessage"] = "There were validation errors. Please correct them.";
                return Page();
            }

            var loan = await _context.Loans
                .Include(l => l.book)
                .Include(l => l.user)
                .FirstOrDefaultAsync(l => l.LoanId == Loan.LoanId);

            if (loan == null)
            {
                _logger.LogWarning("Loan with ID {LoanId} not found in database.", Loan.LoanId);
                TempData["ErrorMessage"] = "Loan not found.";
                return RedirectToPage("./Index");
            }

            // Log fetched loan data for debug purposes
            _logger.LogInformation("Loan found: {LoanId}, Book: {BookTitle}, User: {UserName}", loan.LoanId, loan.book?.Title, loan.user?.FirstName);

            // Update loan properties
            loan.ReturnDate = DateTime.UtcNow;
            loan.IsReturned = true;

            // Update book's AvailableStock if book exists
            if (loan.book != null)
            {
                loan.book.AvailableStock++;
                _logger.LogInformation("Updated book stock for BookId: {BookId}, New AvailableStock: {Stock}", loan.book.BookId, loan.book.AvailableStock);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Loan returned successfully.";
                _logger.LogInformation("Loan return processed successfully for LoanId: {LoanId}.", Loan.LoanId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving changes to database: {ErrorMessage}", ex.Message);
                TempData["ErrorMessage"] = "An error occurred while saving the loan return.";
                return Page();
            }

            return RedirectToPage("./Index");
        }

    }
}
