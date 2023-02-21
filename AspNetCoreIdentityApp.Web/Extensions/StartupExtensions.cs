namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(opt => //şifre sıfırlama token ömrü
            {
                opt.TokenLifespan = TimeSpan.FromHours(2);
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(20);
                //Cookie'de yer alan securitystamp değeriyle veritabanını karşılaştırır uyuşmazlık durumunda oturumu sonlandırır.
                //Default olarak 30 dakikadır ama ben burda 20 dakikaya çektim.
                // _userManager.UpdateSecurityStampAsync(user)
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;


                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddPasswordValidator<CustomPasswordValidator>()
            .AddUserValidator<CustomUserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}