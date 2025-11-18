using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Services;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddAuthorization();
        
        // na razie testowo
        // do helpera Userowego
        // chodzi o to żeby mieć uniwersalną klasę
        // z metodami do pozyskiwania wartości zamiast ciągle
        // pisać to samo w różnych klasach
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<UserHelper>();
        
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
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"].FirstOrDefault();
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICharacterService, CharacterService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        builder.Services.AddScoped<ICampaignService, CampaignService>();
        builder.Services.AddScoped<ISessionService, SessionService>();

        builder.Services.AddSignalR();

        var app = builder.Build();  
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
        
        app.UseStaticFiles();
        app.MapHub<ChatHub>("/chatHub");

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