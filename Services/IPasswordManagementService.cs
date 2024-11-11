using UserAuthentication.Models;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Services
{
    public interface IPasswordManagementService
    {
        Task<AuthModel> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
        Task<AuthModel> ChangePasswordAsync(ChangePasswordModel changePasswordModel);
        Task<AuthModel> ResetPasswordRequestAsync(string email);
        Task<AuthModel> VerifyResetPasswordRequestAsync(ConfirmEmailModel verifyREsetPassword);
    }
}
