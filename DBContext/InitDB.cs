using Microsoft.EntityFrameworkCore;
using MMX4.WebAPI.Authorization;
using MMX4.WebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMX4.WebAPI.DBContext
{
    public interface IAccountManager
    {

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<Tuple<bool, string[]>> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> CreateUserAsync(ApplicationUser user, IEnumerable<string> roles, string password);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(ApplicationRole role);
        Task<Tuple<bool, string[]>> DeleteRoleAsync(string roleName);
        Task<Tuple<bool, string[]>> DeleteUserAsync(ApplicationUser user);
        Task<Tuple<bool, string[]>> DeleteUserAsync(string userId);
        Task<ApplicationRole> GetRoleByIdAsync(string roleId);
        Task<ApplicationRole> GetRoleByNameAsync(string roleName);
        Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName);
        Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<Tuple<ApplicationUser, string[]>> GetUserAndRolesAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<List<Tuple<ApplicationUser, string[]>>> GetUsersAndRolesAsync(int page, int pageSize);
        Task<Tuple<bool, string[]>> ResetPasswordAsync(ApplicationUser user, string newPassword);
        Task<bool> TestCanDeleteRoleAsync(string roleId);
        Task<bool> TestCanDeleteUserAsync(string userId);
        Task<Tuple<bool, string[]>> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<Tuple<bool, string[]>> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
        Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user);
        Task<Tuple<bool, string[]>> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<DownloadURL> GetURL();
        Task<bool> UpdateURL(string value);
    }
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }




    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager)
        {
            _accountManager = accountManager;
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            if (!await _context.Users.AnyAsync())
            {

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                await CreateUserAsync("admin", "tempP@ss123", "Inbuilt Administrator", "admin@admin.com", "+1 (123) 000-0000", new string[] { adminRoleName });
                await CreateUserAsync("user", "tempP@ss123", "Inbuilt Standard User", "user@admin.com", "+1 (123) 000-0001", new string[] { userRoleName });
                
            }
            
        }



        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var result = await this._accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Item1)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            var result = await _accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Item1)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Item2)}");


            return applicationUser;
        }
    }
}
