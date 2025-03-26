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

// Add Cross-Origin Resource Sharing (CORS) services to the application
// This is necessary to allow web browsers to make requests to our API from different domains
builder.Services.AddCors();

// Build the application
var app = builder.Build();

// Configure CORS middleware with specific policy
app.UseCors(x => x.AllowAnyHeader()    // Allow any HTTP header in the request
    .AllowAnyMethod()                   // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
    .WithOrigins("http://localhost:3000", "https://localhost:3000")); // Only allow requests from these specific origins

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
