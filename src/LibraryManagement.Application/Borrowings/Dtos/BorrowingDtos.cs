using LibraryManagement.Domain.Borrowings;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Borrowings.Dtos
{
    public record BorrowBookRequest(Guid BookId);
    public record BorrowingDto(
        Guid Id, Guid BookId, string BookTitle, Guid UserId,
        DateTime BorrowedAt, DateTime DueAt, DateTime? ReturnedAt);

    public static class BorrowingMappings
    {
        public static BorrowingDto ToDto(this Borrowing b, string bookTitle) =>
            new(b.Id, b.BookId, bookTitle, b.UserId, b.BorrowedAt, b.DueAt, b.ReturnedAt);
    }
}
