using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WildingPines
{
    /// <summary>
    /// Documentation for the Web API library is here:
    /// https://docs.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-3.1
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for the solution. Starts the REST server.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
