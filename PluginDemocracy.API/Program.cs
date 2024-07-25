using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using PluginDemocracy.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PluginDemocracy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel server limits
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 629145600; // 600MB
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
            });

            //Logging 
            // Add services to the container.
            builder.Services
                .AddControllers(options =>
                {
                    options.Filters.Add<ModelStateFeatureFilter>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                  options.SuppressModelStateInvalidFilter = true; // Optional: If you want to suppress the automatic 400 response
                })
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                })
              ;
            builder.Services.AddLogging();

            // Add services to the container.
            //Authentication
            string secret = Environment.GetEnvironmentVariable("JsonWebTokenSecretKey") ?? string.Empty;
            if (string.IsNullOrEmpty(secret)) throw new Exception("JsonWebTokenSecretKey is null or empty");
            byte[] key = Encoding.ASCII.GetBytes(secret);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = 401; //Unauthorized
                                context.Response.ContentType = "application/json";

                                //Manually constructing the PDAPIResponse equivalent JSON
                                var response = new
                                {
                                    Alerts = new List<object> { new { Severity = "info", Message = "Session has expired. Please log in again." } },
                                    RedirectTo = (string?)null,
                                    RedirectParameters = new Dictionary<string, string>(),
                                    SessionJWT = (string?)null,
                                    LogOut = true
                                };

                                var json = JsonSerializer.Serialize(response);
                                context.Response.WriteAsync(json);
                                //prevent the default ASP.NET Core response:
                                context.NoResult();
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddDbContext<PluginDemocracyContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PluginDemocracyDatabase"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5, // Maximum number of retries
                            maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                            errorNumbersToAdd: null); // SQL error numbers to consider for retries
                        sqlOptions.MigrationsAssembly("PluginDemocracy.Data");
                    }));

            builder.Services.AddScoped<APIUtilityClass>();
            builder.Logging.AddConsole();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                c =>
                {
                    c.ResolveConflictingActions(apiDescriptions =>
                    {
                        // Custom logic to resolve conflicts
                        return apiDescriptions.First();
                    });
                }
                );

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyCorsPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:7132")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            var app = builder.Build();
            app.UseMiddleware<ApiLoggingMiddleware>(); //I added this for logging 

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("MyCorsPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}
