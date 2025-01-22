using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Pages.Users
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
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Fetching user with ID {UserId} for editing.", id);
            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Attempting to update user with ID {UserId}.", User.UserId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for user ID {UserId}.", User.UserId);
                return Page();
            }

            var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == User.UserId);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", User.UserId);
                return NotFound();
            }

            // Check if the password has changed
            if (existingUser.Password != User.Password)
            {
                _logger.LogInformation("Password updated for user ID {UserId}. Hashing new password.", User.UserId);
                User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);
            }

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} updated successfully.", User.UserId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == User.UserId))
                {
                    _logger.LogWarning("User with ID {UserId} no longer exists.", User.UserId);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency issue occurred while updating user ID: {UserId}.", User.UserId);
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
