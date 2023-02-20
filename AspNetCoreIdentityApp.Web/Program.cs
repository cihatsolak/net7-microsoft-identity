using AspNetCoreIdentityApp.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Identity Configuration
builder.Services.AddIdentityWithExt();

builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityAppCookie"
    };

    options.Cookie = cookieBuilder;

    options.LoginPath = new PathString("/Home/Signin");
    options.LogoutPath = new PathString("/Member/logout");
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
    options.SlidingExpiration = true; // Kullanıcı her siteye girdiğinde 10 gün daha uzatacaktır. 10 gün hiç giriş yapmazsa tekrar login sayfasına gidecek.
});

var app = builder.Build();

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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
