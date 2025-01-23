using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> OnGetAsync()
        {
            Loans = await _context.Loans
                .Include(l => l.book)
                .Include(l => l.user)
                .ToListAsync();

            return Page();
        }
    }
}

 

