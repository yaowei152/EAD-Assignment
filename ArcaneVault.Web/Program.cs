/*
 * Name: <YOUR NAME>
 * Admin No: 241036U
 * Tutorial Group: <TUTORIAL GROUP>
 */

using ArcaneVault.Shared;
using ArcaneVault.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// The Web project owns the auth cookie (the Api is stateless and never issues cookies of its own).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ArcaneVaultAuth";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffOnly", policy => policy.RequireRole(RoleNames.Staff));
});

builder.Services.AddHttpClient<ArcaneVaultApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl is not configured in appsettings.json.");
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddRazorPages(options =>
{
    // CollectionItems: any authenticated user (User or Staff - Staff has User rights too).
    options.Conventions.AuthorizeFolder("/CollectionItems");
    // Categories and Analytics: Staff only.
    options.Conventions.AuthorizeFolder("/Categories", "StaffOnly");
    options.Conventions.AuthorizeFolder("/Analytics", "StaffOnly");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Intentionally no app.UseHttpsRedirection(): this is a local prototype and both projects
// talk to each other over plain HTTP so the solution runs without requiring anyone to trust
// a local dev HTTPS certificate first (dotnet dev-certs https --trust). See README.

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
