using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using TradeManagementApp.Data;
using TradeManagementApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("TradeContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'TradeContextConnection' not found.");

// Add services to the container.
builder.Services.AddRazorPages();

// Configure the DbContext with the connection string
builder.Services.AddDbContext<TradeContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TradeContext>()
    .AddDefaultTokenProviders();

// Register the dummy email sender service
builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create an admin user if it doesn't exist
    var adminEmail = "admin@example.com"; // Change this to your admin email
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(adminUser, "CarlosSuperUserVilla$321"); // Change password as needed
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // This ensures static files like uploads are served

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

app.Run();
