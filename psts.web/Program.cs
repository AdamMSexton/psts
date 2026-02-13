using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using psts.web.Data;
using Psts.Web.Data;
using System.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

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
    options.Password.RequiredUniqueChars = 3;
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
        var tenantId = builder.Configuration["Authentication:Microsoft:TenantId"];

        // TenantId "common" allows any Microsoft account
        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
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

        // Fix issuer validation for multi-tenant scenarios
        options.TokenValidationParameters.IssuerValidator = (issuer, token, parameters) =>
        {
            // Accept tokens from any tenant when using "common"
            if (tenantId == "common" || tenantId == "organizations" || tenantId == "consumers")
            {
                return issuer;
            }
            // For specific tenant, validate normally
            if (issuer.Contains(tenantId))
            {
                return issuer;
            }
            throw new Microsoft.IdentityModel.Tokens.SecurityTokenInvalidIssuerException($"Issuer '{issuer}' is invalid.");
        };
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

builder.Services.AddScoped<PstsDbSeeder>();

var app = builder.Build();



// AMS - Apply migrations (for DB init and validation) & Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PstsDbContext>();
    db.Database.Migrate();
    
    // AMS - Seed Database
    var seeder = scope.ServiceProvider.GetRequiredService<PstsDbSeeder>();
    await seeder.SeedDbAsync();
}


// AMS ***** Create a dev user. DELETE FOR PRODUCTION
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

app.MapControllers();       // For APIs later

app.MapGet("/", context =>
{
    context.Response.Redirect("/Account/Login");
    return Task.CompletedTask;
});

app.UseDeveloperExceptionPage();

app.Run();
