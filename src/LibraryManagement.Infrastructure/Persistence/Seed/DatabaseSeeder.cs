using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(LibraryDbContext db, IPasswordHasher hasher, CancellationToken ct = default)
        {
            await db.Database.MigrateAsync(ct); // aplica migrations pendientes

            if ( !await db.Users.AnyAsync(ct) )
            {
                db.Users.AddRange(
                    new User("librarian@library.com", hasher.Hash("Password123!"), "Library Admin", UserRole.Librarian),
                    new User("member@library.com", hasher.Hash("Password123!"), "John Member", UserRole.Member));
            }

            if ( !await db.Books.AnyAsync(ct) )
            {
                db.Books.AddRange(
                    new Book("Clean Architecture", "Robert C. Martin", "Software", "9780134494166", 3),
                    new Book("The Pragmatic Programmer", "Hunt & Thomas", "Software", "9780201616224", 2),
                    new Book("Domain-Driven Design", "Eric Evans", "Software", "9780321125217", 1));
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
