using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AdventureWorks.Data.OLTP.AdventureWorksOLTPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorksOLTP")));

builder.Services.AddScoped<AdventureWorks.DomainServices.HR.EmployeeService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();