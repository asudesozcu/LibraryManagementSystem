using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int LoanId { get; set; }

        [Required]
       
        public int userId { get; set; }
        [ForeignKey(nameof(userId))]

        public User user { get; set; }

        [Required]
        public int BookId { get; set; }
        [ForeignKey(nameof(BookId))]


        public Book book { get; set; }

      
        [Required]
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }


        public bool IsReturned { get; set; } = false;


    }
}
