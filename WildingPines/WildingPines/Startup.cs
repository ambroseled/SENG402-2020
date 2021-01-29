using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WildingPines.Communication.SprayModuleIpc;
using WildingPines.Io;
using WildingPines.Io.Reports;
using WildingPines.Io.SprayPlans;
using WildingPines.Io.Usb;

namespace WildingPines
{
    /// <summary>
    /// Used to start the Web API server.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // For dependency injection
            // See https://stackoverflow.com/a/38139500/5382830 for a nice and short explanation on different
            // service instantiation strategies.
            services.AddScoped<IUsbIo, UsbIo>();
            services.AddScoped<IUsbSprayPointsImporter, UsbSprayPointsImporter>();
            services.AddScoped<IUsbReportsExporter, UsbReportsExporter>();
            services.AddScoped<ILocalSprayPlansManager, LocalSprayPlansManager>();
            
            // Needs to be a singleton so that only one instance is created, and thus no multiple sockets try to bind
            // to the same address and port.
            services.AddSingleton<ISprayModuleBridge, SprayModuleBridge>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // comment out to avoid upgrading to a HTTPS connection and then getting an invalid certificate error
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}