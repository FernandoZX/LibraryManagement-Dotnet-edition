using LibraryManagement.Domain.Borrowings;
using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;

namespace LibraryManagement.Domain.Tests.Borrowings
{
    public class BorrowingTests
    {
        private static readonly DateTime BorrowedAt = new(2026, 1, 1);

        [Fact]
        public void Due_date_is_two_weeks_after_borrow()
        {
            var borrowing = new Borrowing(Guid.NewGuid(), Guid.NewGuid(), BorrowedAt);
            borrowing.DueAt.ShouldBe(BorrowedAt.AddDays(14));
        }

        [Fact]
        public void Is_overdue_when_past_due_and_not_returned()
        {
            var borrowing = new Borrowing(Guid.NewGuid(), Guid.NewGuid(), BorrowedAt);
            borrowing.IsOverdue(BorrowedAt.AddDays(15)).ShouldBeTrue();
        }

        [Fact]
        public void Is_not_overdue_after_returned()
        {
            var borrowing = new Borrowing(Guid.NewGuid(), Guid.NewGuid(), BorrowedAt);
            borrowing.MarkAsReturned(BorrowedAt.AddDays(20));
            borrowing.IsOverdue(BorrowedAt.AddDays(25)).ShouldBeFalse();
        }
    }
}
