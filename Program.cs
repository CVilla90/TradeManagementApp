using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using TradeManagementApp.Data;
using TradeManagementApp.Services;
using Microsoft.Extensions.Logging;

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

// Register the email sender service (dummy service in this case)
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Add logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    try
    {
        // Define roles
        string[] roles = { "Admin", "User" };
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation($"Created role: {role}");
            }
        }

        // Retrieve admin credentials from environment variables
        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@example.com";
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") 
                            ?? throw new InvalidOperationException("Admin password not set.");

        // Create an admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation($"Admin user created with email: {adminEmail}");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError($"Error creating admin user: {error.Description}");
                }
            }
        }
        else
        {
            logger.LogInformation("Admin user already exists.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"An error occurred while seeding the roles or creating the admin user: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Commenting this out to avoid HTTPS issues in local dev
// app.UseHttpsRedirection();

app.UseStaticFiles(); // Ensure static files like uploads are served

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
