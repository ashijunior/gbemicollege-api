using System.ComponentModel.DataAnnotations;

namespace gbemi.Models
{
    public class LoginDTO
    {
        [Key]

        [Required(ErrorMessage = "User Name is required!")]
        [StringLength(100)]
        public string UserName { get; set; }

        public string Password { get; set; }

    }
}
