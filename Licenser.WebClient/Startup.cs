using Licenser.Encryption.Services;
using Licenser.LicenseDistribution.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Licenser.WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ILicenseDistributionService, LicenseDistributionService>();
            services.AddScoped<ILicenseKeyService, LicenseKeyService>();
            services.AddScoped<IRSAKeyService, RSAKeyService>();

            services.AddMvc();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILicenseKeyService licenseKeyGenerationService, IRSAKeyService rsaKeyService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Generating encryption keys.
            licenseKeyGenerationService.GeneratePublicPrivateKeyPair();
            rsaKeyService.GeneratePublicPrivateKeyPair();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");

            });
        }
    }
}
