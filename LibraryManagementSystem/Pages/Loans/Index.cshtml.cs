using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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
            // Kullanıcı rolünü al
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Eğer kullanıcı Admin ise tüm Loan kayıtlarını getir
            if (userRole == "Admin")
            {
                Loans = await _context.Loans
                    .Include(l => l.book)
                    .Include(l => l.user)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                // Kullanıcı e-posta adresini al
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError("", "User session not found. Please log in.");
                    return Page();
                }


                // Kullanıcıyı e-posta ile arıyoruz
                

                // Kullanıcının yalnızca kendi Loan kayıtlarını al
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

              

                Loans = await _context.Loans
                    .Include(l => l.book)
                    .Where(l => l.userId == user.UserId)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return Page();
        }
    }
}

 

