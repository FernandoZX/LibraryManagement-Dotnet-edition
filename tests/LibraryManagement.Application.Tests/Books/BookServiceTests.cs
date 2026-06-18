using LibraryManagement.Application.Books;
using LibraryManagement.Application.Books.Dtos;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Common.Ports;
using LibraryManagement.Domain.Books;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace LibraryManagement.Application.Tests.Books
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _books = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private BookService CreateSut() => new(_books.Object, _uow.Object);

        [Fact]
        public async Task Create_with_duplicate_isbn_returns_conflict()
        {
            _books.Setup(r => r.ExistsByIsbnAsync("123", default)).ReturnsAsync(true);

            var result = await CreateSut().CreateAsync(new CreateBookRequest("T", "A", "G", "123", 3));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Conflict);
        }

        [Fact]
        public async Task Create_persists_and_returns_dto()
        {
            _books.Setup(r => r.ExistsByIsbnAsync(It.IsAny<string>(), default)).ReturnsAsync(false);

            var result = await CreateSut().CreateAsync(
            new CreateBookRequest("Clean Architecture", "Martin", "Software", "999", 5));

            result.IsSuccess.ShouldBeTrue();
            result.Value!.AvailableCopies.ShouldBe(5);
            _books.Verify(r => r.AddAsync(It.IsAny<Book>(), default), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task Update_persists_and_returns_dto()
        {
            Guid bookId = Guid.NewGuid();
            _books.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new Book("T", "A", "G", "123", 3));

            // Act
            var result = await CreateSut().UpdateAsync(bookId, new UpdateBookRequest("A", "C", "G", "124", 5));

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value!.AvailableCopies.ShouldBe(5);
            _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_nonexistent_book_returns_not_found()
        {
            _books.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Book?)null);

            var result = await CreateSut().UpdateAsync(Guid.NewGuid(), new UpdateBookRequest("T", "A", "G", "1", 3));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.NotFound);
            result.Error!.Message.ShouldBe("Book not found.");
        }

        [Fact]
        public async Task Update_with_duplicate_isbn_returns_conflict()
        {
            Guid bookId = Guid.NewGuid();
            _books.Setup(r => r.ExistsByIsbnAsync("123", default)).ReturnsAsync(true);
            _books.Setup(r => r.GetByIdAsync(bookId, default)).ReturnsAsync(new Book("T", "A", "G", "123", 3));

            var result = await CreateSut().UpdateAsync(bookId, new UpdateBookRequest("T", "A", "G", "123", 3));

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.Conflict);
            result.Error!.Message.ShouldBe("A book with this ISBN already exists.");
        }

        [Fact]
        public async Task Delete_nonexistent_book_returns_not_found()
        {
            _books.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Book?)null);

            var result = await CreateSut().DeleteAsync(Guid.NewGuid());

            result.IsSuccess.ShouldBeFalse();
            result.Error!.Type.ShouldBe(ErrorType.NotFound);
            result.Error!.Message.ShouldBe("Book not found.");
        }
    }

}
