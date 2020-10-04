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
                options.User.AllowedUserNameCharacters = "abcdefghijklmno�pqrs�tuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._"; //Kullan�c� ad� bu karakterleri i�erebilir.

                options.Password.RequiredLength = 4; //�ifre uzunlu�u minimum 4 karakter olmal�.
                options.Password.RequireNonAlphanumeric = false; // ?,* gibi karakterlerin girilmesi zorunlu degil.
                options.Password.RequireDigit = false; //Rakam kullan�lmas� zorunlu degil.
                options.Password.RequireLowercase = false; //K���k karakter girilmesi zorunlu degil.
                options.Password.RequireUppercase = false; //B�y�k karakter girilmesi zorunlu degil.

            })
            .AddUserValidator<CustomUserValidator>() //Kendi AppUser Do�rulamam�z� yapmak
            .AddPasswordValidator<CustomPasswordValidator>() //Kendi �ifre Do�rulamam�z� yapmak
            .AddErrorDescriber<CustomIdentityErrorDescriber>() //Hata mesajlar�n�n t�rk�ele�tirilmesi
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
