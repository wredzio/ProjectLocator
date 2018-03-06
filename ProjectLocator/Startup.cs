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
using ProjectLocator.Shared.Middlewares;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ProjectLocator.Shared.MediatR;
using FluentValidation.AspNetCore;
using ProjectLocator.Areas.Identity.Accounts.Register;
using MediatR;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("ApplicationConnection")));

            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("IdentityConnection")));

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

            services.AddMvc()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddControllersAsServices();
            services.AddLogging();
            AddHangfire(services);

            //services.AddEmailsService();
            //services.AddApplicationService();
            //services.AddScoped<IViewRenderService, ViewRenderService>();

            var fileProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(fileProvider);
            });

            services.AddScoped<IMediator, Mediator>();
            services.AddTransient<SingleInstanceFactory>(sp => t => sp.GetService(t));
            services.AddTransient<MultiInstanceFactory>(sp => t => sp.GetServices(t));

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<RegisterCommandApplicationUserMapper>().As<IMapper<RegisterCommand, ApplicationUser>>();
            builder.ConfigureMediatR();

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        private void AddHangfire(IServiceCollection services)
        {
            var sqlServerStorageOptions = new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(5),
            };

            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"), sqlServerStorageOptions));

            JobStorage.Current = new PostgreSqlStorage(Configuration.GetConnectionString("HangfireConnection"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            RoleManager<ApplicationRole> roleManager, ILoggerFactory loggerFactory)
        {
            env.ConfigureNLog("nlog.config");
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseHangfireDashboard("/hangfire");
            app.UseHangfireServer();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMiddleware(typeof(LoggerMiddleware));

            app.UseMvc();

            roleManager.SeedRoles().Wait();
        }
    }
}
