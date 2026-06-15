using FluentValidation;
using LibraryManagement.Application.Auth;
using LibraryManagement.Application.Books;
using LibraryManagement.Application.Borrowings;
using LibraryManagement.Application.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LibraryManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBorrowingService, BorrowingService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddSingleton(TimeProvider.System);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
