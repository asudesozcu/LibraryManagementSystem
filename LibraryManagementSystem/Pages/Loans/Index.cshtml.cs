using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Loans
{
    public class IndexModel : PageModel
    {
        private readonly LibraryContext _context;

        public IndexModel(LibraryContext context)
        {
            _context = context;
        }

        public List<Loan> Loans { get; set; }

        public async Task OnGetAsync()
        {
            Loans = await _context.Loans
                .Include(l => l.user)
                .Include(l => l.book)
                .ToListAsync();
        }
    }
}
