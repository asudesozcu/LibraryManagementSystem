using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Books
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
        public Book Book { get; set; }

        public SelectList Categories { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Navigated to Edit page for Book ID: {BookId}", id);

            Book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);

            if (Book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found", id);
                return NotFound();
            }

            Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name", Book.CategoryId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Entered OnPostAsync method");
            _logger.LogInformation("Book object before validation: {@Book}", Book);

            // Log the CategoryId coming from the form
            _logger.LogInformation("CategoryId from form: {CategoryId}", Book?.CategoryId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for Book ID: {BookId}", Book?.BookId);

                // Log each validation error
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                }

                // Repopulate the Categories dropdown
                Categories = new SelectList(_context.Categories, "CategoryId", "Name", Book?.CategoryId);
                _logger.LogInformation("Repopulated Categories dropdown. Selected CategoryId: {SelectedCategoryId}", Book?.CategoryId);

                return Page();
            }

            try
            {
                _logger.LogInformation("Attempting to update Book ID: {BookId}", Book.BookId);

                // Attach the book and set its state to Modified
                _context.Attach(Book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated Book ID: {BookId}", Book.BookId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_context.Books.Any(b => b.BookId == Book.BookId))
                {
                    _logger.LogError("Book with ID {BookId} no longer exists. Concurrency issue.", Book.BookId);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency issue occurred while updating Book ID: {BookId}", Book.BookId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating Book ID: {BookId}", Book.BookId);
                throw;
            }

            return RedirectToPage("./Index");
        }





    }
}
