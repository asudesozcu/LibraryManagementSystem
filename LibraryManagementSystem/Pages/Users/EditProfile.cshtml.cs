using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Pages.Users
{
    [Authorize(Roles = "User")]
    public class EditProfileModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditProfileModel> _logger;

        public EditProfileModel(LibraryContext context, IWebHostEnvironment environment, ILogger<EditProfileModel> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public User ProfileUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogError("[OnGetAsync] Email claim not found in session.");
                TempData["ErrorMessage"] = "User session not found. Please log in.";
                return RedirectToPage("/Users/Login");
            }

            _logger.LogInformation("[OnGetAsync] Retrieved email: {Email}", userEmail);

            ProfileUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

            if (User == null)
            {
                _logger.LogWarning("[OnGetAsync] No user found with email: {Email}", userEmail);
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Users/Login");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(IFormFile? ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Validation failed. Errors:");

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogError("[OnPostAsync] Email claim not found in session.");
                TempData["ErrorMessage"] = "User session not found. Please log in.";
                return RedirectToPage("/Users/Login");
            }

            _logger.LogInformation("[OnPostAsync] Retrieved email: {Email}", userEmail);

            // Fetch user from the database
            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower());

            if (userInDb == null)
            {
                _logger.LogWarning("[OnPostAsync] No user found with email: {Email}", userEmail);
                TempData["ErrorMessage"] = "User not found.";
                return Page();
            }

            _logger.LogInformation("[OnPostAsync] Updating user: {FirstName} {LastName}, Email: {Email}",
                                   userInDb.FirstName, userInDb.LastName, userInDb.Email);

            // Update user properties
            userInDb.FirstName = ProfileUser.FirstName;
            userInDb.LastName = ProfileUser.LastName;

            // Validate file input
            if (ProfilePicture == null)
            {
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return Page();
            }

            // Validate file size (e.g., max 5 MB)
            if (ProfilePicture.Length > 5 * 1024 * 1024) // 5 MB
            {
                TempData["ErrorMessage"] = "File size cannot exceed 5 MB.";
                return Page();
            }

            // Validate file type (only images: jpg, png)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(ProfilePicture.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type. Only JPG and PNG files are allowed.";
                return Page();
            }

            // Profile picture upload handling
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                _logger.LogInformation("[OnPostAsync] 'uploads' folder created at {Path}.", uploadsFolder);
            }

            // Generate a unique file name for the uploaded picture
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfilePicture.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                // Update user's profile picture path
                userInDb.ProfilePicturePath = $"/uploads/{fileName}";
                _logger.LogInformation("[OnPostAsync] Profile picture uploaded and saved as: {Path}.", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OnPostAsync] Error uploading profile picture.");
                TempData["ErrorMessage"] = "An error occurred while uploading the profile picture.";
                return Page();
            }

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("[OnPostAsync] User profile updated successfully.");
                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OnPostAsync] Error saving changes to the database.");
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                return Page();
            }

            return RedirectToPage("/Users/EditProfile"); // Redirect to a profile page or another appropriate page
        }

    }

}
