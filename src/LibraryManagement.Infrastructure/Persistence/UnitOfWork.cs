using LibraryManagement.Application.Common.Ports;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _db;
        public UnitOfWork(LibraryDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
