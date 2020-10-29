using MemberShip.Web;
using MemberShip.Web.ClaimProviders;
using MemberShip.Web.Requirements;
using MemberShip.Web.Tools.Settings;
using MemberShip.Web.TwoFactorServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.ConfigureDbContext(Configuration);
            services.ConfigureIdentity();
            services.ConfigureApplicationCookie();
            services.ConfiguraAuthentication(Configuration);
            services.ConfigureAuthorization();

            services.AddSession();

            services.AddScoped<IClaimsTransformation, ClaimProvider>(); //Claimi �zelle�tirdik, claim'lere ek olarak �zellikler ekliyorum bu s�n�f ile.
            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandle>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();

            services.Configure<SendGridSettings>(Configuration.GetSection(nameof(SendGridSettings)));
            services.Configure<TwoFactorSettings>(Configuration.GetSection(nameof(TwoFactorSettings)));

            services.AddMvc().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseStatusCodePages();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Member}/{action=Index}/{id?}");
            });
        }
    }
}
