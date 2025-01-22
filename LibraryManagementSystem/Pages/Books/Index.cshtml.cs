using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Pages.Books
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Book> Books { get; set; } = new List<Book>();

        public IndexModel(LibraryContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync(string searchString, int? pageNumber)
        {
            Books = await _context.Books
          .Include(b => b.Category) // Include category if there's a relationship
          .ToListAsync();

            var query = _context.Books.AsQueryable();

            // Search Logic
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            // Pagination Logic
            var pageSize = 10;
            Books = await query.Skip(((pageNumber ?? 1) - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }
    }
}
