using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(LibraryContext context, ILogger<DetailsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Fetching details for user ID {UserId}.", id);
            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            return Page();
        }
    }
}
