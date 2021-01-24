using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Website.Areas.Identity.IdentityHostingStartup))]
namespace Website.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}