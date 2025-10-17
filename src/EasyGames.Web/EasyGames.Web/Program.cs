using EasyGames.Web.Data;
using EasyGames.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- EF Core: SQLite ---
var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Data Source=EasyGames.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));

// --- App services (keep these consistent across projects) ---
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- Middleware ---
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Areas before default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- Migrate + seed on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();        // runs any pending migrations
    DbSeeder.Seed(db);            // your existing seed method
}

app.Run();



