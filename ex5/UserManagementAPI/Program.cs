using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Common;
using UserManagementAPI.Data;
using UserManagementAPI.Middleware;
using UserManagementAPI.Services;
using FluentValidation.AspNetCore;
using FluentValidation;
using UserManagementAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// Configure DbContext with In-Memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("UserManagementDb"));

// Register HttpClient for external API calls
builder.Services.AddHttpClient<IJsonPlaceholderService, JsonPlaceholderService>();

// Register services with Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJsonPlaceholderService, JsonPlaceholderService>();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS to allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(Constants.CorsPolicy, policy =>
    {
        policy.WithOrigins(Constants.AngularAppUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "RESTful API for managing users with CRUD operations"
    });
});

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database with external API data if empty
// JSONPlaceholder serves as mock data provider for initial seed
// Graceful failure allows app to start regardless of network
// Only runs once, subsequent starts use existing database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var jsonPlaceholderService = services.GetRequiredService<IJsonPlaceholderService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if Users table is empty
        if (!await context.Users.AnyAsync())
        {
            logger.LogInformation("Database is empty. Fetching initial users from JSONPlaceholder API");

            // Fetch users from external API
            var users = await jsonPlaceholderService.FetchInitialUsersAsync();

            if (users.Count > 0)
            {
                // Add users to database
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded database with {Count} users from JSONPlaceholder", users.Count);
            }
            else
            {
                logger.LogWarning("No users were fetched from JSONPlaceholder. Database remains empty.");
            }
        }
        else
        {
            logger.LogInformation("Database already contains users. Skipping initial seed.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database. Application will start with existing data.");
        // Continue application startup even if seeding fails
    }
}

// Configure the HTTP request pipeline

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

// Use global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors(Constants.CorsPolicy);

// Enable authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
