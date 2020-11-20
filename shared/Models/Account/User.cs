// David Wahid
using Microsoft.AspNetCore.Identity;
namespace shared.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
