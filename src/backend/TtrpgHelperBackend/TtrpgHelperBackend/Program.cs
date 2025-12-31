using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TtrpgHelperBackend.Helpers;
using TtrpgHelperBackend.MessagesAndNotofications;
using TtrpgHelperBackend.Seed;
using TtrpgHelperBackend.Services;
using TtrpgHelperBackend.Services.Resource;
using TtrpgHelperBackend.Services.Session;

namespace TtrpgHelperBackend;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddAuthorization();
        
        // CORS – pozwalamy frontowi na innym porcie (Vite: http://localhost:5174)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "http://localhost:5174")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();  // jeśli cookies ??
            
            });
        });


        // na razie testowo
        // do helpera Userowego
        // chodzi o to żeby mieć uniwersalną klasę
        // z metodami do pozyskiwania wartości zamiast ciągle
        // pisać to samo w różnych klasach
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<UserHelper>();
        
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                // This converts Enums to Strings globally
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
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
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/mainHub") || path.StartsWithSegments("/GameSessionHub")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        // ==========
        // -- AUTH --
        builder.Services.AddScoped<IUserService, UserService>();
        
        // ================
        // -- CHARACTERS --
        builder.Services.AddScoped<ICharacterService, CharacterService>();
        
        // ===================================
        // -- TO TO NIE WIEM PO CO TUTAJ XD --
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        
        // =============
        // -- SESSION --
        builder.Services.AddScoped<ICampaignService, CampaignService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ISessionEventService, SessionEventService>();
        
        // ==============
        // -- RESOURCE --
        builder.Services.AddScoped<IScenarioService, ScenarioService>();
        builder.Services.AddScoped<IScenarioChapterService, ScenarioChapterService>();
        builder.Services.AddScoped<INoteService, NoteService>();
        builder.Services.AddScoped<IItemService, ItemService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<INpcService, NpcService>();
        
        // ==========================
        // -- CHAT & NOTIFICATIONS --
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IUploadService, UploadService>();
        builder.Services.AddScoped<IFriendService, FriendService>();
        
        builder.Services.AddSignalR();

        var app = builder.Build();  
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
            await DataSeeder.Seed(dbContext);
        }
        
        app.UseStaticFiles();
        // CORS
        app.UseCors("FrontendPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Configure the HTTP request pipeline.
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); // Serves the OpenAPI spec at /openapi/v1.json
            app.MapScalarApiReference();
        }
        
        //app.UseHttpsRedirection();
        
        app.MapHub<MainHub>("/mainHub");
        app.MapHub<GameSessionHub>("/GameSessionHub");

        app.Run();
    }
}