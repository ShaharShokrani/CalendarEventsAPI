using AutoMapper;
using CalendarEvents.API;
using CalendarEvents.API.Authorization;
using CalendarEvents.DataAccess;
using CalendarEvents.Models;
using CalendarEvents.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using System;
using System.Reflection;

namespace CalendarEvents
{
    //TODO: create a TestStartUp
    public class Startup
    {
        private readonly ILogger<Startup> log;
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILogger<Startup> log)
        {
            Configuration = configuration;            
            this.log = log;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: move this into a solid function.
            #region Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            IdentityModelEventSource.ShowPII = true; //Add this line

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                        //TODO: Use the config instead.
                                        builder
                                        .SetIsOriginAllowed(origin => origin == "http://localhost:4200")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                  });
            });

            //As a best practice its better to use authorization fro all of the controllers, and allow anonymous.
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddControllers();

            services.AddHttpContextAccessor();

            services.AddScoped<IAuthorizationHandler, MustOwnHandler<EventModel>>();

            string authority = Environment.GetEnvironmentVariable("Authority") ?? "http://localhost:5002";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;                
            }).AddJwtBearer(o =>
            {
                o.Authority = authority;
                o.Audience = "calendareventsapi";
                o.RequireHttpsMetadata = false;                
            });

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "Events.Update",
                    policyBuilder =>
                    {
                        //policyBuilder.RequireAuthenticatedUser();
                        //policyBuilder.AddRequirements(
                        //    new MustOwnRequirement<EventModel>()
                        //);
                        policyBuilder.RequireClaim("scope", "calendareventsapi");
                    });
                authorizationOptions.AddPolicy(
                    "Events.Insert",
                    policyBuilder =>
                    {
                        policyBuilder.RequireClaim("scope", "calendareventsapi");
                    });
            });

            string migrationsAssembly = typeof(CalendarEvents.DataAccess.ApplicationDbContext)
                .GetTypeInfo().Assembly.GetName().Name;

            string server = Environment.GetEnvironmentVariable("DatabaseServer") ?? "localhost";
            string database = Environment.GetEnvironmentVariable("DatabaseName") ?? "CalendarEventsAPIDb";
            string port = Environment.GetEnvironmentVariable("DatabasePort") ?? "1443";
            string user = Environment.GetEnvironmentVariable("DatabaseUser") ?? "sa";
            string password = Environment.GetEnvironmentVariable("DatabasePassword") ?? "<YourStrong@Passw0rd>";

            string connectionString = $"Server={server},{port};Database={database};User ID={user};Password={password};";
            Console.WriteLine(connectionString);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(migrationsAssembly)
                );
            });



            //TODO: register all the generic service and repository with generic syntax like autofac does <>.
            services.AddScoped<IGenericService<EventModel>, GenericService<EventModel>>();
            services.AddScoped<IGenericRepository<EventModel>, GenericRepository<EventModel>>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IScrapingService, ScrapingService>(); //WIll be inside another project.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                PrepareDB.PreparePopulation(app).Wait();
                app.UseDeveloperExceptionPage();
            }

            //else
            //{
            //    app.UseExceptionHandler(appBuilder =>
            //    {
            //        appBuilder.Run(async context =>
            //        {
            //            // ensure generic 500 status code on fault.
            //            context.Response.StatusCode = StatusCodes.Status500InternalServerError; ;
            //            await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
            //        });
            //    });
            //    // The default HSTS value is 30 days. You may want to change this for 
            //    // production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseAuthentication();

            //loggerFactory.AddFile(Configuration.GetSection("Logging"));           

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseStaticFiles();
            //app.UseMvc();
            
            
            //eventsDbContext.Database.Migrate();

            // Start Scrapper Service whe application start, and stop it when stopping
            //appLifetime.ApplicationStarted.Register(scrapingService.Start);
            //appLifetime.ApplicationStarted.Register( () => log.LogInformation("Application Started"));
            //appLifetime.ApplicationStopping.Register(scrapingService.Stop);

            //appLifetime.ApplicationStopping.Register(() => log.LogInformation("Application Stopping"));
        }
    }
}
