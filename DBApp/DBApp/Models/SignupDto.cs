
using System.ComponentModel.DataAnnotations;

namespace DBApp.Models
{
    public class SignupDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Role { get; set; }
    }
}