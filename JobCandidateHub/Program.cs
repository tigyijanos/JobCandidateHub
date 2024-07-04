using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using JobCandidateHub.Data;
using JobCandidateHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobCandidateHub API", Version = "v1" });
});

// Register CandidateDbContext with SQLite
builder.Services.AddDbContext<CandidateDbContext>(options =>
    options.UseSqlite("Data Source=candidates.db"));

builder.Services.AddMemoryCache();

// Register ValidationService
builder.Services.AddScoped<IValidationService, ValidationService>();

// Register CandidateService
builder.Services.AddScoped<ICandidateService, CandidateService>();

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CandidateDbContext>();
    dbContext.Database.EnsureCreated();  // This ensures the database and tables are created
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobCandidateHub API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
