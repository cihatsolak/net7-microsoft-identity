using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Requirements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Identity Configuration
builder.Services.AddIdentityWithExt();

builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CityRestriction", policy =>
    {
        //city claim'inde istanbul, izmir, manisa olanlar erişebilir
        policy.RequireClaim("city", "İstanbul", "İzmir", "Manisa");
        policy.RequireRole("master", "admin"); //master ve admin rolleri erişebilir.
    });

    options.AddPolicy("ExchangeRestriction", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
});


builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityAppCookie"
    };

    options.Cookie = cookieBuilder;

    options.LoginPath = new PathString("/Home/Signin");
    options.LogoutPath = new PathString("/Member/logout");
    options.AccessDeniedPath = new PathString("/RoleTest/AccessDenied");
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
    options.SlidingExpiration = true; // Kullanıcı her siteye girdiğinde 10 gün daha uzatacaktır. 10 gün hiç giriş yapmazsa tekrar login sayfasına gidecek.
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
