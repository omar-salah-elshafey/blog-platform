//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using UserAuthentication.Data;
//using UserAuthenticationApp.Models;

//namespace UserAuthentication.Services
//{
//    public class LogoutService: ILogoutService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public LogoutService(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task<AuthModel> LogoutAsync(string refreshToken)
//        {
//            var user = await _userManager.Users
//                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

//            if (user == null)
//                return new AuthModel { Message = "Invalid token!" };

//            var token = user.RefreshTokens.First(t => t.Token == refreshToken);
//            token.RevokedOn = DateTime.UtcNow;

//            await _userManager.UpdateAsync(user);

//            return new AuthModel { Message = "Logout successful." };
//        }
//    }
//}
