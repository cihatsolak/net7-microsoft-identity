using MemberShip.Web.ClaimProviders;
using MemberShip.Web.Containers;
using MemberShip.Web.Requirements;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

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

            services.AddScoped<IClaimsTransformation, ClaimProvider>(); //Claimi özelleþtirdik, claim'lere ek olarak özellikler ekliyorum bu sýnýf ile.
            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandle>();

            services.AddMvc().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSession();
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Security}/{action=SignIn}/{id?}");
            });
        }
    }
}
