using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Pages.Books
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
        public Book Book { get; set; }

        public SelectList Categories { get; set; }

        public IActionResult OnGet()
        {
            Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            return Page();
        }




        public async Task<IActionResult> OnPostAsync()
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

                Categories = new SelectList(_context.Categories, "CategoryId", "Name");
                Console.WriteLine($"CategoryId: {Book.CategoryId}");
               
                _context.Books.Add(Book);
                await _context.SaveChangesAsync();
                return Redirect("/Books/Index");
              

            }



            _context.Books.Add(Book);
            await _context.SaveChangesAsync();
            return Redirect("/Books/Index");

        }


    }
}
