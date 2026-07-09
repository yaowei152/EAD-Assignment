/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Api.Data;
using ArcaneVault.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ArcaneVaultDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPasswordHasher<ArcaneVaultUser>, PasswordHasher<ArcaneVaultUser>>();

var app = builder.Build();

// Apply pending EF Core migrations and seed lookup/reference data on startup so the
// SQLite database file always exists and is up to date with no manual steps.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArcaneVaultDbContext>();
    await SeedData.InitializeAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Intentionally no app.UseHttpsRedirection(): this is a local prototype and both projects
// talk to each other over plain HTTP so the solution runs without requiring anyone to trust
// a local dev HTTPS certificate first (dotnet dev-certs https --trust). See README.

app.UseAuthorization();

app.MapControllers();

app.Run();
