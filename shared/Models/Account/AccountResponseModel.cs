// David Wahid
using System;
using System.ComponentModel.DataAnnotations;

namespace shared.Models.Account
{
    public class AccountResponseModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        public string Token { get; set; }

        [Obsolete("Used only for model binding.")]
        public AccountResponseModel() { }

        public AccountResponseModel(string userId, string fullName, string email, string role, string token)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            Role = role;
            Token = token;
        }
    }
}
