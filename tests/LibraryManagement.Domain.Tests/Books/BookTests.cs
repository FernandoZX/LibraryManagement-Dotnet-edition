using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Common;
using Shouldly;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Domain.Tests.Books
{
    public class BookTests
    {
        private static Book CreateBook(int totalCopies = 3) =>
            new("Clean Architecture", "Robert C. Martin", "Software", "9780134494166", totalCopies);

        [Fact]
        public void New_book_has_all_copies_available()
        {
            var book = CreateBook(totalCopies: 5);
            book.AvailableCopies.ShouldBe(5);
        }

        [Fact]
        public void Borrow_decrements_available_copies()
        {
            var book = CreateBook(totalCopies: 2);
            book.Borrow();
            book.AvailableCopies.ShouldBe(1);
        }

        [Fact]
        public void Borrow_with_no_copies_throws()
        {
            var book = CreateBook(totalCopies: 1);
            book.Borrow();
            Should.Throw<DomainException>(() => book.Borrow());
        }

        [Fact]
        public void Return_increments_available_copies()
        {
            var book = CreateBook(totalCopies: 2);
            book.Borrow();
            book.Return();
            book.AvailableCopies.ShouldBe(2);
        }

        [Fact]
        public void Creating_book_with_empty_title_throws() =>
            Should.Throw<DomainException>(() => new Book("", "Author", "Genre", "123", 1));
    }
}
