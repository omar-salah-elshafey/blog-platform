﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Email;
using UserAuthentication.Models;
using UserAuthenticationApp.Models;

namespace UserAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IOptions<DataProtectionTokenProviderOptions> _tokenProviderOptions;
        private readonly IMapper _mapper;
        public AuthService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            IOptions<DataProtectionTokenProviderOptions> tokenProviderOptions,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _tokenService = tokenService;
            _logger = logger;
            _tokenProviderOptions = tokenProviderOptions;
            _mapper = mapper;
        }

        public async Task<AuthModel> RegisterUserAsync(RegisterUser registerUser, string role)
        {
            //check if user exists
            if (await _userManager.FindByEmailAsync(registerUser.Email) is not null)
                return new AuthModel { Message = "This Email is already used!" };
            if (await _userManager.FindByNameAsync(registerUser.UserName) is not null)
                return new AuthModel { Message = "This UserName is already used!" };

            // Create the new user
            var user = _mapper.Map<ApplicationUser>(registerUser);
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }
            // Assign the user to the specified role
            await _userManager.AddToRoleAsync(user, role);

            //generating the token to verify the user's email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Dynamically get the expiration time from the options
            var expirationTime = _tokenProviderOptions.Value.TokenLifespan.TotalMinutes;

            var authModel = _mapper.Map<AuthModel>(user);
            authModel.Message = $"A verification code has been sent to your Email.{Environment.NewLine}Verify Your Email to be able to login :) ";
            await _emailService.SendEmailAsync(user.Email, "Email Verification Code.",
                $"Hello {user.UserName}, Use this new token to verify your Email: {token}{Environment.NewLine}This code is Valid only for {expirationTime} Minutes.");

            return authModel;
        }

        public async Task<AuthModel> LoginAsync(LoginModel loginModel)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(loginModel.Email); //check if the user exists
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                authModel.IsAuthenticated = false;
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            if (user.IsDeleted)
            {
                authModel.Message = "User Not Found!";
                return authModel;
            }
            if (!user.EmailConfirmed)
                return new AuthModel { Message = "Please Confirm Your Email First." };
            var jwtSecurityToken = await _tokenService.CreateJwtTokenAsync(user);
            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.ExpiresAt = jwtSecurityToken.ValidTo;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Username = user.UserName;
            authModel.IsConfirmed = true;

            //checj if the user already has an active refresh token
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeToken.Token;
                authModel.RefreshTokenExpiresOn = activeToken.ExpiresOn;
            }
            else
            {
                // Generate a new refresh token and add it to the user's tokens
                var refreshToken = await _tokenService.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                // Send the refresh token along with the JWT token
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiresOn = refreshToken.ExpiresOn;
            }

            return authModel;
        }
        
        public async Task<string> AddRoleAsync(AddRoleModel roleModel)
        {
            var user = await _userManager.FindByNameAsync(roleModel.UserName);
            if (user == null)
                return ("Invalid UserName!");
            if (!await _roleManager.RoleExistsAsync(roleModel.Role))
                return ("Invalid Role!");
            if (await _userManager.IsInRoleAsync(user, roleModel.Role))
                return ("User Is already assigned to this role!");
            var result = await _userManager.AddToRoleAsync(user, roleModel.Role);
            return $"User {roleModel.UserName} has been assignd to Role {roleModel.Role} Successfully :)";
        }

        public async Task<List<UserDto>> GetUSersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDto = new List<UserDto>();
            foreach (var user in users)
            {
                if (!user.IsDeleted)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDto.Add(new UserDto
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles.ToList(),
                    });
                }
            }
            return userDto;
        }

        public async Task<AuthModel> DeleteUserAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user is null)
                return new AuthModel { Message = $"User with UserName: {UserName} isn't found!" };
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new AuthModel { Message = $"An Error Occured while Deleting the user{UserName}" };
            return new AuthModel { Message = $"User with UserName: '{UserName}' has been Deleted successfully" };
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            // Revoke the refresh token
            var result = await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            if (!result)
            {
                _logger.LogInformation("Failed to revoke token during logout.");
                return false;
            }

            _logger.LogInformation("User logged out successfully.");
            return true;
        }

        public async Task<UpdateUserModel> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrEmpty(updateUserDto.UserName) 
                || string.IsNullOrEmpty(updateUserDto.FirstName) || string.IsNullOrEmpty(updateUserDto.LastName))
                return new UpdateUserModel { Message = "UserName, FirstName, and LastName are required!" };
            var user = await _userManager.FindByNameAsync(updateUserDto.UserName);
            if (user is null)
                return new UpdateUserModel { Message = $"User with UserName: {updateUserDto.UserName} isn't found!" };
            
            _mapper.Map(updateUserDto, user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return new UpdateUserModel { Message = $"Failed to update user: {errors}" };
            }
            return _mapper.Map<UpdateUserModel>(user);
        }
    }
}
