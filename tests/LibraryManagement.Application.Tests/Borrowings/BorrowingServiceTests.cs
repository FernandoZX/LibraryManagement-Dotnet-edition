using LibraryManagement.Application.Borrowings;
using LibraryManagement.Application.Borrowings.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Books;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Shouldly;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Application.Tests.Borrowings
{
    public class BorrowingServiceTests
    {
        private readonly Mock<IBorrowingRepository> _borrowings = new();
        private readonly Mock<IBookRepository> _books = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        private BorrowingService CreateSut() => new(_borrowings.Object, _books.Object, _uow.Object, _clock);
        private static Book AvailableBook() => new("Clean Architecture", "Martin", "Software", "999", 2);

        [Fact]
        public async Task Borrow_unavailable_book_returns_conflict()
        {
            var book = new Book("T", "A", "G", "1", 1);
            book.Borrow(); // queda en 0 disponibles
            _books.Setup(r => r.GetByIdAsync(book.Id, default)).ReturnsAsync(book);

            var result = await CreateSut().BorrowAsync(Guid.NewGuid(), new BorrowBookRequest(book.Id));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Conflict);
        }

        [Fact]
        public async Task Borrow_same_book_twice_returns_conflict()
        {
            var book = AvailableBook();
            var userId = Guid.NewGuid();
            _books.Setup(r => r.GetByIdAsync(book.Id, default)).ReturnsAsync(book);
            _borrowings.Setup(r => r.HasActiveBorrowingAsync(userId, book.Id, default)).ReturnsAsync(true);

            var result = await CreateSut().BorrowAsync(userId, new BorrowBookRequest(book.Id));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Conflict);
        }

        [Fact]
        public async Task Borrow_success_decrements_copies_and_sets_due_in_two_weeks()
        {
            var book = AvailableBook(); // 2 copias
            var userId = Guid.NewGuid();
            _books.Setup(r => r.GetByIdAsync(book.Id, default)).ReturnsAsync(book);
            _borrowings.Setup(r => r.HasActiveBorrowingAsync(userId, book.Id, default)).ReturnsAsync(false);

            var result = await CreateSut().BorrowAsync(userId, new BorrowBookRequest(book.Id));

            result.IsSuccess.ShouldBeTrue();
            book.AvailableCopies.ShouldBe(1);
            result.Value!.DueAt.ShouldBe(new DateTime(2026, 1, 15)); // 1 ene + 14 días, determinista
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
