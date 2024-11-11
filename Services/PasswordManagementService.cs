using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserAuthentication.Data;
using UserAuthentication.Email;
using UserAuthentication.Models;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Services
{
    public class PasswordManagementService : IPasswordManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IOptions<DataProtectionTokenProviderOptions> _tokenProviderOptions;

        public PasswordManagementService(UserManager<ApplicationUser> userManager, IEmailService emailService, IOptions<DataProtectionTokenProviderOptions> tokenProviderOptions)
        {
            _userManager = userManager;
            _emailService = emailService;
            _tokenProviderOptions = tokenProviderOptions;
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

        public async Task<AuthModel> ResetPasswordRequestAsync(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return new AuthModel { Message = "The Email you Provided is not Correct!" };
            //generating the token to verify the user's email
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Dynamically get the expiration time from the options
            var expirationTime = _tokenProviderOptions.Value.TokenLifespan.TotalMinutes;

            await _emailService.SendEmailAsync(email, "Password Reset Code.",
                $"Hello {user.UserName}, Use this new token to Reset your Password: {token}\n This code is Valid only for {expirationTime} Minutes.");
            return new AuthModel { Message = "A Password Reset Code has been sent to your Email!" };
        }

        public async Task<AuthModel> VerifyResetPasswordRequestAsync(ConfirmEmailModel verifyREsetPassword)
        {

            if (string.IsNullOrEmpty(verifyREsetPassword.Email) || string.IsNullOrEmpty(verifyREsetPassword.Token))
                return new AuthModel { ISPasswordResetRequestVerified = false, Message = "UserName and token are required." };

            var user = await _userManager.FindByEmailAsync(verifyREsetPassword.Email);
            if (user == null)
                return new AuthModel { ISPasswordResetRequestVerified = false, Message = "User not found." };
            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", verifyREsetPassword.Token);
            if (!result)
                return new AuthModel { ISPasswordResetRequestVerified = false, Message = "Token is not valid!" };

            return new AuthModel { ISPasswordResetRequestVerified = true, Message = "Your Password reset request is verified." };
        }
    }
}
