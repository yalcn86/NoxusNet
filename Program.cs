using Microsoft.EntityFrameworkCore;
using WhatchParty.Models;
using WhatchParty.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Session (Oturum) Servis Kaydı - Mobil Cihazlar ve HTTPS İçin Tam Optimize Edildi
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Mobil tarayıcıların HTTPS altında oturumu reddetmesini engellemek için siber önlem:
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Veritabanı Servisinin Kaydı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// SIGNALR SERVİS PROTOKOLÜ
builder.Services.AddSignalR();

var app = builder.Build();

// AUTOMATIC MIGRATION: Sunucu ilk açıldığında veritabanını ve tabloları otomatik oluşturur
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Siber Doğrulama: Veritabanı ve tablolar başarıyla oluşturuldu!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Veritabanı migration hatası: " + ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// SIGNALR ROTASI
app.MapHub<TunnelHub>("/tunnelHub");

app.Run();
