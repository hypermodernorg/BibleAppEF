using BibleAppEF.Areas.Identity.Data;
using BibleAppEF.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BibleAppEF.Areas.Identity.IdentityHostingStartup))]
namespace BibleAppEF.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("IdentityContextConnection")));

                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddRoles<ApplicationRole>() // ? 
                    .AddRoleManager<RoleManager<ApplicationRole>>() // ?
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
                services.AddAuthorization(options => {
                    options.AddPolicy("ViewRoles", policy => policy.RequireClaim("CanViewRoles"));
                    options.AddPolicy("ViewUsers", policy => policy.RequireClaim("CanViewUsers"));
                });
            });
        }
    }
}