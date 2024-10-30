using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Email;
using UserAuthentication.Models;
using UserAuthentication.Services;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public readonly IAuthService _authService;
        public readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController
            (UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IAuthService authService, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _authService = authService;
            _tokenService = tokenService;
        }
        //[Authorize(Roles ="Admin")]
        [HttpPost("register-reader")]
        public async Task<IActionResult> RegisterReaderAsync([FromBody]RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterReaderAsync(registerUser);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.Email,
                result.Username,
                result.Message,
                //result.Roles,
                result.IsAuthenticated,
                result.IsConfirmed
            });
        }

        [HttpPost("register-author")]
        public async Task<IActionResult> RegisterAutherAsync([FromBody] RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAutherAsync(registerUser);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.Email,
                result.Username,
                result.Message,
                //result.Roles,
                result.IsAuthenticated,
                result.IsConfirmed
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAdminAsync(registerUser);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.Email,
                result.Username,
                result.Message,
                //result.Roles,
                result.IsAuthenticated,
                result.IsConfirmed
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.LoginAsync(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresOn);
            }
            SetUserIdCookie(user.Id);
            SetUserNameCookie(user.UserName);
            return Ok(result.Token);
        }
        
        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _tokenService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresOn);
            return Ok(new
            {
                result.Token,
                result.ExpiresAt,
                result.RefreshToken,
                result.RefreshTokenExpiresOn,
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ResetPasswordAsync(resetPasswordModel);
            return Ok(result.Message);
        }
        
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ChangePasswordAsync(changePasswordModel);
            return Ok(result.Message);
        }

        [HttpPost("add-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoleAsync(AddRoleModel roleModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.FindByNameAsync(roleModel.UserName);
            var result = await _authService.AddRoleAsync(roleModel);

            return Ok(result);
        }

        [HttpGet("get-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _authService.GetUSersAsync();
            if (users == null || !users.Any())
                return NotFound("No users found!");
            return Ok(users);
        }

        [HttpDelete("delete-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsync(string UserName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.DeleteUserAsync(UserName);
            return Ok(result.Message);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var userId = Request.Cookies["userId"];
            var userName = Request.Cookies["UserName"];
            var result = await _authService.LogoutAsync(refreshToken);
            if (!result)
                return BadRequest(result);

            RemoveRefreshTokenCookie(refreshToken);
            RemoveUserIdCookie(userId);
            RemoveUserNameCookie(userName);
            return Ok(new { message = "Successfully logged out" });
        }

        [HttpPost("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.UpdateUserAsync(updateUserDto);
            return Ok(result);
        }

        private void SetRefreshTokenCookie(string refreshToken, DateTime ex)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = ex.ToLocalTime(),
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void SetUserIdCookie(string userId)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("userID", userId, cookieOptions);
        }

        private void SetUserNameCookie(string userName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("userName", userName, cookieOptions);
        }

        private void RemoveRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1).ToLocalTime(),
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("refreshToken", "", cookieOptions);
        }

        private void RemoveUserIdCookie(string userId)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1).ToLocalTime(),
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("userID", "", cookieOptions);
        }

        private void RemoveUserNameCookie(string userName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1).ToLocalTime(),
                Secure = true,    // Set this in production when using HTTPS
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("userName", "", cookieOptions);
        }
    }
}
