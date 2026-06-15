using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Borrowings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly LibraryDbContext _db;
        public BorrowingRepository(LibraryDbContext db) => _db = db;

        public Task<Borrowing?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _db.Borrowings.FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<bool> HasActiveBorrowingAsync(Guid userId, Guid bookId, CancellationToken ct = default) =>
            _db.Borrowings.AnyAsync(b => b.UserId == userId && b.BookId == bookId && b.ReturnedAt == null, ct);

        public async Task AddAsync(Borrowing borrowing, CancellationToken ct = default) =>
            await _db.Borrowings.AddAsync(borrowing, ct);
    }
}
