using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer("Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
            return new LibraryDbContext(options);
        }
    }
}
