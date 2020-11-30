using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Web.Options;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StorageOptions>(Configuration.GetSection("StorageOptions"));
            
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
            services.Configure<GzipCompressionProviderOptions>(compressionOptions =>
                compressionOptions.Level = CompressionLevel.Optimal);

            services.AddHttpContextAccessor();
            
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
            services.AddControllersWithViews().AddMicrosoftIdentityUI();
            
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Info/Index", "");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}