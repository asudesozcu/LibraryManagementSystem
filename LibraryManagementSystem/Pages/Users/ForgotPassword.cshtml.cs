using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagementSystem.Pages.Users
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(LibraryContext context, ILogger<ForgotPasswordModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Navigated to Forgot Password page.");
        }

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No account found with the provided email.");
                return Page();
            }

            // Simulate sending a reset link (implement actual email sending logic)
            _logger.LogInformation("Password reset link sent to email: {Email}", Email);

            TempData["Message"] = "Password reset link has been sent to your email.";
            return RedirectToPage("/Index");
        }
    }
    }
