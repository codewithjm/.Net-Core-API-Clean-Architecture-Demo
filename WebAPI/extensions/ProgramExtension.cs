using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using WebAPI.Middlewares;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

namespace WebAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class ProgramExtension
{
    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        _ = builder.Host.UseSerilog((_, lc) => lc
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(GetSerilogFileConfig(), rollingInterval: RollingInterval.Day));
        _ = builder.Services.Configure((JsonOptions opt) =>
        {
            opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.SerializerOptions.PropertyNameCaseInsensitive = true;
            opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        _ = builder.Services.AddEndpointsApiExplorer();
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JSON Web Token based security",
        };
        var securityRequirements = new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        };
        var securityContactInfo = new OpenApiContact()
        {
            Name = "Teleperformance USA",
            Email = "www.wfmesp.teleperformanceusa.com",
            Url = new Uri("https://wfmesp.teleperformanceusa.com/")
        };
        var securityInfo = new OpenApiInfo()
        {
            Version = "v1",
            Title = "Employee Scheduling Portal APIs",
            Description = "One stop shop of all the apis of ESP API.",
            Contact = securityContactInfo
        };
        _ = builder.Services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", securityInfo);
            //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.EnableAnnotations();
            options.DocInclusionPredicate((_, _) => true);

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(securityRequirements);
        });

        _ = builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("Access:ADMIN", p => p.
            RequireAuthenticatedUser().
            RequireClaim("AccessLevel", "ADMIN"));
        });

        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(option => option.WithOrigins("http://localhost:4200", "https://localhost:3000",
                "https://dev-wfmesp.teleperformanceusa.com", "https://wfmesp.teleperformanceusa.com")
                .AllowAnyMethod().AllowAnyHeader());
        });
        //builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        //builder.Services.AddBusiness();
        //builder.Services.AddPersistence(builder.Configuration);
        //builder.Services.AddShared(builder.Configuration);
        return builder;
    }

    private static string GetSerilogFileConfig()
    {
        string logPath = AppDomain.CurrentDomain.BaseDirectory;
        return @$"{logPath}\logs\log-.txt";
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        var ti = CultureInfo.CurrentCulture.TextInfo;
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Local")
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c =>
                                 c.SwaggerEndpoint("/swagger/v1/swagger.json",
                                 $"ESP Api - {ti.ToTitleCase(app.Environment.EnvironmentName)} - v1"));
        }
        else
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c =>
                                 c.SwaggerEndpoint("/api/swagger/v1/swagger.json",
                                 $"ESP Api - {ti.ToTitleCase(app.Environment.EnvironmentName)} - v1"));
        }


        app.UseCors();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.UseHttpsRedirection();
        _ = app.UseMiddleware<GlobalErrorHandlingMiddleware>();

        using (var scope = app.Services.CreateScope())
        {
            //var dataContext = scope.ServiceProvider.GetRequiredService<ESPDbContext>();
            ////dataContext.Database.Migrate();
            //var dataTPSContext = scope.ServiceProvider.GetService<TPSWebDbContext>();
        }


        return app;
    }

}
