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

        public EditModel(LibraryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Loan Loan { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Loan = await _context.Loans
                .Include(l => l.book)
                .Include(l => l.user)
                .FirstOrDefaultAsync(l => l.LoanId == id);

            if (Loan == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loan = await _context.Loans
                .Include(l => l.book)
                .FirstOrDefaultAsync(l => l.LoanId == Loan.LoanId);

            if (loan == null)
            {
                return NotFound();
            }

            // Kitap iade edildi olarak işaretle
            loan.IsReturned = true;
            loan.ReturnDate = DateTime.UtcNow;

            // Kitabı tekrar müsait hale getir
            loan.book.IsAvailable = true;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
