// David Wahid
using System.ComponentModel.DataAnnotations;

namespace shared.Models
{

    public class UserRequest
    {
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordEncrypted { get; set; }
        public EUserRole Role { get; set; }
    }
}
