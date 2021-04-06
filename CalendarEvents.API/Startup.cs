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
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Text;

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
                                        //.SetIsOriginAllowed(origin => origin == "http://localhost:4200")
                                        .AllowAnyOrigin()
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
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "Events.Update",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        //policyBuilder.AddRequirements(
                        //    new MustOwnRequirement<EventModel>()
                        //);
                        //policyBuilder.RequireClaim("scope", "calendareventsapi");
                    });
                authorizationOptions.AddPolicy(
                    "Events.Insert",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        //policyBuilder.RequireClaim("scope", "calendareventsapi");
                    });
            });

            string migrationsAssembly = typeof(CalendarEvents.DataAccess.ApplicationDbContext)
                .GetTypeInfo().Assembly.GetName().Name;

            string connectionString = null;
            try
            {
                string server = Environment.GetEnvironmentVariable("DatabaseServer") ?? "localhost";
                string database = Environment.GetEnvironmentVariable("DatabaseName") ?? "CalendarEventsAPIDb";
                string port = Environment.GetEnvironmentVariable("DatabasePort") ?? "1443";
                string user = Environment.GetEnvironmentVariable("DatabaseUser") ?? "sa";
                string password = Environment.GetEnvironmentVariable("DatabasePassword") ?? "<YourStrong@Passw0rd>";
                connectionString = $"Server={server},{port};Database={database};User ID={user};Password={password};";
            }
            catch
            {
                connectionString = Configuration.GetConnectionString("DefaultConnection");
            }


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(migrationsAssembly)
                );
            });



            //TODO: register all the generic service and repository with generic syntax like autofac does <>.
            services.AddScoped<IGenericService<EventModel>, EventService>();
            services.AddScoped<IGenericRepository<EventModel>, EventRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IFilter<EventModel>, MultiCheckboxFilter>();
            services.AddScoped<MultiCheckboxFilter>();
            services.AddScoped<IFiltersService<EventModel>, EventFiltersService>();
            services.AddScoped<EventFilterResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case FilterType.MultiCheckbox:
                        return serviceProvider.GetService<MultiCheckboxFilter>();
                    case FilterType.Undefined:
                    default:
                        throw new NotSupportedException($"EventFilterResolver, key: {key}");
                }
            });
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<EventModel>>());
            //services.AddScoped<IScrapingService, ScrapingService>(); //WIll be inside another project.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            PrepareDB.PreparePopulation(app).Wait();
            if (env.IsDevelopment())
            {                
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
