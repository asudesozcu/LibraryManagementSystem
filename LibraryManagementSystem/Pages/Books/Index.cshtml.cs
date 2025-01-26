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
        public string CurrentFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public IndexModel(LibraryContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync(string searchString, int? pageNumber)
        {
            CurrentFilter = searchString;

            var query = _context.Books
                .Include(b => b.Category) // Include related category
                .AsQueryable();

            // Search Logic
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            // Pagination Logic
            var pageSize = 10;
            CurrentPage = pageNumber ?? 1;
            var totalItems = await query.CountAsync(); // Total number of items
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            Books = await query.Skip((CurrentPage - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        }
    }
}
