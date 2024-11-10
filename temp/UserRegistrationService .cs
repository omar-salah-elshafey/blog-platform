//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using UserAuthentication.Data;
//using UserAuthentication.Email;
//using UserAuthentication.Models;
//using UserAuthenticationApp.Models;

//namespace UserAuthentication.Services
//{
//    public class UserRegistrationService: IUserRegistrationService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IEmailService _emailService;
//        private readonly IOptions<DataProtectionTokenProviderOptions> _tokenProviderOptions;

//        public UserRegistrationService(UserManager<ApplicationUser> userManager, IEmailService emailService, IOptions<DataProtectionTokenProviderOptions> tokenProviderOptions)
//        {
//            _userManager = userManager;
//            _emailService = emailService;
//            _tokenProviderOptions = tokenProviderOptions;
//        }

//        private async Task<AuthModel> RegisterUserAsync(RegisterUser registerUser, string role)
//        {
//            // Check if user exists
//            if (await _userManager.FindByEmailAsync(registerUser.Email) is not null)
//                return new AuthModel { Message = "This Email is already used!" };
//            if (await _userManager.FindByNameAsync(registerUser.UserName) is not null)
//                return new AuthModel { Message = "This UserName is already used!" };

//            // Create the new user
//            var user = new ApplicationUser
//            {
//                FirstName = registerUser.FirstName,
//                LastName = registerUser.LastName,
//                UserName = registerUser.UserName,
//                Email = registerUser.Email
//            };
//            var result = await _userManager.CreateAsync(user, registerUser.Password);
//            if (!result.Succeeded)
//            {
//                var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
//                return new AuthModel { Message = errors };
//            }

//            // Assign the user to the specified role
//            await _userManager.AddToRoleAsync(user, role);

//            // Generate the email confirmation token
//            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//            var expirationTime = _tokenProviderOptions.Value.TokenLifespan.TotalMinutes;
//            await _emailService.SendEmailAsync(user.Email, "Email Verification Code",
//                $"Hello {user.UserName}, Use this new token to verify your Email: {token}{Environment.NewLine}This code is valid for {expirationTime} minutes.");

//            return new AuthModel
//            {
//                Email = user.Email,
//                IsAuthenticated = true,
//                Username = user.UserName,
//                Message = $"A verification code has been sent to your Email. Verify your email to be able to login."
//            };
//        }
//    }
//}
