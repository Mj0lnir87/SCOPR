using SCOPR.API;

var builder = WebApplication.CreateBuilder(args);

// Ensure the correct appsettings file is loaded based on the environment
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Base configuration
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true) // Environment-specific configuration
    .AddEnvironmentVariables();

DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration);

// Add services to the container.  
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // Enable support annotations
});

// Read AllowedHosts from configuration
var allowedHosts = builder.Configuration["AllowedHosts"]?
    .Split(';', StringSplitOptions.RemoveEmptyEntries)
    ?? Array.Empty<string>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedHostsPolicy", policy =>
    {
        if (allowedHosts.Contains("*"))
        {
            policy.AllowAnyOrigin(); // Allow all origins if '*' is specified
        }
        else
        {
            policy.WithOrigins(allowedHosts) // Allow only specified origins
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowedHostsPolicy"); // CORS middleware

app.UseAuthorization();

app.MapControllers();

app.Run();
