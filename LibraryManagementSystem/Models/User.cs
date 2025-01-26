using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255)]
        [RegularExpression(@"^(?=.*[A-Z]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter and be at least 6 characters long.")]

        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Loan> Loans { get; set; }

        public string? ProfilePicturePath { get; set; }
    }
}
