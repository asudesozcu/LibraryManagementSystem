using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(LibraryContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Navigated to Create page.");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Attempting to create a new user.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed.");
                return Page();
            }

            // Hash the password before saving
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully.");
            return RedirectToPage("./Index");
        }
    }
}
