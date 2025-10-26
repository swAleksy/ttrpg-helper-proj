using System.Text;
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
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true; 
            options.AppendTrailingSlash = true;
        });
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AppSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                    ValidateIssuerSigningKey = true
                };
            });
        
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ICharacterService, CharacterService>();

        
        var app = builder.Build();  
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapControllers();
            app.MapOpenApi(); // Serves the OpenAPI spec at /openapi/v1.json
            app.MapScalarApiReference();
        }
        
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}