using Microsoft.EntityFrameworkCore;
using Psts.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// AMS - Reqister EF Core
builder.Services.AddDbContext<PstsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// AMS - Apply migrations (for DB init and validation)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PstsDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "Hello World!");

app.Run();
