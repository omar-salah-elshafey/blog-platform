using Microsoft.AspNetCore.Identity;
using UserAuthentication.Data;
using UserAuthentication.Models;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Services
{
    public class PasswordManagementService : IPasswordManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            if (string.IsNullOrEmpty(resetPasswordModel.Email) || string.IsNullOrEmpty(resetPasswordModel.NewPassword))
                return new AuthModel { Message = "Email and Password are required!" };

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                return new AuthModel { Message = "Email is incorrect!" };

            if (!resetPasswordModel.NewPassword.Equals(resetPasswordModel.ConfirmNewPassword))
                return new AuthModel { Message = "Please confirm the new password!" };

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);
            if (!result.Succeeded)
                return new AuthModel { Message = "Invalid token!" };

            return new AuthModel { Message = "Your password has been reset successfully." };
        }

        public async Task<AuthModel> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            if (string.IsNullOrEmpty(changePasswordModel.Email) || string.IsNullOrEmpty(changePasswordModel.CurrentPassword))
                return new AuthModel { Message = "Email and password are required!" };

            var user = await _userManager.FindByEmailAsync(changePasswordModel.Email);
            if (user == null)
                return new AuthModel { Message = "Email is incorrect!" };

            if (changePasswordModel.CurrentPassword.Equals(changePasswordModel.NewPassword))
                return new AuthModel { Message = "New and old password cannot be the same!" };

            if (!changePasswordModel.NewPassword.Equals(changePasswordModel.ConfirmNewPassword))
                return new AuthModel { Message = "Please confirm the new password!" };

            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);
            if (!result.Succeeded)
                return new AuthModel { Message = "An error occurred while changing the password!" };

            return new AuthModel { Message = "Your password has been updated successfully." };
        }
    }
}
