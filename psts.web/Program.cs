using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using psts.web.Data;
using Psts.Web.Data;
using System.Data;

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

builder.Services.AddAuthentication();
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
