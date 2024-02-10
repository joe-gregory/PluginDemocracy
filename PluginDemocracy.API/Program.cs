using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using PluginDemocracy.Data;
using System.Text;

namespace PluginDemocracy.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
