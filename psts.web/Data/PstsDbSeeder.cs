using Microsoft.AspNetCore.Identity;
using Psts.Web.Data;

namespace psts.web.Data
{
    public class PstsDbSeeder
    // AMS ***** This class will verify required roles and at least 1 admin exists.  Ensure bare minimum setup in the case of a fresh install or DB
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PstsDbContext _db;
        private readonly ILogger _logger;

        public PstsDbSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, PstsDbContext db, ILogger<PstsDbSeeder> logger )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        public async Task SeedDbAsync()
        {
            bool rolesCreated = true;

            // AMS - Verify Admin role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                       _logger.LogError("Role 'Admin' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Manager role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Manager"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Manager' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Employee role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Employee"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Employee' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Clerk role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Clerk"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Clerk"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Clerk' failed: {Error}", e.Description);
                }
            }

            // AMS - Verify Pending role exists, if not create it
            if (!await _roleManager.RoleExistsAsync("Pending"))
            {
                var status = await _roleManager.CreateAsync(new IdentityRole("Pending"));
                if (!status.Succeeded)
                {
                    rolesCreated = true;
                    foreach (var e in status.Errors)
                        _logger.LogError("Role 'Pending' failed: {Error}", e.Description);
                }
            }

            if (!rolesCreated)
            {
                _logger.LogError("Could not create required user roles. APPLICATION MAY NOT FUNCTION CORRECTLY.");
            }

            // AMS if no admins exist create the default and set to require password change at next login.
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (!admins.Any())
            {
                // AMS - Setup new admin user settings
                var initialAdmin = new AppUser 
                { 
                    UserName = "admin",
                    Email = "admin@psts.local",
                    EmailConfirmed = true,
                    ResetPassOnLogin = true,
                    LoginPassAllowed = true,
                    OIDCAllowed = false
                };
        
                // AMS - Verify admin username is not in use
                if (await _userManager.FindByNameAsync(initialAdmin.UserName) == null)
                {
                    // AMS - Create new admin user
                    string randPassword = $"{Guid.NewGuid():N}".Substring(0, 12) + "aA1!";
                    var result = await _userManager.CreateAsync(initialAdmin, randPassword);
                    // AMS - Take success/fail of Admin user creation, assign admin role, and log an appropriate output.
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create initial admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(initialAdmin, "Admin");
                        _logger.LogCritical(@"
                            ==================================================
                             INITIAL ADMIN ACCOUNT CREATED
                             USERNAME: admin
                             PASSWORD: {Password}
                            ==================================================
                            ", randPassword);
                    }
                }
            }
        }
    }
}
