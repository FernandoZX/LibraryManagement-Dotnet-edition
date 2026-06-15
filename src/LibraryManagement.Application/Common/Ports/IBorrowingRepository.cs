using LibraryManagement.Domain.Borrowings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Common.Ports
{
    public interface IBorrowingRepository
    {
        Task<Borrowing?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> HasActiveBorrowingAsync(Guid userId, Guid bookId, CancellationToken ct = default);
        Task AddAsync(Borrowing borrowing, CancellationToken ct = default);
    }
}
