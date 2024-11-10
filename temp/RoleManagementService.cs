//using Microsoft.AspNetCore.Identity;
//using UserAuthentication.Data;
//using UserAuthentication.Models;

//namespace UserAuthentication.Services
//{
//    public class RoleManagementService: IRoleManagementService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;

//        public RoleManagementService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//        }

//        public async Task<string> AddRoleAsync(AddRoleModel roleModel)
//        {
//            var user = await _userManager.FindByNameAsync(roleModel.UserName);
//            if (user == null)
//                return "Invalid UserName!";

//            if (!await _roleManager.RoleExistsAsync(roleModel.Role))
//                return "Invalid Role!";

//            var result = await _userManager.AddToRoleAsync(user, roleModel.Role);
//            return result.Succeeded ? "Role added successfully" : "Failed to add role";
//        }
//    }
//}
