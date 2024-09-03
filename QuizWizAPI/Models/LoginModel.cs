using System.ComponentModel.DataAnnotations;

namespace ScrollandScribeAPI.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string adminValidation { get; set; }
    }
}
