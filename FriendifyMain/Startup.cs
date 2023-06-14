using FriendifyMain.Mappers;
using FriendifyMain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;



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

            services.AddLogging(logging =>
            {
                logging.AddConsole(); // Configure logging to write to the console
                logging.AddDebug();   // Optionally, add additional logging providers

                // Add authentication logging
                logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
                logging.AddFilter("System.Security.Claims", LogLevel.Debug);
                // Add more logging providers as needed
            });

            services.AddDbContext<FriendifyContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>().AddEntityFrameworkStores<FriendifyContext>();

            // Create a symmetric security key from the secret key in configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

            // Create a signing credentials object from the security key
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            

            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["Jwt:JwtIssuer"], // Get the issuer name from configuration
                       ValidAudience = Configuration["Jwt:JwtAudience"], // Get the audience URL from configuration
                       IssuerSigningKey = signingCredentials.Key
                   };
               });

            // Add authorization policies based on roles or claims
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
    
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                    policy.AuthenticationSchemes.Add("Bearer"); // Add Bearer authentication scheme
                });
    
                options.AddPolicy("Moderator", policy =>
                {
                    policy.RequireRole("Moderator");
                    policy.AuthenticationSchemes.Add("Bearer"); // Add Bearer authentication scheme
                });
    
                options.AddPolicy("User", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "User");
                    policy.AuthenticationSchemes.Add("Bearer"); // Add Bearer authentication scheme
                });
            });
            

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMySPA", policy =>
                {
                    policy.WithOrigins("https://localhost:44401")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("Access-Control-Allow-Origin");
                });
            });

            // Add AutoMapper for mapping models and view models
            services.AddAutoMapper(typeof(RegisterMapper));
            services.AddAutoMapper(typeof(UpdateMapper));

            // Add Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
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

            app.UseRouting();
            app.UseCors("AllowMySPA");
            app.UseAuthentication(); // Enable authentication middleware
            app.UseAuthorization(); // Enable authorization middleware
            
            // Add authentication logging middleware
            app.Use(async (context, next) =>
            {
                await next();

                if (context.User.Identity?.IsAuthenticated == true)
                {
                    logger.LogInformation("User authenticated: {UserName}", context.User.Identity.Name);
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}