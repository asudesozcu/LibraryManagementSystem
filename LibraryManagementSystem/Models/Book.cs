using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The Title field must not exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The Author field must not exceed 100 characters.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "The Category field is required.")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [Required(ErrorMessage = "The Stock field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; } // Total Stock

        [Required(ErrorMessage = "The Available Stock field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available Stock cannot be negative.")]
        public int AvailableStock { get; set; } // Current Available Stock

        public int PublicationYear { get; set; }

        // Computed property to determine if the book is available for loan
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsAvailable { get; private set; } = false;


        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
