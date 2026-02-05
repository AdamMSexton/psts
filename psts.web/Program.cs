using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Psts.Web.Data;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// AMS - Reqister EF Core
builder.Services.AddDbContext<PstsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<PstsDbContext>()
    .AddDefaultTokenProviders();

// AMS - Establish standard password requirements.  Update when ready to deploy.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

builder.Services.Configure<IdentityRole>(options =>
{
    options.Name = null; // just ensure EF Core doesn't try weird constraints
});

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

// Configure external authentication
var authBuilder = builder.Services.AddAuthentication();

// Google Authentication - only enabled if ClientId is configured
if (!string.IsNullOrEmpty(builder.Configuration["Authentication:Google:ClientId"]))
{
    authBuilder.AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    });
}

// Microsoft Authentication - only enabled if ClientId is configured
if (!string.IsNullOrEmpty(builder.Configuration["Authentication:Microsoft:ClientId"]))
{
    authBuilder.AddOpenIdConnect("Microsoft", options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["Authentication:Microsoft:TenantId"]}/v2.0";
        options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
        options.CallbackPath = "/signin-microsoft";
        options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

// Auth0 Authentication - only enabled if ClientId is configured
if (!string.IsNullOrEmpty(builder.Configuration["Authentication:Auth0:ClientId"]))
{
    authBuilder.AddOpenIdConnect("Auth0", options =>
    {
        options.Authority = builder.Configuration["Authentication:Auth0:Authority"]!;
        options.ClientId = builder.Configuration["Authentication:Auth0:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Auth0:ClientSecret"]!;
        options.CallbackPath = "/signin-auth0";
        options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

// Okta Authentication - only enabled if ClientId is configured
if (!string.IsNullOrEmpty(builder.Configuration["Authentication:Okta:ClientId"]))
{
    authBuilder.AddOpenIdConnect("Okta", options =>
    {
        options.Authority = builder.Configuration["Authentication:Okta:Authority"]!;
        options.ClientId = builder.Configuration["Authentication:Okta:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Okta:ClientSecret"]!;
        options.CallbackPath = "/signin-okta";
        options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.TokenValidationParameters.NameClaimType = "name";
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });
}

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

builder.Services.AddControllers();      // For APIs later

var app = builder.Build();

// AMS - Apply migrations (for DB init and validation)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PstsDbContext>();
    db.Database.Migrate();
}


// AMS ***** This section verify roles and at least 1 admin exists.  ensure bare minimum setup in the case of a fresh install or DB
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    bool rolesCreated = true;

    // AMS - Verify Admin role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Admin' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Manager role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Manager"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Manager"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Manager' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Employee role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Employee"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Employee"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Employee' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Clerk role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Clerk"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Clerk"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Clerk' failed: {Error}", e.Description);
        }
    }

    // AMS - Verify Pending role exists, if not create it
    if (!await roleManager.RoleExistsAsync("Pending"))
    {
        var status = await roleManager.CreateAsync(new IdentityRole("Pending"));
        if (!status.Succeeded)
        {
            rolesCreated = true;
            foreach (var e in status.Errors)
                app.Logger.LogError("Role 'Pending' failed: {Error}", e.Description);
        }
    }

    if (!rolesCreated)
    {
        app.Logger.LogError("Could not create required user roles. APPLICATION MAY NOT FUNCTION CORRECTLY.");
    }

    // AMS if no admins exist create the default and set to require password change at next login.
    var admins = await userManager.GetUsersInRoleAsync("Admin");
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
        if (await userManager.FindByNameAsync(initialAdmin.UserName) == null)
        {
            // AMS - Create new admin user
            string randPassword = $"{Guid.NewGuid():N}".Substring(0, 12) + "aA1!";
            var result = await userManager.CreateAsync(initialAdmin, randPassword);
            // AMS - Take success/fail of Admin user creation, assign admin role, and log an appropriate output.
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create initial admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                await userManager.AddToRoleAsync(initialAdmin, "Admin");
                app.Logger.LogCritical(@"
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





// AMS ***** Create a dev user. Delete for production
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    var devUser = new AppUser { UserName = "dev", Email = "dev@psts.local" };
    if (await userManager.FindByNameAsync(devUser.UserName) == null)
    {
        await userManager.CreateAsync(devUser, "devdev");
    }
}
// AMS ***** DELETE ABOVE FOR PRODUCTION, DEV ONLY.                                           Yeah im sure theres a better more professional way to do this but...  How would i leave these jokes.  Also if your reading this i forgot to delete, o jokes on me? you maybe?


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


app.MapGet("/_debug-login", async (
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager) =>
{
    var user = await userManager.FindByNameAsync("admin");
    if (user == null)
        return Results.Text("User NOT found");

    var result = await signInManager.PasswordSignInAsync(
        "admin",
        "0e9a5b04296baA1!",
        false,
        false);

    return Results.Text(
        $"Succeeded={result.Succeeded}, NotAllowed={result.IsNotAllowed}, LockedOut={result.IsLockedOut}");
});





app.MapControllers();       // For APIs later

app.MapGet("/", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.UseDeveloperExceptionPage();

app.Run();
