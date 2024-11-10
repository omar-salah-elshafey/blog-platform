//using Microsoft.AspNetCore.Identity;
//using UserAuthentication.Data;
//using UserAuthentication.Models;
//using UserAuthenticationApp.Models;

//namespace UserAuthentication.Services
//{
//    public class UserManagementService: IUserManagementService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public UserManagementService(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task<AuthModel> DeleteUserAsync(string username)
//        {
//            var user = await _userManager.FindByNameAsync(username);
//            if (user == null)
//                return new AuthModel { Message = "User not found!" };

//            var result = await _userManager.DeleteAsync(user);
//            if (!result.Succeeded)
//                return new AuthModel { Message = "Failed to delete user!" };

//            return new AuthModel { Message = "User deleted successfully" };
//        }

//        public async Task<AuthModel> UpdateUserAsync(UpdateUserModel updateUserModel)
//        {
//            var user = await _userManager.FindByNameAsync(updateUserModel.UserName);
//            if (user == null)
//                return new AuthModel { Message = "User not found!" };

//            user.FirstName = updateUserModel.FirstName;
//            user.LastName = updateUserModel.LastName;
//            var result = await _userManager.UpdateAsync(user);
//            if (!result.Succeeded)
//                return new AuthModel { Message = "Failed to update user!" };

//            return new AuthModel { Message = "User updated successfully" };
//        }
//    }
//}
