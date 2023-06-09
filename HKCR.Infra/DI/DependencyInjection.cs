﻿using System.Text;
using HKCR.Application.Common.Interface;
using HKCR.Domain.Entities;
using HKCR.Infra.Persistence;
using HKCR.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace HKCR.Infra.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("CourseworkDB"),
                    b =>
                        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)),
            ServiceLifetime.Transient);

        // Added Microsoft.AspNetCore.Identity.UI package for AddIdentity method
        // services.AddIdentity<IdentityUser, IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddDefaultTokenProviders();

        services.AddIdentity<AddUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();


        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        services.AddScoped<UserManager<AddUser>, UserManager<AddUser>>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IAuthentication, AuthenticationService>();
        services.AddTransient<ICarDetails, CarDetails>();
        services.AddTransient<IDocDetails, DocDetails>();
        services.AddTransient<IPaymentDetails, PaymentService>();
        services.AddTransient<IRentalDetails, RentalServices>();
        services.AddTransient<IRentDetails, RentService>();
        services.AddTransient<IOffersDetails, OffersService>();
        services.AddTransient<IDamageRequestDetails, DamageRequestService>();
        // services.AddTransient<IUserDetails, UserDetails>();
        // services.AddTransient<ICustomerDetails, CustomerService>();

        return services;
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("jwtConfig");
        var secretKey = jwtConfig["secret"];
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["validIssuer"],
                    ValidAudience = jwtConfig["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
    }
}