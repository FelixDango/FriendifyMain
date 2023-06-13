﻿using FriendifyMain.Mappers;
using FriendifyMain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FriendifyMain
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
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddLogging(logging =>
            {
                logging.AddConsole(); // Configure logging to write to the console
                logging.AddDebug();   // Optionally, add additional logging providers
                // Add more logging providers as needed
            });

            services.AddDbContext<FriendifyContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = null;

            })
            .AddEntityFrameworkStores<FriendifyContext>()
            .AddDefaultTokenProviders();
            
            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme; // Use JWTs for authentication
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Use JWTs for challenges
            }).AddScheme<JwtBearerOptions, JwtBearerHandler>(
                JwtBearerDefaults.AuthenticationScheme, // Use JWTs for authentication
                options => Configuration.Bind("JwtSettings", options) // Bind the JwtSettings section of appsettings.json to the options
            );
            

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAutoMapper(typeof(RegisterMapper));
            
            // add swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1");
                });
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); // Add this line to enable authentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
