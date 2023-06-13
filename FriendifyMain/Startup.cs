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
                // Add more logging providers as needed
            });

            services.AddDbContext<FriendifyContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>().AddEntityFrameworkStores<FriendifyContext>();

            // Add authentication using JWT only

            // Create a symmetric security key from the secret key in configuration
            var secretKey = Configuration["SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Create a signing credentials object from the security key
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration["JwtIssuer"], // Get the issuer name from configuration
                       ValidAudience = Configuration["JwtAudience"], // Get the audience URL from configuration
                       IssuerSigningKey = signingCredentials.Key,
                       AuthenticationType = "Bearer"
                   };
               });

            // Add authorization policies based on roles or claims
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); // Require the Admin role for some actions
                options.AddPolicy("Moderator", policy => policy.RequireRole("Moderator")); // Require the Admin role for some actions
                options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User")); // Require the User claim for some actions
            });
            

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMySPA", policy =>
                {
                    policy.WithOrigins("https://localhost:44401")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
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

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); // Enable authentication middleware
            app.UseAuthorization(); // Enable authorization middleware
            app.UseCors("AllowMySPA");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}