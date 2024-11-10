using System.IdentityModel.Tokens.Jwt;
using UserAuthentication.Data;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Services
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<RefreshToken> GenerateRefreshToken();
    }
}
