using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TtrpgHelperBackend.Services;

namespace TtrpgHelperBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
        // {
        //     x.TokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidIssuer = builder.Configuration["Tokens:Issuer"],
        //     }
        // });
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        // 💡 ADD THIS SECTION TO ALLOW TRAILING SLASHES
        builder.Services.Configure<RouteOptions>(options =>
        {
            // Setting this to true makes the router match both /resource and /resource/
            // for all endpoints.
            options.LowercaseUrls = true; // Optional, but good practice
            options.AppendTrailingSlash = true;
        });
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        builder.Services.AddScoped<IAuthService, AuthService>();

        
        var app = builder.Build();  
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapControllers();
            app.MapOpenApi(); // Serves the OpenAPI spec at /openapi/v1.json
            app.MapScalarApiReference();
        }
        
        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}