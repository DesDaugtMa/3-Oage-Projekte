using Chat.Config;
using Chat.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IHostBuilder hostBuilder = builder.Host.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder.Sources.Clear();
    configurationBuilder.AddJsonFile($"Config/appsettings.json", false);
    configurationBuilder.AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", true);
    configurationBuilder.AddEnvironmentVariables();
    configurationBuilder.AddUserSecrets<Program>(true);
});

AppSettings appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
