using Microsoft.EntityFrameworkCore;
using MVC.Intro.Data;
using MVC.Intro.Services;

var builder = WebApplication.CreateBuilder(args);

// ПЪТ ДО БАЗАТА
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataFolder);
var dbPath = Path.Combine(dataFolder, "products.db");

// РЕГИСТРАЦИЯ С DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ProductService>();

var app = builder.Build();

// АВТОМАТИЧНО СЪЗДАВАНЕ НА ТАБЛИЦАТА
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Създава Products таблицата
}

// HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "routing",
    pattern: "{controller=Routing}/{*default}",
    defaults: new { controller = "Routing", action = "Default" });

app.Run();