using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using psts.web.Domain.Enums;
using Psts.Web.Data;
using System.Linq.Expressions;

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

        private static readonly Dictionary<SystemSettings, string> DefaultSettings = new()
        {
            {SystemSettings.MakeOIDCAvailable, "true" },
            {SystemSettings.OIDCEnabledByDefault, "false" },
            {SystemSettings.DisableAccountAfterXDaysStale, "0" },
            {SystemSettings.ManagerApprovalForAdjustments, "false" },
            {SystemSettings.MaxDaysInPastForEntry, "14" },
            {SystemSettings.MaxDaysInFutureForEntry, "0" },
            {SystemSettings.MaxHoursByEmployeePerDay, "8" }
        };


        public async Task SeedDbAsync()
        {
            
            // ********** CREATE ROLES **********
            
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

            
            // ********** CREATE DEFAULT ADMIN IF NEEDED **********
            
            // AMS if no admins exist create the default and set to require password change at next login.
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (!admins.Any())
            {
                // AMS - Setup new admin user settings
                
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
                try 
                { 
                    var initialAdmin = new AppUser 
                    { 
                        UserName = "admin",
                        Email = "admin@psts.local",
                        EmailConfirmed = true,
                        ResetPassOnLogin = true,
                        LoginPassAllowed = true,
                        OIDCAllowed = false
                    };
        
                    // AMS - Create new admin user
                    string randPassword = $"{Guid.NewGuid():N}".Substring(0, 12) + "aA1!";
                    var resultU = await _userManager.CreateAsync(initialAdmin, randPassword);
                    // AMS - Take success/fail of Admin user creation, assign admin role, and log an appropriate output.
                    if (!resultU.Succeeded)
                    {
                        throw new Exception($"Failed to create initial admin user: {string.Join(", ", resultU.Errors.Select(e => e.Description))}");
                    }

                    var resultUR = await _userManager.AddToRoleAsync(initialAdmin, "Admin");
                    if (!resultUR.Succeeded)
                    {
                        throw new Exception($"Failed to add 'admin' role to admin user: {string.Join(", ", resultUR.Errors.Select(e => e.Description))}");
                    }


                    // Create Profile
                    var initialAdminProfile = new PstsUserProfile
                    {
                        FName = "Default",
                        LName = "Administrator",
                        EmployeeId = initialAdmin.Id,
                        ManagerId = null
                    };

                    _db.PstsUserProfiles.Add(initialAdminProfile);
                    await _db.SaveChangesAsync();

                    _logger.LogCritical(@"
                        ==================================================
                            INITIAL ADMIN ACCOUNT CREATED
                            USERNAME: admin
                            PASSWORD: {Password}
                        ==================================================
                        ", randPassword);
                    
                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }
            }


            // ********** Populate Application settings **********

            foreach (var setting in DefaultSettings)
            {
                var key = setting.Key;
                var defaultVale = setting.Value;

                bool settingExists = await _db.AppSettingss.AnyAsync(u => u.Setting == key.ToString());
                if (!settingExists)
                {
                    var newSettingRecord = new AppSettings();
                    newSettingRecord.Setting = key.ToString();
                    newSettingRecord.Value = defaultVale;
                    try
                    {
                        _db.AppSettingss.Add(newSettingRecord);
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error writing default " + key.ToString() + " Setting to AppSettings DB. => " + ex.Message);
                    }
                }
            }

        }
    }


}
