using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Pages.Categories
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
        public Category Category { get; set; }

        public IActionResult OnGet()
        {
            _logger.LogInformation("Navigated to the Create page.");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Log entering the method
            _logger.LogInformation("Entered OnPostAsync method.");

            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                // Log a warning if validation fails
                _logger.LogWarning("Model validation failed.");

                // Log detailed validation errors
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Validation error in field '{state.Key}': {error.ErrorMessage}");
                    }
                }

                return Page(); // Return to the page with validation errors
            }

            // Log the received data for debugging
            _logger.LogInformation($"Category Name received: {Category?.Name}");

            try
            {
                // Add the new category to the database
                _context.Categories.Add(Category);
                await _context.SaveChangesAsync();

                // Log successful save
                _logger.LogInformation($"Category '{Category?.Name}' added successfully.");

                // Redirect to the Index page
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during database operations
                _logger.LogError(ex, "An error occurred while saving the category to the database.");
                // Return the same page with the error message
                ModelState.AddModelError(string.Empty, "An error occurred while saving the category. Please try again.");
                return Page();
            }
        }

    }
}