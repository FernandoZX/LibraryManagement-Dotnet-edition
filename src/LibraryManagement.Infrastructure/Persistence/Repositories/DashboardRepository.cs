using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Application.Dashboard.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Infrastructure.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly LibraryDbContext _db;
        public DashboardRepository(LibraryDbContext db) => _db = db;

        public async Task<LibrarianDashboardDto> GetLibrarianDashboardAsync(DateTime now, CancellationToken ct = default)
        {
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            var totalBooks = await _db.Books.CountAsync(ct);

            var totalBorrowed = await _db.Borrowings.CountAsync(b => b.ReturnedAt == null, ct);

            var dueToday = await _db.Borrowings
            .CountAsync(b => b.ReturnedAt == null && b.DueAt >= today && b.DueAt < tomorrow, ct);

            var membersWithOverdue = await _db.Borrowings
            .Where(b => b.ReturnedAt == null && b.DueAt < now)
            .Join(_db.Users, b => b.UserId, u => u.Id, (b, u) => new { b, u })
            .GroupBy(x => new { x.u.Id, x.u.FullName, x.u.Email })
            .Select(g => new MemberWithOverdueDto(g.Key.Id, g.Key.FullName, g.Key.Email, g.Count()))
            .ToListAsync(ct);

            return new LibrarianDashboardDto(totalBooks, totalBorrowed, dueToday, membersWithOverdue);
        }

        public async Task<MemberDashboardDto> GetMemberDashboardAsync(Guid userId, DateTime now, CancellationToken ct = default)
        {
            var borrowings = await _db.Borrowings
            .Where(b => b.UserId == userId)
            .Join(_db.Books, b => b.BookId, bk => bk.Id, (b, bk) => new { b, bk })
            .OrderByDescending(x => x.b.BorrowedAt)
            .Select(x => new MemberBorrowingDto(
                x.b.Id, x.bk.Id, x.bk.Title,
                x.b.BorrowedAt, x.b.DueAt, x.b.ReturnedAt,
                x.b.ReturnedAt == null && x.b.DueAt < now))
            .ToListAsync(ct);

            return new MemberDashboardDto(borrowings);
        }
    }
}
