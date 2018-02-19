using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectLocator.Areas.Application.DatabaseContext;
using ProjectLocator.Areas.Identity.DatabaseContext;
using ProjectLocator.Areas.Identity.Models;
using Hangfire.PostgreSql;
using Hangfire;
using ProjectLocator.Areas.Identity.ConfigureServices;
using NLog.Web;
using System.Text;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ProjectLocator
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
            services.AddDbContext<ApplicationContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("ApplicationConnection")));

            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Login";
                options.SlidingExpiration = true;
            });

            services.AddMvc();
            services.AddLogging();

            var sqlServerStorageOptions = new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(5),
            };

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"), sqlServerStorageOptions));

            JobStorage.Current = new PostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"));

            //services.AddEmailsService();
            //services.AddApplicationService();
            //services.AddScoped<IViewRenderService, ViewRenderService>();

            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(fileProvider);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            RoleManager<ApplicationRole> roleManager, ILoggerFactory loggerFactory)
        {
            env.ConfigureNLog("nlog.config");
            loggerFactory.AddNLog();

            //add NLog.Web
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();
            roleManager.SeedRoles().Wait();

            //app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            //app.UseMiddleware(typeof(LoggerMiddleware));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
