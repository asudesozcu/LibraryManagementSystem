using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LibraryManagementSystem.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<CreateModel> _logger;

        public LoginModel(LibraryContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }
       

        [BindProperty]
        public LoginViewModel Login { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // This method is for initial GET request to display the login form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Entered OnPostAsync method");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed");
                return Page();
            }

            var hashedInputPassword = HashPassword(Login.Password); // Hash the input password
            _logger.LogInformation("password: {hashedInputPassword}", hashedInputPassword);
            var user = _context.Users
                .SingleOrDefault(u => u.Email == Login.Email && u.Password == hashedInputPassword);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", Login.Email);
                ModelState.AddModelError("", "Invalid email or password.");
                return Page();
            }

            _logger.LogInformation("Successful login for user: {Email}", user.Email);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect based on user role
            if (user.Role == "Admin")
            {
                return RedirectToPage("/Books/Index");
            }

            return RedirectToPage("/Loan/Index");
        }

        // Password hashing method
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }



        public IActionResult OnPostForgotPassword()
        {
            // Redirect to the Forgot Password page
            return RedirectToPage("/Users/ForgotPassword");
        }

        public IActionResult OnPostCreateNewAccount()
        {
            // Redirect to the Register page
            return RedirectToPage("/Users/Register");
        }
       
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
