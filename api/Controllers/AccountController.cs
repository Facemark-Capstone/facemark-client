using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Data;
using api.Models.Options;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using shared.Models;
using shared.Models.Account;
using shared.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> mUserManager;
        private readonly ILogger<AccountController> mLogger;
        private readonly SignInManager<User> mSignInManager;
        private readonly FacemarkDbContext mDbContext;
        private readonly IOptionsSnapshot<EncryptionOptions> mOptions;
        private readonly IJwtService mJwt;

        public AccountController(UserManager<User> userManager,
                    ILogger<AccountController> logger,
                    SignInManager<User> signInManager,
                    FacemarkDbContext context,
                    IOptionsSnapshot<EncryptionOptions> options,
                    IJwtService jwt)
        {
            mUserManager = userManager;
            mLogger = logger;
            mSignInManager = signInManager;
            mDbContext = context;
            mOptions = options;
            mJwt = jwt;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest model)
        {
            if (model == null)
                return BadRequest(new { error = true, message = "Bad Request", data = "" });

            if (await UserExistsAsync(model.Email))
                return BadRequest(new { error = true, message = "User Exists", data = "" });

            // Create user
            var result = await CreateUserAsync(model);
            if (!result.Succeeded)
                return BadRequest(new { error = true, message = "User register failed", data = "" });

            //Get user and add to role 
            var user = await mUserManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { error = true, message = "User register failed", data = "" });

            var roleResult = await mUserManager.AddToRoleAsync(user, model.Role.ToString());
            if (!roleResult.Succeeded)
                return BadRequest(new { error = true, message = "Failed to assign to a role", data = "" });

            var roles = await mUserManager.GetRolesAsync(user);
            if (roles == null || roles.Count < 1)
                return BadRequest(new { error = true, message = "Failed to assign to a role", data = "" });

            var token = mJwt.GenerateToken(user.Email, model.Role.ToString());

            return Ok(new AccountResponseModel(user.Id, user.FullName, user.Email, roles[0], token));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
                return BadRequest(new { error = true, message = "Bad request.", data = "" });

            if (!(await UserExistsAsync(model.Email)))
                return BadRequest(new { error = true, message = "User does not exist.", data = "" });

            User user = await mUserManager.FindByEmailAsync(model.Email);

            Microsoft.AspNetCore.Identity.SignInResult result = await mSignInManager.PasswordSignInAsync(
                user.UserName,
                Aes128.DecryptFromUrlSafeBase64String(model.PasswordEncrypted, mOptions.Value.AesEncryptionKey),
                true,
                false);

            if (result.Succeeded)
            {
                var roles = await mUserManager.GetRolesAsync(user);
                if (roles == null || roles.Count < 1)
                    return BadRequest(new { error = true, message = "Login failed.", data = "" });

                var token = mJwt.GenerateToken(user.Email, roles[0]);

                HttpContext.Session.SetString("JWT", token);

                return Ok(new AccountResponseModel(user.Id, user.FullName, user.Email, roles[0], token));
            }
            return BadRequest(new { error = true, message = "Login failed.", data = "" });
        }

        [HttpPost]
        [Route("users")]
        [Authorize(Roles = "Admin, Superadmin")]
        public async Task<IActionResult> Users()
        {
            return Ok(await mUserManager.Users.ToListAsync());
        }

        [HttpPost]
        [Route("users/{userId}")]
        [Authorize(Roles = "Admin, Superadmin")]
        public async Task<IActionResult> Users([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { error = true, message = "Bad Request", data = "" });
            }

            User user = await mUserManager.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest(new { error = true, message = "User doesnt exist.", data = "" });
            }

            return Ok(user);
        }

        [HttpDelete]
        [Route("users/{userId}")]
        [Authorize(Roles = "Admin, Superadmin")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { error = true, message = "Bad Request", data = "" });
            }

            User user = await mUserManager.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest(new { error = true, message = "User doesnt exist.", data = "" });
            }

            var result = await mUserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return StatusCode(500);
        }

        private async Task<bool> UserExistsAsync(string email)
        {
            var user = await mUserManager.FindByEmailAsync(email ?? "1");
            return user != null;
        }

        private async Task<IdentityResult> CreateUserAsync(UserRequest model)
        {
            return await mUserManager.CreateAsync(new User()
            {
                FullName = model.FullName ?? "",
                Email = model.Email,
                UserName = model.Email
            }, Aes128.DecryptFromUrlSafeBase64String(model.PasswordEncrypted, mOptions.Value.AesEncryptionKey));
        }
    }
}
