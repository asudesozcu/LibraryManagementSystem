using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(LibraryContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {UserId} for deletion.", id);
            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            _logger.LogInformation("Attempting to delete user with ID {UserId}.", id);
            User = await _context.Users.FindAsync(id);

            if (User != null)
            {
                _context.Users.Remove(User);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
            }
            else
            {
                _logger.LogWarning("User with ID {UserId} not found during deletion.", id);
            }

            return RedirectToPage("./Index");
        }
    }
}
