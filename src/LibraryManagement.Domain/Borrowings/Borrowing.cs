using LibraryManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Borrowings
{
    public class Borrowing : Entity
    {
        public const int LoanPeriodInDays = 14;

        public Guid BookId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime BorrowedAt { get; private set; }
        public DateTime DueAt { get; private set; }
        public DateTime? ReturnedAt { get; private set; }

        private Borrowing() { } // EF Core

        public Borrowing(Guid bookId, Guid userId, DateTime borrowedAt)
        {
            if (bookId == Guid.Empty) throw new DomainException("BookId is required.");
            if (userId == Guid.Empty) throw new DomainException("UserId is required.");

            BookId = bookId;
            UserId = userId;
            BorrowedAt = borrowedAt;
            DueAt = borrowedAt.AddDays(LoanPeriodInDays);
        }

        public bool IsReturned => ReturnedAt.HasValue;

        public bool IsOverdue(DateTime asOf) => !IsReturned && asOf > DueAt;

        public void MarkAsReturned(DateTime returnedAt)
        {
            if (IsReturned)
                throw new DomainException("This borrowing has already been returned.");
            ReturnedAt = returnedAt;
        }
    }
}
