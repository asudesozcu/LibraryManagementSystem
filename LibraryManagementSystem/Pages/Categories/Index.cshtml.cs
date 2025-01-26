using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Pages.Categories
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly LibraryContext _context;
        private readonly ILogger<IndexModel> _logger;
       public IList<Category> Categories { get; set; } = new List<Category>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public IndexModel(LibraryContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task OnGetAsync(int? pageNumber)
        {
            var query = _context.Categories.AsQueryable();

            // Pagination logic
            int pageSize = 10; // Number of items per page
            CurrentPage = pageNumber ?? 1; // Default to the first page if no page is specified
            var totalItems = await query.CountAsync(); // Total number of categories
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // Calculate total pages

            // Get the categories for the current page
            Categories = await query.Skip((CurrentPage - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
        }
    }
}
