using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Infrastructure.Persistence.Repositories;
using LibraryManagement.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
