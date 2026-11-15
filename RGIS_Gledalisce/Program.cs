using Microsoft.EntityFrameworkCore;
using RGIS_Gledalisce.Data;
using RGIS_Gledalisce.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BazaContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/deploy-test", () => new
{
    message = "CD deployment successful",
    version = "1.0"
});

app.Run();
