using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using SIMCMD.Core;
using SIMCMD.Services.Scheduler;
using SIMCMD.Data;
using SIMCMD.Models;
using SIMCMD.BackGroundJobs;
using SIMCMD.BackGroundJobs.Jobs;
using Quartz;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using System.ComponentModel.DataAnnotations;
using SIMCMD.Services.Email;

namespace SIMCMD
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityExtendUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()    
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SIMCMD", Version = "v1" });
            });
            services.AddRazorPages();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMyBackgroundJobs, MyBackgroundJobs>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();

                q.AddJobAndTrigger<SystemExecuteJob_DownloadFiles>(Configuration);

                q.AddJobAndTrigger<SystemExecuteJob_ConvertFiles>(Configuration);

                q.AddJobAndTrigger<SystemExecuteJob_ImportFiles>(Configuration);
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIMCMD v1"));
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        } 
    }
}
