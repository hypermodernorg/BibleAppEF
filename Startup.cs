using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BibleAppEF.Areas.ImportBible.Models;
using Microsoft.EntityFrameworkCore;
using BibleAppEF.Areas.ImportBible.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql;

namespace BibleAppEF
{
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
            services.AddControllersWithViews();
            services.AddRazorPages();
          
            services.AddDbContext<BibleContext, BibleContext>();
            // Replace "YourDbContext" with the name of your own DbContext derived class.
            //services.AddDbContextPool<BibleContext>(
            //    dbContextOptions => dbContextOptions
            //        .UseMySql(
            //            // Replace with your connection string.
            //            "server=localhost;user=root;password=Nisarascalcj1!r;database=biblebase",
            //            // Replace with your server version and type.
            //            // For common usages, see pull request #1233.
            //            MySqlServerVersion.LatestSupportedServerVersion,   //(8, 0, 21), // use MariaDbServerVersion for MariaDB
            //            mySqlOptions => mySqlOptions
            //                .CharSetBehavior(CharSetBehavior.NeverAppend))
            //        // Everything from this point on is optional but helps with debugging.
            //        .EnableSensitiveDataLogging()
            //        .EnableDetailedErrors()
            //);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    areaName: "ImportBible",
                    name: "ImportBible",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


            });

        }
    }
}
