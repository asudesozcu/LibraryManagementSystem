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
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required(ErrorMessage = "The Category field is required.")]
        public int CategoryId { get; set; } 

        public Category? Category { get; set; }

        public int PublicationYear { get; set; }

        public bool IsAvailable { get; set; } = true;


    }
}
