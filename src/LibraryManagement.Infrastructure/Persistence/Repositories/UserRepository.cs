using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryDbContext _db;
        public UserRepository(LibraryDbContext db) => _db = db;

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
            _db.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct = default) =>
            await _db.Users.AddAsync(user, ct);
    }
}
