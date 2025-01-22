using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public SelectList Users { get; set; }
        public SelectList Books { get; set; }

        public void OnGet()
        {
            Users = new SelectList(_context.Users, "UserId", "Email");
            Books = new SelectList(_context.Books, "BookId", "Title");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Users = new SelectList(_context.Users, "UserId", "Email");
                Books = new SelectList(_context.Books, "BookId", "Title");
                return Page();
            }

            _context.Loans.Add(Loan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
