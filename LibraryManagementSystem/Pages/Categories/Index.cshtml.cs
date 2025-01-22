using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly LibraryContext _context;

        public List<Category> Categories { get; set; }
        public IndexModel(LibraryContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.ToListAsync();
        }
    }
}
