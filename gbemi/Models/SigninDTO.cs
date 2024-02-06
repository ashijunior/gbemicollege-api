using System.ComponentModel.DataAnnotations;

namespace gbemi.Models
{
    public class SigninDTO
    {
        [Key]

        [Required(ErrorMessage = "First Name is required!")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required!")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User Name is required!")]
        [StringLength(100)]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Student Email is required!, please input correct Email.")]
        public string Email { get; set; }

        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }
    }
}
