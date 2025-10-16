using EasyGames.Web.Data;
using EasyGames.Web.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) MVC
builder.Services.AddControllersWithViews();

// 2) InMemory provider so everyone can run without SQL now
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EasyGamesDb")); // <— this was missing

// 3) App services 
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Session + HttpContext accessor
builder.Services.AddHttpContextAccessor();           
builder.Services.AddSession();                       

// Cart service (session-backed)
builder.Services.AddScoped<ICartService, CartService>();


var app = builder.Build();
// seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();


// Areas route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
