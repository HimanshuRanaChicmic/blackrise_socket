using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Domain;
using BlackRise.Identity.Persistence.CustomTokenProvider;
using BlackRise.Identity.Persistence.Services;
using BlackRise.Identity.Persistence.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlackRise.Identity.Persistence;

public static class PersistenceServiceRegisteration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientUrlSetting>(configuration.GetSection("ClientUrlSettings"));
        services.Configure<JwtSetting>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSetting>(configuration.GetSection("EmailSettings"));

        return services;
    }

    public static void AddIdentityServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        _ = services.AddDbContext<IdentityDbContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString"),
                b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName));
        });

        _ = services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.User.RequireUniqueEmail = true;

            opt.SignIn.RequireConfirmedEmail = true;

            opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            opt.Tokens.PasswordResetTokenProvider = "resetpassword";
        })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation")
            .AddTokenProvider<ResetPasswordTokenProvider<ApplicationUser>>("resetpassword");

        services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes(double.Parse(builder.Configuration.GetSection("ClientUrlSettings:EmailConfirmationTokenExpire").Value ?? string.Empty)));

        services.Configure<ResetPasswordTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes(double.Parse(builder.Configuration.GetSection("ClientUrlSettings:ResetPasswordTokenExpire").Value ?? string.Empty)));

        _ = services.AddTransient<IAuthService, AuthService>();

        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? string.Empty))
                };

                o.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.Response.OnStarting(async () =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "text/plain";

                            await context.Response.WriteAsync("401 not authorized");
                        });

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.OnStarting(async () =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "text/plain";

                            await context.Response.WriteAsync("403 forbidden");
                        });

                        return Task.CompletedTask;
                    }
                };

            });

    }
}
