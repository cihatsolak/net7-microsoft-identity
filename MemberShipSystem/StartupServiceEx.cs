using MemberShip.Web.IdentityCustomValidators;
using MemberShip.Web.Models;
using MemberShip.Web.Requirements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web
{
    public static class StartupServiceEx
    {
        public static void ConfigureApplicationCookie(this IServiceCollection services)
        {
            CookieBuilder cookieConfig = new CookieBuilder
            {
                Name = Cookie.NAME,
                HttpOnly = false,
                SameSite = SameSiteMode.Strict, //Sadece bana istek yapan sayfadan cookie al.
                SecurePolicy = CookieSecurePolicy.SameAsRequest //İstek http'den yapılmışsa http, https den yapılmışssa https'den cookie al.
            };

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie = cookieConfig;

                options.LoginPath = new PathString(Cookie.LOGIN_PATH);
                options.LogoutPath = new PathString(Cookie.LOGOUT_PATH);
                options.AccessDeniedPath = new PathString(Cookie.ACCESS_DENIED_PATH);
                options.SlidingExpiration = true; // Kullanıcı cookie ömrünün yarısını geçtikten sonra default olarak verilen değer kadar cookie süresi uzar.
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; // Email adresi unique olsun.
                options.User.AllowedUserNameCharacters = Identity.ALLOWED_USERNAME_CHARACTERS; //Kullanıcı adı bu karakterleri içerebilir.

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

        public static void ConfiguraAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecretKey"];
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecretKey"];
            })
            .AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = configuration["Authentication:Twitter:ConsumerKey"];
                twitterOptions.ConsumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
                twitterOptions.RetrieveUserDetails = true;
            });
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //Claim bazlı yetkilendirme yapabilmek için policy eklememiz gereklidir. 
                options.AddPolicy(Policy.CITY, policy => //Ankara Policy.
                {
                    policy.RequireClaim(ClaimName.CITY, "ankara"); //Kullanıcının city claim'ine sahip olması gerekli ve değeride ankara olmalı.
                });

                options.AddPolicy(Policy.AGE, policy =>
                {
                    policy.RequireClaim(ClaimName.AGE);
                });

                options.AddPolicy(Policy.EXCHANGE, policy =>
                {
                    policy.AddRequirements(new ExpireDateExchangeRequirement());
                });
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
