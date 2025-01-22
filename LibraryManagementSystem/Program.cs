using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDb")));
builder.Services.AddSession();

// Configure authentication with cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Users/Login"; // Redirect to login page if not authenticated
        options.LogoutPath = "/Users/Logout"; // Redirect to logout action
        options.AccessDeniedPath = "/Users/AccessDenied"; // Redirect if access is denied
    });

var app = builder.Build();

// Add Admin user if not exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LibraryContext>();
    AddAdminUser(context);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add session and authentication middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Custom middleware for login redirection
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

    // Redirect to login if unauthenticated and not accessing login or register pages
    if (!isAuthenticated && !path.StartsWithSegments("/Users/Login") && !path.StartsWithSegments("/Users/Register"))
    {
        context.Response.Redirect("/Users/Login");
        return;
    }

    await next();
});

// Map Razor Pages
app.MapRazorPages();

// Set login page as the default route
app.MapGet("/", context =>
{
    context.Response.Redirect("/Users/Login");
    return Task.CompletedTask;
});

app.Run();

void AddAdminUser(LibraryContext context)
{
    var adminUser = new User
    {
        FirstName = "Admin",
        LastName = "User",
        Email = "admin@example.com",
        Password = HashPassword("Admin@123"), // Hash the password here
        Role = "Admin",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    };

    if (!context.Users.Any(u => u.Email == adminUser.Email))
    {
        context.Users.Add(adminUser);
        context.SaveChanges();
        Console.WriteLine("Admin user created successfully.");
    }
    else
    {
        Console.WriteLine("Admin user already exists.");
    }
}

// Password hashing method
string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}

