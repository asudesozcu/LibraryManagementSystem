using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagementSystem.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly LibraryContext _context;

        public RegisterModel(LibraryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterViewModel Register { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.Users.Any(u => u.Email == Register.Email))
            {
                ModelState.AddModelError("Register.Email", "Email already exists.");
                return Page();
            }

            var hashedPassword = HashPassword(Register.Password);

            var user = new User
            {
                FirstName = Register.FirstName,
                LastName = Register.LastName,
                Email = Register.Email,
                Password = hashedPassword,
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful. Please login.";
            return RedirectToPage("/Users/Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
