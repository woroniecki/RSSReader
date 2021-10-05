using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using AutoWrapper;
using Microsoft.AspNetCore.Mvc;
using AutoWrapper.Wrappers;
using System.Reflection;
using ServiceLayer.AuthServices;
using NetCore.AutoRegisterDi;
using LogicLayer.Helpers;
using DbAccess.Core;
using RSSReader.Helpers;
using DataLayer.Code;
using DataLayer.Models;
using RSSReader.Config;
using System;

namespace RSSReader
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public string ConfigName { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .ConfigureJsonFile()
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigName = Configuration.GetValue(typeof(string), "ConfigName") as string;

            string sqlConnectionStr = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DataContext>(options => options
            .UseSqlServer(
                sqlConnectionStr,
                b => b.MigrationsAssembly("DataLayer"))
            );
             
            services.AddDefaultIdentity<ApiUser>()
                .AddEntityFrameworkStores<DataContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                // options.Lockout.MaxFailedAccessAttempts = 5;
                // options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.Configure<ApiBehaviorOptions>(options => {
                //options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //var modelState = actionContext.ModelState.Values;
                    throw new ApiProblemDetailsException(actionContext.ModelState);
                };
            });
            services.AddMvc();

            services.AddCors();
            
            services.AddHttpContextAccessor();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IHttpHelperService, HttpHelperService>();

            services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(AuthLoginService)))
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();

            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = $"ClientApp/{ConfigName}");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            //It breaks the SPA
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions
            {
                UseApiProblemDetailsException = false,
                IsApiOnly = false,
                BypassHTMLValidation = true
            });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: $"start:{ConfigName.ToLower()}");
                }
            });
        }
    }
}
