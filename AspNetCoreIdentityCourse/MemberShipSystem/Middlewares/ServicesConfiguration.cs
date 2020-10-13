using MemberShip.Web.IdentityCustomValidators;
using MemberShip.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MemberShip.Web.Middlewares
{
    public static class ServicesConfiguration
    {
        public static void ConfigureApplicationCookie(this IServiceCollection services)
        {
            CookieBuilder cookieConfig = new CookieBuilder();
            cookieConfig.Name = "MyBlog";
            cookieConfig.HttpOnly = false;
            cookieConfig.SameSite = SameSiteMode.Strict; //Sadece bana istek yapan sayfadan cookie al.
            cookieConfig.SecurePolicy = CookieSecurePolicy.SameAsRequest; //İstek http'den yapılmışsa http, https den yapılmışssa https'den cookie al.

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie = cookieConfig;

                options.LoginPath = new PathString("/Account/SignIn");
                options.AccessDeniedPath = new PathString("/AccessDenied");
                //options.LogoutPath = new PathString("Account/SignOut");
                options.SlidingExpiration = true; // Kullanıcı cookie ömrünün yarısını geçtikten sonra default olarak verilen değer kadar cookie süresi uzar.
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; // Email adresi unique olsun.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoöpqrsştuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._"; //Kullanıcı adı bu karakterleri içerebilir.

                options.Password.RequiredLength = 4; //Şifre uzunluğu minimum 4 karakter olmalı.
                options.Password.RequireNonAlphanumeric = false; // ?,* gibi karakterlerin girilmesi zorunlu degil.
                options.Password.RequireDigit = false; //Rakam kullanılması zorunlu degil.
                options.Password.RequireLowercase = false; //Küçük karakter girilmesi zorunlu degil.
                options.Password.RequireUppercase = false; //Büyük karakter girilmesi zorunlu degil.

            })
            .AddUserValidator<CustomUserValidator>() //Kendi AppUser Doğrulamamızı yapmak
            .AddPasswordValidator<CustomPasswordValidator>() //Kendi Şifre Doğrulamamızı yapmak
            .AddErrorDescriber<CustomIdentityErrorDescriber>() //Hata mesajlarının türkçeleştirilmesi
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders(); //Şifre sıfırlamada token üretmesi içiin

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(20); 
                //Cookie'de yer alan securitystamp değeriyle veritabanını karşılaştırır uyuşmazlık durumunda oturumu sonlandırır.
                //Default olarak 30 dakikadır ama ben burda 20 dakikaya çektim.
            });
        }

        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
        }

    }
}
