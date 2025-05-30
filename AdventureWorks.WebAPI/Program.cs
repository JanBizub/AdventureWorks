using DependencyInjectionApi.Data;
using Microsoft.EntityFrameworkCore;
using AdventureWorks.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AdventureWorksDw2022Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register CustomersService and ICalculationService for dependency injection
builder.Services.AddScoped<CustomersService>();
builder.Services.AddScoped<ICalculationService, CalculationService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// A partial class for the Program, created to make the Program.cs file accessible 
/// for integration tests. This allows test projects to reference and interact with 
/// the application's entry point.
/// </summary>
public partial class Program { }