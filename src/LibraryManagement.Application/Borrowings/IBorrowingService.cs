using LibraryManagement.Application.Borrowings.Dtos;
using LibraryManagement.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Borrowings
{
    public interface IBorrowingService
    {
        Task<Result<BorrowingDto>> BorrowAsync(Guid userId, BorrowBookRequest request, CancellationToken ct = default);
        Task<Result<BorrowingDto>> ReturnAsync(Guid borrowingId, CancellationToken ct = default);
    }
}
