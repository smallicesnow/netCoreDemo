using Demeter.Agent.Auth;
using Demeter.Agent.Object;
using Demeter.Agent.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Demeter.Agent
{
    public class Startup
    {
        private static DemeterConfig config = new DemeterConfig();
        private const string PublicPolicy = "CorsPolicy";
        private static readonly string secretKey = "VnN0eVVvZ0Z5c2hCMHFlWFlQZDZnRGRLMmN3NWthblE5N1k3Zm9QbU1nSitIT0haV0JCWkZsaHBiKy9OUEV4SA==";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the IConfiguration instance which MyOptions binds against.
            services.Configure<DemeterConfig>(Configuration);
            Configuration.GetSection("DemeterConfig").Bind(config);
            // Add framework services.
            services.AddAuthentication(authenticationOptions => authenticationOptions.DefaultScheme = "Cookies")
                        .AddCookie(options =>
                        {
                            options.LoginPath = "/api/token";
                            options.Cookie.HttpOnly = false;
                            options.ExpireTimeSpan = TimeSpan.FromHours(1);
                        });

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = false,
                //ValidIssuer = "ExampleIssuer",
                // Validate the JWT Audience (aud) claim
                ValidateAudience = false,
                //ValidAudience = "ExampleAudience",
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.FromMinutes(5)
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.Authority = "ExampleIssuer";
                //options.Audience = "ExampleAudience";
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 404;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    }
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy(PublicPolicy,
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            //services.AddDbContext<DbContext>(options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
        private void ConfigureAuth(IApplicationBuilder app)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            });
            app.UseAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            ConfigureAuth(app);
            app.UseCors(PublicPolicy);
            app.UseMvc();
        }
        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            // Don't do this in production, obviously!
            if (username == config.Username && password == config.Password)
            {
                var claim = new Claim[] { new Claim("username", username), new Claim("password", password) };
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), claim));
            }
            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
