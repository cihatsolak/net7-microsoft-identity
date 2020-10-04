using MemberShip.Web.IdentityCustomValidators;
using MemberShip.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MemberShipSystem
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; // Email adresi unique olsun.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoöpqrsþtuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._"; //Kullanýcý adý bu karakterleri içerebilir.

                options.Password.RequiredLength = 4; //Þifre uzunluðu minimum 4 karakter olmalý.
                options.Password.RequireNonAlphanumeric = false; // ?,* gibi karakterlerin girilmesi zorunlu degil.
                options.Password.RequireDigit = false; //Rakam kullanýlmasý zorunlu degil.
                options.Password.RequireLowercase = false; //Küçük karakter girilmesi zorunlu degil.
                options.Password.RequireUppercase = false; //Büyük karakter girilmesi zorunlu degil.

            })
            .AddUserValidator<CustomUserValidator>() //Kendi AppUser Doðrulamamýzý yapmak
            .AddPasswordValidator<CustomPasswordValidator>() //Kendi Þifre Doðrulamamýzý yapmak
            .AddErrorDescriber<CustomIdentityErrorDescriber>() //Hata mesajlarýnýn türkçeleþtirilmesi
            .AddEntityFrameworkStores<AppIdentityDbContext>();

            services.AddMvc().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
