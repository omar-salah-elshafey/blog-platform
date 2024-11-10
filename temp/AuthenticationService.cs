//using Microsoft.AspNetCore.Identity;
//using System.IdentityModel.Tokens.Jwt;
//using UserAuthentication.Data;
//using UserAuthenticationApp.Models;

//namespace UserAuthentication.Services
//{
//    public class AuthenticationService: IAuthenticationService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly ITokenService _tokenService;

//        public AuthenticationService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
//        {
//            _userManager = userManager;
//            _tokenService = tokenService;
//        }

//        public async Task<AuthModel> LoginAsync(LoginModel loginModel)
//        {
//            var authModel = new AuthModel();
//            var user = await _userManager.FindByEmailAsync(loginModel.Email); // check if the user exists
//            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
//            {
//                authModel.IsAuthenticated = false;
//                authModel.Message = "Email or Password is incorrect!";
//                return authModel;
//            }

//            if (!user.EmailConfirmed)
//                return new AuthModel { Message = "Please confirm your email first." };

//            var jwtSecurityToken = await _tokenService.CreateJwtTokenAsync(user);
//            authModel.IsAuthenticated = true;
//            authModel.Email = user.Email;
//            authModel.ExpiresAt = jwtSecurityToken.ValidTo;
//            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
//            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
//            authModel.Username = user.UserName;
//            authModel.IsConfirmed = true;

//            // Check if the user already has an active refresh token
//            var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
//            if (activeToken != null)
//            {
//                authModel.RefreshToken = activeToken.Token;
//                authModel.RefreshTokenExpiresOn = activeToken.ExpiresOn;
//            }
//            else
//            {
//                var refreshToken = await _tokenService.GenerateRefreshToken();
//                user.RefreshTokens.Add(refreshToken);
//                await _userManager.UpdateAsync(user);
//                authModel.RefreshToken = refreshToken.Token;
//                authModel.RefreshTokenExpiresOn = refreshToken.ExpiresOn;
//            }

//            return authModel;
//        }
//    }
//}
