using Microsoft.EntityFrameworkCore;
using Persistence;

// Create a web application builder with the provided arguments
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Register the MVC controllers
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    // Configure the DbContext to use SQLite with the connection string from the configuration
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Build the application
var app = builder.Build();

// Map the controllers to the application
app.MapControllers();

// Create a scope to manage the lifetime of services
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    // Get the required AppDbContext service
    var context = services.GetRequiredService<AppDbContext>();
    // Apply any pending migrations to the database
    await context.Database.MigrateAsync();
    // Seed the database with initial data
    await DbInitializer.SeedData(context);
}
catch (Exception ex)
{
    // Log any errors that occur during migration
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration.");
}

// Run the application
app.Run();
