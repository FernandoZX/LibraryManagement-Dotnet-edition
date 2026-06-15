using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Dashboard.Dtos
{
    // Vista del Librarian
    public record LibrarianDashboardDto(
        int TotalBooks,
        int TotalBorrowedBooks,
        int BooksDueToday,
        IReadOnlyList<MemberWithOverdueDto> MembersWithOverdueBooks);

    public record MemberWithOverdueDto(Guid UserId, string FullName, string Email, int OverdueCount);

    // Vista del Member
    public record MemberDashboardDto(IReadOnlyList<MemberBorrowingDto> Borrowings);

    public record MemberBorrowingDto(
        Guid BorrowingId, Guid BookId, string BookTitle,
        DateTime BorrowedAt, DateTime DueAt, DateTime? ReturnedAt, bool IsOverdue);
}
